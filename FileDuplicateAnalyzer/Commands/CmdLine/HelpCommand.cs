using System.Reflection;

using FileDuplicateAnalyzer.GlobalArgs;
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
    private readonly GlobalArgsSet _globalArgsSet;
    private readonly IServiceProvider _serviceProvider;

    public HelpCommand(
        IConfiguration config,
        CommandsSet commands,
        GlobalArgsSet globalArgsSet,
        IOutput output,
        Texts texts,
        IServiceProvider serviceProvider) : base(config, output, texts)
    {
        _globalArgsSet = globalArgsSet;
        _serviceProvider = serviceProvider;
        _commandsSet = commands;
    }

    private void Sep() => _out.WriteLine("".PadLeft(50, '-'));

    public override int Run(string[] args)
    {
        CheckMaxArgs(args, 1);

        Sep();
        var date =
            DateOnly.ParseExact(
            _config.GetValue<string>("App:ReleaseDate")!,
            "dd/MM/yyyy",
            null);
        _out.WriteLine(_config.GetValue<string>("App:Title")!
            + $" ({Assembly.GetExecutingAssembly().GetName().Version} {date})");
        _out.WriteLine("culture: " + Thread.CurrentThread.CurrentCulture.Name);
        Sep();

        if (args.Length == 0)
        {
            _out.WriteLine("commands:");
            foreach (var kvp in _commandsSet.Commands)
            {
                var command = (Command)_serviceProvider.GetRequiredService(kvp.Value);
                _out.WriteLine(kvp.Key + " : " + command.ShortDescription());
            }
            _out.WriteLine();
            _out.WriteLine("global args:");
            foreach (var kvp in _globalArgsSet.Args)
            {
                var globalArg = (GlobalArg)_serviceProvider.GetRequiredService(kvp.Value);
                _out.WriteLine(globalArg.Prefix + kvp.Key + " : " + globalArg.Description());
            }
        }
        else
        {
            var commandType = _commandsSet.Get(args[0]);
            var command = (Command)_serviceProvider.GetRequiredService(commandType);
            _out.WriteLine(command.LongDescription());
        }
        Sep();

        return Globals.ExitOk;
    }
}
