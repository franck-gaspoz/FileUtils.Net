using FileDuplicateAnalyzer.Commands;
using FileDuplicateAnalyzer.Services.CmdLine;
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

    /// <summary>
    /// command line input
    /// <para>FileDuplicateAnalyzer command [options]</para>
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>status code</returns>
    public static int Main(string[] args)
        => new Program()
            .Startup(args);

    /// <summary>
    /// starts the program using command line arguments
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>exit code</returns>
    /// <exception cref="Exception"></exception>
    public int Startup(string[] args)
    {
        try
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
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return ExitFail;
        }
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
