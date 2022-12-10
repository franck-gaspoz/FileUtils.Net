using System.Diagnostics.CodeAnalysis;

using FileDuplicateAnalyzer.GlobalArgs;

using Microsoft.Extensions.DependencyInjection;

namespace FileDuplicateAnalyzer.Services.CmdLine;

internal class GlobalArgsSet
{
    protected readonly Dictionary<string, Type> _args = new();

    public IReadOnlyDictionary<string, Type> Args
        => _args;

    public void Add(
        string name,
        Type argType)
        => _args.Add(name, argType);

    private bool TryGet(
        IServiceProvider serviceProvider,
        string str,
        [NotNullWhen(true)]
        out Arg? arg)
    {
        arg = null;
        var argName = str;
        while (argName.StartsWith('-'))
            argName = argName[1..];
        if (_args.TryGetValue(argName, out var classType))
        {
            arg = (Arg)serviceProvider.GetRequiredService(classType);
            return true;
        }
        return false;
    }

    public Dictionary<string, Arg> Parse(
        IServiceProvider serviceProvider,
        List<string> args)
    {
        Dictionary<string, Arg> res = new();
        var stackArgs = args.ToList();
        stackArgs.Reverse();
        Stack<string>? stack = new(stackArgs);
        var index = 0;
        while (stack.Count > 0)
        {
            var str = stack.Pop();
            if (TryGet(
                serviceProvider,
                str,
                out var arg))
            {
                res.Add(str, arg);
                arg.ParseParameters(args, index);
            }
            index++;
        }
        return res;
    }

    public static bool ExistsInArgList(
        Type argType,
        List<string> args) =>
            argType.InheritsFrom(typeof(GlobalArg))
            && args.Contains(
                GlobalArg.GetPrefixFromClassName(argType.Name)
                + GlobalArg.ClassNameToArgName(argType.Name));
}

