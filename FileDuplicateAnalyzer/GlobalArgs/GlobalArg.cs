using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileDuplicateAnalyzer.GlobalArgs;

internal abstract class GlobalArg : Arg
{
    protected GlobalArg(
        string name,
        IConfiguration config,
        Texts texts,
        int parametersCount = 0) : base(name, config, texts, parametersCount)
    {
    }
}

