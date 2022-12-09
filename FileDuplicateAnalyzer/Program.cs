using FileDuplicateAnalyzer.Commands;
using FileDuplicateAnalyzer.GlobalArgs;
using FileDuplicateAnalyzer.Services.CmdLine;
using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static FileDuplicateAnalyzer.Services.CmdLine.Globals;

namespace FileDuplicateAnalyzer;

/// <summary>
/// main
/// </summary>
public class Program
{
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
        List<string>? argList = args.ToList();
        IHost? host = CreateHost(argList);
        host.StartAsync();

        Texts texts = host.Services.GetRequiredService<Texts>();

        if (args.Length == 0)
            throw new Exception(texts._("missingArguments"));

        Command? command = GetCommand(texts, args[0]);
        return command.Run(argList.ToArray()[1..]);
    }

    private IHost CreateHost(List<string> args)
    {
        IHostBuilder? hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder
            .ConfigureAppConfiguration(
                configure =>
                {
                    configure.AddJsonFile("config/appSettings.json", optional: false);
                    string? cultureConfigFileName = $"config/appSettings.{Thread.CurrentThread.CurrentCulture.Name}.json";
                    if (File.Exists(cultureConfigFileName))
                        configure.AddJsonFile(cultureConfigFileName, optional: false);
                })
            .ConfigureServices(
                services =>
                {
                    services.AddSingleton<Texts>();
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
        _commandSet = new(services.BuildServiceProvider().GetRequiredService<Texts>());

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

    private Command GetCommand(Texts texts, string commandName)
        => !_commandSet!.Commands.TryGetValue(commandName, out Type? commandType)
            ? throw new Exception(texts._("unknownCommand", commandName))
            : (Command)_serviceProvider!.GetRequiredService(commandType);
}