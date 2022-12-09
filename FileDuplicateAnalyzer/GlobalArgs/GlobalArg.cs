using static FileDuplicateAnalyzer.Services.CmdLine.Globals;

namespace FileDuplicateAnalyzer.GlobalArgs;

internal abstract class GlobalArg
{
    public string Name { get; private set; }

    public GlobalArg(string name) => Name = name;

    public static string ClassNameToArgName(string name)
        => name[0..^9]
            .ToLower();

    public static string GetPrefixFromClassName(string name)
    {
        string? argName = ClassNameToArgName(name);
        return argName.Length == 1
            ? ShortArgNamePrefix
            : LongArgNamePrefix;
    }

    public static string GetPrefixFromArgName(string name)
    => name.Length == 1
    ? ShortArgNamePrefix
    : LongArgNamePrefix;
}

