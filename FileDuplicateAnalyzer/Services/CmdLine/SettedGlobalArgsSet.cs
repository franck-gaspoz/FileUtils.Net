namespace FileDuplicateAnalyzer.Services.CmdLine;

internal sealed class SettedGlobalArgsSet : GlobalArgsSet
{
    public bool Contains(Type argType)
        => _args.Values.Contains(argType);
}

