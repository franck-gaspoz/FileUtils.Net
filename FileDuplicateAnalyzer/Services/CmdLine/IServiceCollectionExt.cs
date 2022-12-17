using System.Reflection;

using FileDuplicateAnalyzer.Commands;
using FileDuplicateAnalyzer.GlobalArgs;

using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.DependencyInjection;

using cons = AnsiVtConsole.NetCore;

namespace FileDuplicateAnalyzer.Services.CmdLine;

internal static class IServiceCollectionExt
{
    public static IServiceCollection AddCommands(
        this IServiceCollection services,
        out CommandsSet commandSet
        )
    {
        commandSet = new CommandsSet(services.BuildServiceProvider().GetRequiredService<Texts>());

        foreach (var classType in
            Assembly
                .GetExecutingAssembly()
                .GetTypes())
        {
            if (classType.InheritsFrom(typeof(Command)))
            {
                services.AddSingleton(classType);

                commandSet.Add(
                    Command.ClassNameToCommandName(classType.Name),
                    classType);
            }
        }

        services.AddSingleton(commandSet);
        return services;
    }

    public static IServiceCollection AddArguments(
        this IServiceCollection services,
        out GlobalArgsSet globalArgsSet)
    {
        globalArgsSet = new GlobalArgsSet();

        foreach (var classType in
            Assembly
                .GetExecutingAssembly()
                .GetTypes())
        {
            if (classType.InheritsFrom(typeof(GlobalArg)))
            {
                var argName = GlobalArg.ClassNameToArgName(
                    classType.Name);

                globalArgsSet.Add(argName, classType);
                services.AddTransient(classType);
            }
        }

        services.AddSingleton(globalArgsSet);
        return services;
    }

    public static IServiceCollection ConfigureOutput(this IServiceCollection services)
    {
        services.AddSingleton<cons.IAnsiVtConsole, cons.AnsiVtConsole>();
        var serviceProvider = services.BuildServiceProvider();
        var settedGlobalArgs = serviceProvider.GetRequiredService<SettedGlobalArgsSet>();
        var console = serviceProvider.GetRequiredService<cons.IAnsiVtConsole>();
        console.Out.IsMute = settedGlobalArgs.Contains<SGlobalArg>();
        return services;
    }

    public static IServiceCollection ParseGlobalArguments(
        this IServiceCollection services,
        List<string> args,
        GlobalArgsSet globalArgsSet,
        out SettedGlobalArgsSet settedGlobalArgsSet)
    {
        settedGlobalArgsSet = new();
        foreach (var kvp in globalArgsSet!.Parse(
            services.BuildServiceProvider(),
            args))
        {
            settedGlobalArgsSet.Add(kvp.Value);
        }
        services.AddSingleton(settedGlobalArgsSet);
        return services;
    }
}
