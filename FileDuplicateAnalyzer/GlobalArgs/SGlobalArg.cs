using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileDuplicateAnalyzer.GlobalArgs;

internal class SGlobalArg : GlobalArg
{
    public SGlobalArg(
        IConfiguration config,
        Texts texts) : base("s", config, texts)
    {
    }
}

