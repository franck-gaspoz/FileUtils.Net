﻿using System.Diagnostics.CodeAnalysis;

using FileUtils.Net.GlobalArgs;

namespace FileUtils.Net.Services.CmdLine;

class SettedGlobalArgsSet
{
    readonly Dictionary<string, Arg> _args = new();

    public IReadOnlyDictionary<string, Arg> Args
        => _args;

    public SettedGlobalArgsSet(
        CommandLineArgs commandLineArgs,
        GlobalArgsSet globalArgsSet)
    {
        foreach (var kvp in globalArgsSet.Parse(commandLineArgs))
            Add(kvp.Value);
    }

    public void Add(Arg arg)
        => _args.Add(arg.Name, arg);

    public bool TryGetByType<T>([NotNullWhen(true)] out T? arg)
        where T : Arg
    {
        arg = (T?)_args
            .Values
            .Where(x => x.GetType() == typeof(T))
            .FirstOrDefault();

        return arg != null;
    }

    public bool Contains<T>()
        where T : Arg
        => TryGetByType<T>(out _);
}

