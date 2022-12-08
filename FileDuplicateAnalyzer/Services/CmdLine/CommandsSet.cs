namespace FileDuplicateAnalyzer.Services.CmdLine;

internal class CommandsSet
{
    private readonly Dictionary<string, Type> _commands = new();

    public IReadOnlyDictionary<string, Type> Commands
        => _commands;

    public void Add(
        string name,
        Type commandType)
        => _commands.Add(name, commandType);
}

