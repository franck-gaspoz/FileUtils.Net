﻿
using AnsiVtConsole.NetCore;

using FileUtils.Net.Extensions;
using FileUtils.Net.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileUtils.Net.Commands;

/// <summary>
/// abstract command
/// </summary>
public abstract class Command
{
    /// <summary>
    /// console service
    /// </summary>
    protected readonly IAnsiVtConsole Console;

    /// <summary>
    /// texts service
    /// </summary>
    protected readonly Texts Texts;

    /// <summary>
    /// app config
    /// </summary>
    protected readonly IConfiguration Config;

    /// <summary>
    ///  min arg count
    /// </summary>
    protected readonly int? MinArgCount;

    /// <summary>
    /// max arg count
    /// </summary>
    protected readonly int? MaxArgCount;

    /// <summary>
    /// construit une instance de commande
    /// </summary>
    /// <param name="config">app config</param>
    /// <param name="console">console service</param>
    /// <param name="texts">texts service</param>
    /// <param name="minArgCount">command minimum args count</param>
    /// <param name="maxArgCount">command maximum args count</param>
    public Command(
        IConfiguration config,
        IAnsiVtConsole console,
        Texts texts,
        int minArgCount,
        int maxArgCount)
    {
        Config = config;
        Texts = texts;
        Console = console;
        MinArgCount = minArgCount;
        MaxArgCount = maxArgCount;
    }

    /// <summary>
    /// exec command
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>return code</returns>
    public int Run(string[] args) => Execute(args);

    /// <summary>
    /// command run body to be implemented by subclasses
    /// </summary>
    /// <param name="args">args</param>
    /// <returns>return code</returns>
    /// <exception cref="NotImplementedException">not implemented</exception>
    protected abstract int Execute(string[] args);

    /// <summary>
    /// short description of the command
    /// </summary>
    /// <returns>text of the description</returns>
    public string ShortDescription()
        => Config.GetValue<string>($"Commands:{ClassNameToCommandName()}:ShortDesc")
        ?? Texts._("CommandShortHelpNotFound", ClassNameToCommandName())!;

    /// <summary>
    /// long description of the command
    /// </summary>
    /// <returns>text of the description</returns>
    public string LongDescription()
        => Config.GetValue<string>($"Commands:{ClassNameToCommandName()}:LongDesc")!
        ?? Texts._("CommandLongHelpNotFound", ClassNameToCommandName())!;

    /// <summary>
    /// returns the command name from this class type
    /// </summary>
    /// <returns>command name</returns>
    public string ClassNameToCommandName()
        => ClassNameToCommandName(GetType().Name);

    /// <summary>
    /// transforms a class type name to a commande name
    /// </summary>
    /// <param name="className">command type name</param>
    /// <returns>command name</returns>
    public static string ClassNameToCommandName(string className)
        => className[0..^7].ToLower();

    /// <summary>
    /// transforms a commande name to a class type name
    /// </summary>
    /// <param name="name">command name</param>
    /// <returns>name of command type</returns>
    public static string CommandNameToClassType(string name)
        => name.ToFirstUpper() + typeof(Command).Name;

    /// <summary>
    /// check args count doesn't exceed max arg count allowed
    /// </summary>
    /// <param name="args">args</param>
    /// <exception cref="ArgumentException">too many arguments</exception>
    protected void CheckMaxArgs(string[] args)
    {
        if (MaxArgCount is null)
            return;
        if (args.Length > MaxArgCount)
            throw new ArgumentException(Texts._("TooManyArguments", MaxArgCount));
    }

    /// <summary>
    /// check args count is not lower than min allowed arg count
    /// </summary>
    /// <param name="args">args</param>
    /// <exception cref="ArgumentException">not enough arguments</exception>
    protected void CheckMinArgs(string[] args)
    {
        if (MinArgCount is null)
            return;
        if (args.Length > MinArgCount)
            throw new ArgumentException(Texts._("NotEnoughArguments", MinArgCount));
    }

    /// <summary>
    /// check args count doesn't exceed max arg count allowed and args count is not lower than min allowed arg count
    /// </summary>
    /// <param name="args">args</param>
    /// <exception cref="ArgumentException">too many arguments</exception>
    /// <exception cref="ArgumentException">not enough arguments</exception>
    protected void CheckMinMaxArgs(string[] args)
    {
        CheckMaxArgs(args);
        CheckMinArgs(args);
    }
}

