namespace FileDuplicateAnalyzer.Services.CmdLine;

internal static class Globals
{
    public const int ExitOk = 0;
    public const int ExitFail = -1;

    public const string ShortArgNamePrefix = "-";
    public const string LongArgNamePrefix = "--";

    public const string ConfigFilePrefix = "config/appSettings.";
    public const string ConfigFilePostfix = ".json";
    public const string ConfigFilePath = "config/appSettings.json";
}

