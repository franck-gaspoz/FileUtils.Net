using AnsiVtConsole.NetCore;

using FileUtils.Net.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileUtils.Net.Commands.MakeDb;

/// <summary>
/// build a file db
/// </summary>
sealed class MakeDbCommand : Command
{
    public MakeDbCommand(
        IConfiguration config,
        IAnsiVtConsole output,
        Texts texts) :
            base(config, output, texts, 0, 0)
    {
    }

    protected override int Execute(string[] args)
        => throw new NotImplementedException();
}
