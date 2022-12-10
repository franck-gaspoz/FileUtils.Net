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
        var argList = args.ToList();
        var host = CreateHost(argList);
        host.StartAsync();

        var texts = host.Services.GetRequiredService<Texts>();

        if (args.Length == 0)
            throw new Exception(texts._("MissingArguments"));

        var command = GetCommand(texts, args[0]);
        return command.Run(argList.ToArray()[1..]);
    }

    private IHost CreateHost(List<string> args)
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder
            .ConfigureAppConfiguration(
                configure =>
                {
                    configure.AddJsonFile("config/appSettings.json", optional: false);
                    var cultureConfigFileName = $"config/appSettings.{Thread.CurrentThread.CurrentCulture.Name}.json";
                    if (File.Exists(cultureConfigFileName))
                        configure.AddJsonFile(cultureConfigFileName, optional: false);
                })
            .ConfigureServices(
                services =>
                {
                    services.AddSingleton<Texts>();
                    AddCommands(services);
                    AddArguments(services);
                    ParseGlobalArguments(args, services);
                });

        ConfigureOutput(args, hostBuilder);

        var host = hostBuilder.Build();
        _serviceProvider = host.Services;
        return host;
    }

    private void ParseGlobalArguments(List<string> args, IServiceCollection services)
    {
        _settedGlobalArgsSet = new();
        foreach (var kvp in _globalArgsSet!.Parse(
            services.BuildServiceProvider(),
            args))
        {
            _settedGlobalArgsSet.Add(kvp.Value);
        }
        services.AddSingleton(_settedGlobalArgsSet);
    }

    private static void ConfigureOutput(
        IServiceCollection services,
        IHostBuilder hostBuilder)
    {
        var serviceProvider = services.BuildServiceProvider();
        var settedGlobalArgs = serviceProvider.GetRequiredService<SettedGlobalArgsSet>();

        if (!settedGlobalArgs.Contains<SGlobalArg>())
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

    private void AddArguments(IServiceCollection services)
    {
        _globalArgsSet = new();

        foreach (var classType in
            GetType()
                .Assembly
                .GetTypes())
        {
            if (classType.InheritsFrom(typeof(GlobalArg)))
            {
                var argName = GlobalArg.ClassNameToArgName(
                    classType.Name);

                _globalArgsSet.Add(argName, classType);
                services.AddTransient(classType);

                /*if (GlobalArgsSet.ExistsInArgList(
                    classType, args
                    ))
                {
                    string? prefixedName = GlobalArg.GetPrefixFromArgName(argName)
                        + argName;
                    args.Remove(prefixedName);
                    _settedGlobalArgsSet.Add(argName, classType);
                }*/
            }
        }

        services.AddSingleton(_globalArgsSet);
    }

    private void AddCommands(IServiceCollection services)
    {
        _commandSet = new(services.BuildServiceProvider().GetRequiredService<Texts>());

        foreach (var classType in
            GetType()
                .Assembly
                .GetTypes())
        {
            if (classType.InheritsFrom(typeof(Command)))
            {
                services.AddSingleton(classType);

                _commandSet.Add(
                    Command.ClassNameToCommandName(classType.Name),
                    classType);
            }
        }

        services.AddSingleton(_commandSet);
    }

    private Command GetCommand(Texts texts, string commandName)
        => !_commandSet!.Commands.TryGetValue(commandName, out var commandType)
            ? throw new Exception(texts._("UnknownCommand", commandName))
            : (Command)_serviceProvider!.GetRequiredService(commandType);
}
