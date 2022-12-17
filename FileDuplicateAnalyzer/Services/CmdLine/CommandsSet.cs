using FileDuplicateAnalyzer.Commands;
using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace FileDuplicateAnalyzer.Services.CmdLine;

internal sealed class CommandsSet
{
    private readonly Texts _texts;
    private readonly IServiceProvider _serviceProvider;

    public CommandsSet(Texts texts, IServiceProvider serviceProvider)
        => (_texts, _serviceProvider) = (texts, serviceProvider);

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
    public Type GetType(string name)
        => !_commands.TryGetValue(name, out var commandType)
            ? throw new ArgumentException(_texts._("UnknownCommand", name))
            : commandType;

    /// <summary>
    /// retourne une instance de commande à partir du type de la commande
    /// </summary>
    /// <param name="typeName">type de la commande</param>
    /// <returns>commande</returns>
    /// <exception cref="ArgumentException">unknown command</exception>
    public Command GetCommandFromTypeName(string typeName)
        => (Command)_serviceProvider
            .GetRequiredService(
                GetType(typeName));

    /// <summary>
    /// retourne une instance de commande à partir du nom de la commande
    /// </summary>
    /// <param name="name">nom de la commande</param>
    /// <returns>commande</returns>
    /// <exception cref="ArgumentException">unknown command</exception>
    public Command GetCommand(string name)
        => GetCommandFromTypeName(
            Command.CommandNameToClassType(name));
}

