using FileDuplicateAnalyzer.GlobalArgs;

namespace FileDuplicateAnalyzer.Services.CmdLine;

internal class GlobalArgsSet
{
    protected readonly Dictionary<string, Type> _args = new();

    public IReadOnlyDictionary<string, Type> Args
        => _args;

    public void Add(
        string name,
        Type argType)
        => _args.Add(name, argType);

    public static bool ExistsInArgList(
        Type argType,
        List<string> args) =>
            argType.InheritsFrom(typeof(GlobalArg))
            && args.Contains(
                GlobalArg.GetPrefixFromClassName(argType.Name)
                + GlobalArg.ClassNameToArgName(argType.Name));
}

