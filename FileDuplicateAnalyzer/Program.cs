using FileDuplicateAnalyzer.Commands;
using FileDuplicateAnalyzer.Services.CmdLine;
using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static FileDuplicateAnalyzer.Services.CmdLine.Globals;

using cons = AnsiVtConsole.NetCore;

namespace FileDuplicateAnalyzer;

/// <summary>
/// main
/// </summary>
public class Program
{
    private IServiceProvider? _serviceProvider;
    private CommandsSet? _commandSet;
    private GlobalArgsSet? _globalArgsSet;

    /// <summary>
    /// command line input
    /// <para>FileDuplicateAnalyzer command [options]</para>
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>status code</returns>
    public static int Main(string[] args)
        => new Program()
            .Run(args);

    /// <summary>
    /// run the command line arguments
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>exit code</returns>
    /// <exception cref="Exception"></exception>
    public int Run(string[] args)
    {
        var argList = args.ToList();

        try
        {
            var host = CreateHost(argList);
            host.StartAsync();
            var texts = host.Services.GetRequiredService<Texts>();
            var console = host.Services.GetRequiredService<IConsole>();
            var lineBreak = false;
            try
            {
                if (args.Length == 0)
                    throw new Exception(texts._("MissingArguments"));

                var command = GetCommand(texts, args[0]);
                console.Out?.WriteLn();
                lineBreak = true;
                var exitCode = command.Run(argList.ToArray()[1..]);

                console.Out?.WriteLn();

                return exitCode;
            }
            catch (Exception commandExecutionException)
            {
                return ExitWithError(
                    commandExecutionException,
                    console,
                    lineBreak);
            }
        }
        catch (Exception hostBuilderException)
        {
            return ExitWithError(
                hostBuilderException,
                new Services.IO.Console(new cons.AnsiVtConsole()),
                true);
        }
    }

    private static int ExitWithError(
        Exception ex,
        IConsole console,
        bool lineBreak)
    {
        if (!lineBreak)
            console.Err?.Logln();
        console.Err?.Logln(ex.Message);
        console.Err?.Logln();
        return ExitFail;
    }

    private IHost CreateHost(List<string> args)
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder
            .ConfigureAppConfiguration(
                configure =>
                {
                    configure.AddJsonFile(
                        ConfigFilePath,
                        optional: false);
                    var cultureConfigFileName = $"{ConfigFilePrefix}{Thread.CurrentThread.CurrentCulture.Name}{ConfigFilePostfix}";
                    configure.AddJsonFile(cultureConfigFileName, optional: false);
                })
            .ConfigureServices(
                services => services
                    .AddSingleton<Texts>()
                    .AddCommands(out _commandSet)
                    .AddArguments(out _globalArgsSet)
                    .ParseGlobalArguments(
                        args,
                        _globalArgsSet,
                        out _)
                    .ConfigureOutput());

        var host = hostBuilder.Build();
        _serviceProvider = host.Services;
        return host;
    }

    private Command GetCommand(Texts texts, string commandName)
        => !_commandSet!.Commands.TryGetValue(commandName, out var commandType)
            ? throw new Exception(texts._("UnknownCommand", commandName))
            : (Command)_serviceProvider!.GetRequiredService(commandType);
}
