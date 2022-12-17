using System.Reflection;

using FileDuplicateAnalyzer.Commands;
using FileDuplicateAnalyzer.GlobalArgs;

using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.DependencyInjection;

using cons = AnsiVtConsole.NetCore;

namespace FileDuplicateAnalyzer.Services.CmdLine;

internal static class IServiceCollectionExt
{
    /// <summary>
    /// add commands founded in the executing assembly as injectable dependencies
    /// </summary>
    /// <param name="services">service collection</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddCommands(
        this IServiceCollection services
        )
    {
        services.AddSingleton(
            serviceProvider =>
            {
                var commandSet = new CommandsSet(
                    serviceProvider.GetRequiredService<Texts>(),
                    serviceProvider);

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
                return commandSet;
            }
        );
        return services;
    }

    /// <summary>
    /// add global arguments founded in the executing assembly as injectable dependencies
    /// </summary>
    /// <param name="services">service collection</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddGlobalArguments(
        this IServiceCollection services)
    {
        services.AddSingleton(
            serviceProvider =>
            {
                var globalArgsSet = new GlobalArgsSet();

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

                return globalArgsSet;
            });

        return services;
    }

    public static IServiceCollection ConfigureOutput(this IServiceCollection services)
    {
        services.AddSingleton<cons.IAnsiVtConsole>(
            serviceProvider =>
            {
                var settedGlobalArgs = serviceProvider.GetRequiredService<SettedGlobalArgsSet>();
                var console = new cons.AnsiVtConsole();
                console.Out.IsMute = settedGlobalArgs.Contains<SGlobalArg>();
                return console;
            });
        return services;
    }

    /// <summary>
    /// add global arguments founded in command line arguments as injectable dependencies
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="args">command line arguments</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddSettedGlobalArguments(
        this IServiceCollection services,
        List<string> args)
    {
        services.AddSingleton<SettedGlobalArgsSet>(
            serviceProvider =>
            {
                var globalArgsSet = serviceProvider.GetRequiredService<GlobalArgsSet>();
                var settedGlobalArgsSet = new SettedGlobalArgsSet();
                foreach (var kvp in globalArgsSet!.Parse(
                    services.BuildServiceProvider(),
                    args))
                {
                    settedGlobalArgsSet.Add(kvp.Value);
                }
                services.AddSingleton(settedGlobalArgsSet);
                return settedGlobalArgsSet;
            });

        return services;
    }
}
