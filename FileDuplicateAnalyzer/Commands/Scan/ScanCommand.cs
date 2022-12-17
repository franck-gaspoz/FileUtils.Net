
using AnsiVtConsole.NetCore;

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
        IAnsiVtConsole output,
        Texts texts) :
            base(config, output, texts, 0, 0)
    {
    }

    protected override int Execute(string[] args)
        => throw new NotImplementedException();
}
