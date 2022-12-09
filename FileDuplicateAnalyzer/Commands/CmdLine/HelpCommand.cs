using FileDuplicateAnalyzer.Services.CmdLine;
using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace FileDuplicateAnalyzer.Commands.CmdLine;

/// <summary>
/// command line help
/// </summary>
internal sealed class HelpCommand : Command
{
    private readonly CommandsSet _commandsSet;
    private readonly IServiceProvider _serviceProvider;

    public HelpCommand(
        CommandsSet commands,
        IOutput output,
        Texts texts,
        IServiceProvider serviceProvider) : base(output, texts)
    {
        _serviceProvider = serviceProvider;
        _commandsSet = commands;
    }

    public override string ShortDescription() => "list commands or returns help about a command";

    public override string LongDescription() => $"help: list all commands{Environment.NewLine}help commandName: help about the command with name commandName";

    public override int Run(string[] args)
    {
        CheckMaxArgs(args, 1);

        if (args.Length == 0)
        {
            foreach (KeyValuePair<string, Type> kvp in _commandsSet.Commands)
            {
                Type commandType = _commandsSet.Get(kvp.Key);
                Command? command = (Command)_serviceProvider.GetRequiredService(commandType);
                _out.WriteLine(kvp.Key + " : " + command.ShortDescription());
            }
        }
        else
        {
            Type commandType = _commandsSet.Get(args[0]);
            Command? command = (Command)_serviceProvider.GetRequiredService(commandType);
            _out.WriteLine(command.LongDescription());
        }

        return Globals.ExitOk;
    }
}
