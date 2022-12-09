using FileDuplicateAnalyzer.Services.Text;

namespace FileDuplicateAnalyzer.Services.CmdLine;

internal sealed class CommandsSet
{
    private readonly Texts _texts;

    public CommandsSet(Texts texts)
        => _texts = texts;

    private readonly Dictionary<string, Type> _commands = new();

    public IReadOnlyDictionary<string, Type> Commands
        => _commands;

    public void Add(
        string name,
        Type commandType)
        => _commands.Add(name, commandType);

    /// <summary>
    /// returns type of a command
    /// </summary>
    /// <param name="name">name of a command</param>
    /// <exception cref="ArgumentException">unknown command</exception>
    /// <returns></returns>
    public Type Get(
        string name) => !_commands.TryGetValue(name, out Type? commandType)
            ? throw new ArgumentException(_texts._("unknownCommand", name))
            : commandType;
}

