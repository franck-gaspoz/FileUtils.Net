﻿using System.Reflection;

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
        _out.WriteLine(_config.GetValue<string>("App:Title")!
            + $" ({Assembly.GetExecutingAssembly().GetName().Version})");
        _out.WriteLine("culture: " + Thread.CurrentThread.CurrentCulture.Name);
        Sep();

        if (args.Length == 0)
        {
            _out.WriteLine("commands:");
            foreach (KeyValuePair<string, Type> kvp in _commandsSet.Commands)
            {
                Command command = (Command)_serviceProvider.GetRequiredService(kvp.Value);
                _out.WriteLine(kvp.Key + " : " + command.ShortDescription());
            }
            _out.WriteLine();
            _out.WriteLine("global args:");
            foreach (KeyValuePair<string, Type> kvp in _globalArgsSet.Args)
            {
                GlobalArg? globalArg = (GlobalArg)_serviceProvider.GetRequiredService(kvp.Value);
                _out.WriteLine(kvp.Key + " : " + globalArg.Description());
            }
        }
        else
        {
            Type commandType = _commandsSet.Get(args[0]);
            Command? command = (Command)_serviceProvider.GetRequiredService(commandType);
            _out.WriteLine(command.LongDescription());
        }
        Sep();

        return Globals.ExitOk;
    }
}
