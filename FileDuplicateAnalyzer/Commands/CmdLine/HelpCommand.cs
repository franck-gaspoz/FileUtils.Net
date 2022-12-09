using FileDuplicateAnalyzer.Services.CmdLine;
using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

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

    public override int Run(string[] args)
    {
        CheckMaxArgs(args, 1);

        if (args.Length == 0)
        {
            foreach (KeyValuePair<string, Type> kvp in _commandsSet.Commands)
            {
                _out.WriteLine(kvp.Key);
            }
        }
        else
        {
            Type command = _commandsSet.Get(args[0]);
            _out.WriteLine(command.Name);
        }

        return Globals.ExitOk;
    }
}
