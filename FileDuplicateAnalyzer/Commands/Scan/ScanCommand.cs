using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileDuplicateAnalyzer.Commands.Scan;

/// <summary>
/// scan files
/// </summary>
internal sealed class ScanCommand : Command
{
    public ScanCommand(
        IConfiguration config,
        IOutput output,
        Texts texts) : base(config, output, texts)
    {
    }

    public override int Run(string[] args)
        => throw new NotImplementedException();
}
