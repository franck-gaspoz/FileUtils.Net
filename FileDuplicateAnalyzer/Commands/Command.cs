namespace FileDuplicateAnalyzer.Commands;

internal abstract class Command
{
    /// <summary>
    /// exec command
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>return code</returns>
    public abstract int Run(string[] args);
}

