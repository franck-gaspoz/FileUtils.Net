using System.Reflection;

using FileUtils.Net.Commands;
using FileUtils.Net.Extensions;
using FileUtils.Net.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace FileUtils.Net.Services.CmdLine;

sealed class CommandsSet
{
    readonly Texts _texts;
    readonly IServiceProvider _serviceProvider;

    public CommandsSet(
        Texts texts,
        IServiceProvider serviceProvider)
    {
        _texts = texts;
        _serviceProvider = serviceProvider;

        foreach (var classType in GetCommandTypes())
        {
            Add(
                Command.ClassNameToCommandName(classType.Name),
                classType);
        }
    }

    public static IEnumerable<Type> GetCommandTypes()
        => Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.InheritsFrom(typeof(Command)));

    readonly Dictionary<string, Type> _commands = new();

    public IReadOnlyDictionary<string, Type> Commands
        => _commands;

    void Add(
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
    /// retourne une instance de commande à partir du nom de la commande
    /// </summary>
    /// <param name="name">nom de la commande</param>
    /// <returns>commande</returns>
    /// <exception cref="ArgumentException">unknown command</exception>
    public Command GetCommand(string name)
            => (Command)_serviceProvider
                .GetRequiredService(
                    GetType(name));
}

