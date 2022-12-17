using System.Reflection;

using AnsiVtConsole.NetCore;

using FileDuplicateAnalyzer.GlobalArgs;
using FileDuplicateAnalyzer.Services.CmdLine;

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
        IAnsiVtConsole console,
        Texts texts,
        IServiceProvider serviceProvider) : base(config, console, texts)
    {
        _globalArgsSet = globalArgsSet;
        _serviceProvider = serviceProvider;
        _commandsSet = commands;
    }

    private const string CyanBold = "(bon,f=cyan)";
    private const string YellowUnderlineBold = "(bon,uon,f=yellow)";
    private const string Green = "(bon,f=green)";

    private void Sep() => _console.Out.WriteLine(CyanBold + "".PadLeft(50, '-'));

    public override int Run(string[] args)
    {
        CheckMaxArgs(args, 1);

        Sep();
        var date =
            DateOnly.ParseExact(
            _config.GetValue<string>("App:ReleaseDate")!,
            Globals.SettingsDateFormat,
            null);
        _console.Out.WriteLine(CyanBold + _config.GetValue<string>("App:Title")!
            + $" ({Assembly.GetExecutingAssembly().GetName().Version} {date})");
        Sep();

        if (args.Length == 0)
        {
            _console.Out.WriteLine(_texts._("GlobalSyntax"));
            _console.Out.WriteLine();

            _console.Out.WriteLine(YellowUnderlineBold + _texts._("Commands"));
            foreach (var kvp in _commandsSet.Commands)
            {
                var command = (Command)_serviceProvider.GetRequiredService(kvp.Value);
                _console.Out.WriteLine(Green + kvp.Key + "(tdoff) : " + command.ShortDescription());
            }
            _console.Out.WriteLine();
            _console.Out.WriteLine(YellowUnderlineBold + _texts._("GlobalArgs"));
            foreach (var kvp in _globalArgsSet.Args)
            {
                var globalArg = (GlobalArg)_serviceProvider.GetRequiredService(kvp.Value);
                _console.Out.WriteLine(Green + globalArg.Prefix + kvp.Key + "(tdoff) : " + globalArg.Description());
            }
        }
        else
        {
            var commandType = _commandsSet.Get(args[0]);
            var command = (Command)_serviceProvider.GetRequiredService(commandType);
            _console.Out.WriteLine(command.LongDescription());
        }
        _console.Out.WriteLine();
        _console.Out.WriteLine(_texts._("CurrentCulture", Thread.CurrentThread.CurrentCulture.Name));
        Sep();

        return Globals.ExitOk;
    }
}
