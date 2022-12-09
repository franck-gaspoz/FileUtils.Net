using FileDuplicateAnalyzer.Commands;
using FileDuplicateAnalyzer.GlobalArgs;
using FileDuplicateAnalyzer.Services.CmdLine;
using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static FileDuplicateAnalyzer.Services.CmdLine.Globals;

namespace FileDuplicateAnalyzer;

/// <summary>
/// main
/// </summary>
public class Program
{
    private readonly Texts _texts = new();
    private IServiceProvider? _serviceProvider;
    private CommandsSet? _commandSet;
    private GlobalArgsSet? _globalArgsSet;
    private SettedGlobalArgsSet? _settedGlobalArgsSet;

    /// <summary>
    /// command line input
    /// <para>FileDuplicateAnalyzer command [options]</para>
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>status code</returns>
    public static int Main(string[] args)
    {
        try
        {
            return new Program()
                .Startup(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return ExitFail;
        }
    }

    internal int Startup(string[] args)
    {
        if (args.Length == 0)
            throw new Exception(_texts._(Texts.MissingArguments));

        List<string>? argList = args.ToList();
        IHost? host = CreateHost(argList);
        host.StartAsync();

        Command? command = GetCommand(args[0]);
        return command.Run(argList.ToArray()[1..]);
    }

    private IHost CreateHost(List<string> args)
    {
        IHostBuilder? hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder
            .ConfigureServices(
                services =>
                {
                    services.AddSingleton(_texts);
                    AddCommands(services);
                    AddArguments(services, args);
                });

        ConfigureOutput(args, hostBuilder);

        IHost? host = hostBuilder.Build();
        _serviceProvider = host.Services;
        return host;
    }

    private static void ConfigureOutput(List<string> args, IHostBuilder hostBuilder)
    {
        if (!GlobalArgsSet.ExistsInArgList(typeof(SGlobalArg), args))
        {
            hostBuilder
                .ConfigureServices(services =>
                    services.AddSingleton<IOutput, Output>());
        }
        else
        {
            hostBuilder
                .ConfigureServices(services =>
                    services.AddSingleton<IOutput, SilentOutput>());
        }
    }

    private void AddArguments(
        IServiceCollection services,
        List<string> args)
    {
        _globalArgsSet = new();
        _settedGlobalArgsSet = new();

        foreach (Type classType in
            GetType()
                .Assembly
                .GetTypes())
        {
            if (classType.InheritsFrom(typeof(GlobalArg)))
            {
                string? argName = GlobalArg.ClassNameToArgName(
                    classType.Name);

                _globalArgsSet.Add(argName, classType);

                if (GlobalArgsSet.ExistsInArgList(
                    classType, args
                    ))
                {
                    string? prefixedName = GlobalArg.GetPrefixFromArgName(argName)
                        + argName;
                    args.Remove(prefixedName);
                    _settedGlobalArgsSet.Add(argName, classType);
                }
            }
        }

        services.AddSingleton(_globalArgsSet);
    }

    private void AddCommands(IServiceCollection services)
    {
        _commandSet = new(_texts);

        foreach (Type classType in
            GetType()
                .Assembly
                .GetTypes())
        {
            if (classType.InheritsFrom(typeof(Command)))
            {
                services.AddSingleton(classType);

                _commandSet.Add(
                    classType
                        .Name[0..^7]
                        .ToLower(),
                    classType);
            }
        }

        services.AddSingleton(_commandSet);
    }

    private Command GetCommand(string commandName)
        => !_commandSet!.Commands.TryGetValue(commandName, out Type? commandType)
            ? throw new Exception(_texts._(Texts.UnknownCommand, commandName))
            : (Command)_serviceProvider!.GetRequiredService(commandType);
}