using FileDuplicateAnalyzer.Services.CmdLine;
using FileDuplicateAnalyzer.Services.IO;

namespace FileDuplicateAnalyzer.Commands.CmdLine;

/// <summary>
/// command line help
/// </summary>
internal class HelpCommand : Command
{
    private readonly CommandsSet _commandsSet;
    private readonly IOutput _out;

    public HelpCommand(
        CommandsSet commands,
        IOutput ouput)
    {
        _commandsSet = commands;
        _out = ouput;
    }

    public override int Run(string[] args)
    {
        foreach (KeyValuePair<string, Type> kvp in _commandsSet.Commands)
        {
            _out.WriteLine(kvp.Key);
        }
        return Globals.ExitOk;
    }
}
