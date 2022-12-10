using System.Diagnostics.CodeAnalysis;

using FileDuplicateAnalyzer.GlobalArgs;

namespace FileDuplicateAnalyzer.Services.CmdLine;

internal sealed class SettedGlobalArgsSet
{
    protected readonly Dictionary<string, Arg> _args = new();

    public IReadOnlyDictionary<string, Arg> Args
        => _args;

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

