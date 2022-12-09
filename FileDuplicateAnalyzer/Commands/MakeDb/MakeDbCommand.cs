using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

namespace FileDuplicateAnalyzer.Commands.MakeDb;

/// <summary>
/// build a file db
/// </summary>
internal sealed class MakeDbCommand : Command
{
    public MakeDbCommand(
        IOutput output,
        Texts texts) : base(output, texts)
    {
    }

    public override int Run(string[] args)

        => throw new NotImplementedException();
}
