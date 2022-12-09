using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;

using static FileDuplicateAnalyzer.Services.CmdLine.Globals;

namespace FileDuplicateAnalyzer.GlobalArgs;

internal abstract class GlobalArg
{
    protected readonly IConfiguration _config;
    protected readonly Texts _texts;

    public string Name { get; private set; }

    public GlobalArg(
        string name,
        IConfiguration config,
        Texts texts)
    {
        Name = name;
        _config = config;
        _texts = texts;
    }

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

    public string Description() =>
        _config.GetValue<string>("GlobalArgs:" + Name)!
        ?? _texts._("GlobalArgHelpNotFound", Name)!;
}

