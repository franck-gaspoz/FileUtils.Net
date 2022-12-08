using FileDuplicateAnalyzer.Commands;
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
    private readonly Texts _texts = Texts.Instance;
    private IServiceProvider? _serviceProvider;
    private CommandsSet? _commandSet;

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

        IHost? host = CreateHost();
        host.StartAsync();

        Command? command = GetCommand(args[0]);
        return command.Run(args[1..]);
    }

    private IHost CreateHost()
    {
        IHostBuilder? hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder
            .ConfigureServices(
                services =>
                {
                    services.AddSingleton<IOutput, Output>();
                    AddCommands(services);
                });

        IHost? host = hostBuilder.Build();
        _serviceProvider = host.Services;
        return host;
    }

    private void AddCommands(IServiceCollection services)
    {
        _commandSet = new CommandsSet();

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