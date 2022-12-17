
using AnsiVtConsole.NetCore;

using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileDuplicateAnalyzer.Commands;

internal abstract class Command
{
    protected readonly IAnsiVtConsole _console;
    protected readonly Texts _texts;
    protected readonly IConfiguration _config;

    public Command(
        IConfiguration config,
        IAnsiVtConsole console,
        Texts texts)
    {
        _config = config;
        _texts = texts;
        _console = console;
    }

    /// <summary>
    /// exec command
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>return code</returns>
    public abstract int Run(string[] args);

    /// <summary>
    /// short description of the command
    /// </summary>
    /// <returns>text of the description</returns>
    public string ShortDescription()
        => _config.GetValue<string>($"Commands:{ClassNameToCommandName()}:ShortDesc")
        ?? _texts._("CommandShortHelpNotFound", ClassNameToCommandName())!;

    /// <summary>
    /// long description of the command
    /// </summary>
    /// <returns>text of the description</returns>
    public string LongDescription()
        => _config.GetValue<string>($"Commands:{ClassNameToCommandName()}:LongDesc")!
        ?? _texts._("CommandLongHelpNotFound", ClassNameToCommandName())!;

    public string ClassNameToCommandName()
        => GetType().Name[0..^7].ToLower();

    public static string ClassNameToCommandName(string className)
        => className[0..^7].ToLower();

    protected void CheckMaxArgs(string[] args, int maxArgCount)
    {
        if (args.Length > maxArgCount)
            throw new ArgumentException(_texts._("TooManyArguments", maxArgCount));
    }

    protected void CheckMinArgs(string[] args, int minArgCount)
    {
        if (args.Length > minArgCount)
            throw new ArgumentException(_texts._("NotEnoughArguments", minArgCount));
    }

    protected void CheckMinMaxArgs(string[] args, int minArgCount, int maxArgCount)
    {
        CheckMaxArgs(args, maxArgCount);
        CheckMinArgs(args, minArgCount);
    }
}

