﻿
using FileUtils.Net.GlobalArgs;

using Microsoft.Extensions.DependencyInjection;

using cons = AnsiVtConsole.NetCore;

namespace FileUtils.Net.Services.CmdLine;

static class IServiceCollectionExt
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
        services.AddSingleton<CommandsSet>();

        foreach (var classType in CommandsSet.GetCommandTypes())
            services.AddTransient(classType);

        return services;
    }

    /// <summary>
    /// add command line arguments as a injectable dependcy
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="args">command line args</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddCommandLineArgs(
        this IServiceCollection services,
        List<string> args)
    {
        services.AddSingleton(
            serviceProvider =>
                new CommandLineArgs(args));

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
        services.AddSingleton<GlobalArgsSet>();

        foreach (var classType in GlobalArgsSet.GetGlobalArgTypes())
            services.AddSingleton(classType);

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
    /// <returns>service collection</returns>
    public static IServiceCollection AddSettedGlobalArguments(
        this IServiceCollection services)
    {
        services.AddSingleton<SettedGlobalArgsSet>();

        return services;
    }
}
