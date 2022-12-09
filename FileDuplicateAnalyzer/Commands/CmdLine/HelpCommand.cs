using System.Reflection;

using FileDuplicateAnalyzer.Services.CmdLine;
using FileDuplicateAnalyzer.Services.IO;
using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;
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
        IConfiguration config,
        CommandsSet commands,
        IOutput output,
        Texts texts,
        IServiceProvider serviceProvider) : base(config, output, texts)
    {
        _serviceProvider = serviceProvider;
        _commandsSet = commands;
    }

    public override int Run(string[] args)
    {
        CheckMaxArgs(args, 1);

        _out.WriteLine(_config.GetValue<string>("App:Title")!
            + $" ({Assembly.GetExecutingAssembly().GetName().Version})");
        _out.WriteLine("culture: " + Thread.CurrentThread.CurrentCulture.Name);
        _out.WriteLine();

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
