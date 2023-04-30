using FileUtils.Net.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileUtils.Net.GlobalArgs;

abstract class GlobalArg : Arg
{
    protected GlobalArg(
        string name,
        IConfiguration config,
        Texts texts,
        int parametersCount = 0) : base(name, config, texts, parametersCount)
    {
    }
}

