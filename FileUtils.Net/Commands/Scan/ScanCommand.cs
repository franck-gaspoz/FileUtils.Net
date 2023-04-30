
using AnsiVtConsole.NetCore;

using FileUtils.Net.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileUtils.Net.Commands.Scan;

/// <summary>
/// scan files
/// </summary>
class ScanCommand : Command
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
