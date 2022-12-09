using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

namespace FileDuplicateAnalyzer.Commands;

internal abstract class Command
{
    protected readonly IOutput _out;
    protected readonly Texts _texts;

    public Command(
        IOutput ouput,
        Texts texts)
    {
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
    public abstract string ShortDescription();

    /// <summary>
    /// long description of the command
    /// </summary>
    /// <returns>text of the description</returns>
    public abstract string LongDescription();

    protected void CheckMaxArgs(string[] args, int maxArgCount)
    {
        if (args.Length > maxArgCount)
            throw new ArgumentException(_texts._(Texts.TooManyArguments, maxArgCount));
    }

    protected void CheckMinArgs(string[] args, int minArgCount)
    {
        if (args.Length > minArgCount)
            throw new ArgumentException(_texts._(Texts.NotEnoughArguments, minArgCount));
    }

    protected void CheckMinMaxArgs(string[] args, int minArgCount, int maxArgCount)
    {
        CheckMaxArgs(args, maxArgCount);
        CheckMinArgs(args, minArgCount);
    }
}

