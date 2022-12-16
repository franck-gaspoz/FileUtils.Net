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
        IConsole console,
        Texts texts,
        IServiceProvider serviceProvider) : base(config, console, texts)
    {
        _globalArgsSet = globalArgsSet;
        _serviceProvider = serviceProvider;
        _commandsSet = commands;
    }

    private void Sep() => _console.Out?.WriteLn("(bon,f=cyan)" + "".PadLeft(50, '-'));

    public override int Run(string[] args)
    {
        CheckMaxArgs(args, 1);

        Sep();
        var date =
            DateOnly.ParseExact(
            _config.GetValue<string>("App:ReleaseDate")!,
            Globals.SettingsDateFormat,
            null);
        _console.Out?.WriteLn("(bon,f=cyan)" + _config.GetValue<string>("App:Title")!
            + $" ({Assembly.GetExecutingAssembly().GetName().Version} {date})");
        Sep();

        if (args.Length == 0)
        {
            _console.Out?.WriteLn(_texts._("GlobalSyntax"));
            _console.Out?.WriteLn();

            _console.Out?.WriteLn("(bon,f=yellow)" + _texts._("Commands"));
            foreach (var kvp in _commandsSet.Commands)
            {
                var command = (Command)_serviceProvider.GetRequiredService(kvp.Value);
                _console.Out?.WriteLn("(f=darkyellow)" + kvp.Key + "(rsf) : " + command.ShortDescription());
            }
            _console.Out?.WriteLn();
            _console.Out?.WriteLn("(bon,f=yellow)" + _texts._("GlobalArgs"));
            foreach (var kvp in _globalArgsSet.Args)
            {
                var globalArg = (GlobalArg)_serviceProvider.GetRequiredService(kvp.Value);
                _console.Out?.WriteLn("(f=darkyellow)" + globalArg.Prefix + kvp.Key + "(rsf) : " + globalArg.Description());
            }
        }
        else
        {
            var commandType = _commandsSet.Get(args[0]);
            var command = (Command)_serviceProvider.GetRequiredService(commandType);
            _console.Out?.WriteLn(command.LongDescription());
        }
        _console.Out?.WriteLn();
        _console.Out?.WriteLn(_texts._("CurrentCulture", Thread.CurrentThread.CurrentCulture.Name));
        Sep();

        return Globals.ExitOk;
    }
}
