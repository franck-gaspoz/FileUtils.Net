using System.Collections.ObjectModel;

namespace FileDuplicateAnalyzer.Services.CmdLine;

internal sealed class CommandLineArgs
{
    public ReadOnlyCollection<string> Args { get; private set; }

    public CommandLineArgs(List<string> args)
        => Args = new ReadOnlyCollection<string>(args);
}
