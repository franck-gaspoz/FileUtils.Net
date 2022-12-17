using AnsiVtConsole.NetCore;

using FileDuplicateAnalyzer.Services.CmdLine;
using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.DependencyInjection;

using static FileDuplicateAnalyzer.Services.CmdLine.Globals;

using cons = AnsiVtConsole.NetCore;

namespace FileDuplicateAnalyzer;

/// <summary>
/// main
/// </summary>
public class Program
{
    /// <summary>
    /// command line input
    /// <para>FileDuplicateAnalyzer command [options]</para>
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>status code</returns>
    public static int Main(string[] args)
        => Program.Run(args);

    /// <summary>
    /// run the command line arguments
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>exit code</returns>
    public static int Run(string[] args)
    {
        var argList = args.ToList();

        try
        {
            var host = new AppHostBuilder(argList).AppHost;
            var texts = host.Services.GetRequiredService<Texts>();
            var console = host.Services.GetRequiredService<IAnsiVtConsole>();
            var commandSet = host.Services.GetRequiredService<CommandsSet>();
            var lineBreak = false;
            try
            {
                if (args.Length == 0)
                    throw new ArgumentException(texts._("MissingArguments"));

                console.Out.WriteLine();
                lineBreak = true;

                var command = commandSet.GetCommand(args[0]);
                var exitCode = command.Run(argList.ToArray()[1..]);

                console.Out.WriteLine();

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
                new cons.AnsiVtConsole(),
                false);
        }
    }

    private static int ExitWithError(
        Exception ex,
        IAnsiVtConsole console,
        bool lineBreak)
    {
        if (!lineBreak)
            console.Logger.LogError();
        console.Logger.LogError(ex.Message);
        console.Logger.LogError();
        return ExitFail;
    }
}

