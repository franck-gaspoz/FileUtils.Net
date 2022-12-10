using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileDuplicateAnalyzer.GlobalArgs;

internal class CGlobalArg : GlobalArg
{
    public CGlobalArg(
        IConfiguration config,
        Texts texts) : base("c", config, texts, 1)
    {
    }
}

