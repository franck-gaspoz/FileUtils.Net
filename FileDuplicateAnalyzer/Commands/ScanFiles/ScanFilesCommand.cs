using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

namespace FileDuplicateAnalyzer.Commands.ScanFiles;

/// <summary>
/// scan files
/// </summary>
internal sealed class ScanFilesCommand : Command
{
    public ScanFilesCommand(
        IOutput output,
        Texts texts) : base(output, texts)
    {
    }

    public override int Run(string[] args)
        => throw new NotImplementedException();

    public override string ShortDescription() => "scan files";

    public override string LongDescription() => "scanfiles : scan files";
}
