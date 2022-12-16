using AnsiVtConsole.NetCore.Component.Console;

namespace FileDuplicateAnalyzer.Services.IO;

internal interface IConsole
{
    public ConsoleTextWriterWrapper? Out { get; }

    public Err? Err { get; }

    public Warn? Warn { get; }
}
