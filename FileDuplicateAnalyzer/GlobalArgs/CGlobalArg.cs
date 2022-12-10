using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileDuplicateAnalyzer.GlobalArgs;

internal sealed class CGlobalArg : GlobalArg
{
    public FileInfo FileInfo { get; private set; }

    public CGlobalArg(
        IConfiguration config,
        Texts texts) : base("c", config, texts, 1)
    {
    }

    protected override void Initialize()
    {

    }
}

