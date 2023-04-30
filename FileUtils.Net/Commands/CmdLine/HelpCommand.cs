using System.Reflection;

using AnsiVtConsole.NetCore;

using FileUtils.Net.GlobalArgs;
using FileUtils.Net.Services.CmdLine;

using FileUtils.Net.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileUtils.Net.Commands.CmdLine;

/// <summary>
/// command line help
/// </summary>
class HelpCommand : Command
{
    readonly CommandsSet _commandsSet;
    readonly GlobalArgsSet _globalArgsSet;
    readonly IServiceProvider _serviceProvider;

    public HelpCommand(
        IConfiguration config,
        CommandsSet commands,
        GlobalArgsSet globalArgsSet,
        IAnsiVtConsole console,
        Texts texts,
        IServiceProvider serviceProvider) :
            base(config, console, texts, 0, 1)
    {
        _globalArgsSet = globalArgsSet;
        _serviceProvider = serviceProvider;
        _commandsSet = commands;
    }

    const string TitleColor = "(bon,f=cyan)";
    const string SectionTitleColor = "(uon,f=yellow,bon)";
    const string CommandNameColor = "(bon,f=green)";
    const string ArgNameColor = "(f=darkyellow)";
    const string ArgValueColor = "(bon,f=cyan)";
    const string StOff = "(tdoff)";

    void Sep() => Console.Out.WriteLine(TitleColor + "".PadLeft(50, '-'));

    protected override int Execute(string[] args)
    {
        OutputAppTitle();

        if (args.Length == 0)
        {
            OutputSectionTitle(Texts._("Syntax"));
            DumpCommandSyntax(Texts._("GlobalSyntax"));
            Console.Out.WriteLine();
            Console.Out.WriteLine();

            OutputSectionTitle(Texts._("Commands"));
            foreach (var kvp in _commandsSet.Commands)
            {
                var command = (Command)_serviceProvider.GetRequiredService(kvp.Value);
                Console.Out.WriteLine(CommandNameColor + kvp.Key + $"{StOff} : " + command.ShortDescription());
            }
            Console.Out.WriteLine();
            OutputSectionTitle(Texts._("GlobalArgs"));
            foreach (var kvp in _globalArgsSet.Args)
            {
                var globalArg = (GlobalArg)_serviceProvider.GetRequiredService(kvp.Value);
                Console.Out.WriteLine(ArgNameColor + globalArg.Prefix + kvp.Key + $"{StOff} : " + globalArg.Description());
            }
        }
        else
        {
            var command = _commandsSet.GetCommand(args[0]);
            DumpLongDescription(command.LongDescription());
        }
        Console.Out.WriteLine();
        Console.Out.WriteLine(Texts._("CurrentCulture", Thread.CurrentThread.CurrentCulture.Name));
        Sep();

        return Globals.ExitOk;
    }

    void OutputAppTitle()
    {
        Sep();
        var date =
            DateOnly.ParseExact(
            Config.GetValue<string>("App:ReleaseDate")!,
            Globals.SettingsDateFormat,
            null);
        Console.Out.WriteLine(TitleColor + Config.GetValue<string>("App:Title")!
            + $" ({Assembly.GetExecutingAssembly().GetName().Version} {date})");
        Sep();
        Console.Out.WriteLine();
    }

    void OutputSectionTitle(string text)
        => Console.Out.WriteLine(SectionTitleColor + text + StOff);

    void DumpCommandSyntax(string text)
    {
        var args = text.Split(' ');
        var cmdName = StringAt(0, ref args);

        Console.Out.Write(CommandNameColor + cmdName + StOff);

        for (var i = 1; i < args.Length; i++)
        {
            var arg = args[i];
            arg = arg.StartsWith('-') ?
                ArgNameColor + arg
                : ArgValueColor + arg;

            Console.Out.Write(" " + arg);
        }

        Console.Out.Write(StOff);
    }

    void DumpLongDescription(string text)
    {
        var lines = text
            .Replace("\r", "")
            .Split('\n');

        foreach (var line in lines)
        {
            var t = line.Split(':');
            var desc = StringAt(1, ref t).Trim();
            var descExists = !string.IsNullOrWhiteSpace(desc);

            var cmdSyntax = StringAt(0, ref t).Trim();

            DumpCommandSyntax(cmdSyntax);

            if (descExists)
                Console.Out.Write(" : " + desc);
            Console.Out.WriteLine();
        }
    }

    static string StringAt(int i, ref string[] t)
        => i <= t.Length ? t[i] : "";
}
