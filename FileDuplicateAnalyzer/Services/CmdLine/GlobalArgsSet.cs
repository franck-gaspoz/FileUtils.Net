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
            arg = serviceProvider.GetRequiredService<Arg>();
            return true;
        }
        return false;
    }

    public Dictionary<string, Arg> Parse(
        IServiceProvider serviceProvider,
        List<string> strs)
    {
        Dictionary<string, Arg> res = new();
        Stack<string>? stack = new(strs);
        while (stack.Count > 0)
        {
            var str = stack.Pop();
            if (TryGet(
                serviceProvider,
                str,
                out var arg))
            {
                res.Add(str, arg);
                //arg.ParseParameters()
            }
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

