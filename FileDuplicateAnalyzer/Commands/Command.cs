using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileDuplicateAnalyzer.Commands;

internal abstract class Command
{
    protected readonly IOutput _out;
    protected readonly Texts _texts;
    protected readonly IConfiguration _config;

    public Command(
        IConfiguration config,
        IOutput ouput,
        Texts texts)
    {
        _config = config;
        _texts = texts;
        _out = ouput;
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
        => _config.GetValue<string>($"commands:{ClassNameToCommandName}:shortDesc")
        ?? _config.GetValue<string>($"texts:commandShortHelpNotFound")!;

    /// <summary>
    /// long description of the command
    /// </summary>
    /// <returns>text of the description</returns>
    public string LongDescription()
        => _config.GetValue<string>($"commands:{ClassNameToCommandName}:longDesc")!
        ?? _config.GetValue<string>($"texts:commandLongHelpNotFound")!;

    public string ClassNameToCommandName
        => GetType().Name[0..^7].ToLower();

    protected void CheckMaxArgs(string[] args, int maxArgCount)
    {
        if (args.Length > maxArgCount)
            throw new ArgumentException(_texts._("tooManyArguments", maxArgCount));
    }

    protected void CheckMinArgs(string[] args, int minArgCount)
    {
        if (args.Length > minArgCount)
            throw new ArgumentException(_texts._("notEnoughArguments", minArgCount));
    }

    protected void CheckMinMaxArgs(string[] args, int minArgCount, int maxArgCount)
    {
        CheckMaxArgs(args, maxArgCount);
        CheckMinArgs(args, minArgCount);
    }
}

