using FileUtils.Net.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileUtils.Net.GlobalArgs;

internal class SGlobalArg : GlobalArg
{
    public SGlobalArg(
        IConfiguration config,
        Texts texts) : base("s", config, texts)
    {
    }
}

