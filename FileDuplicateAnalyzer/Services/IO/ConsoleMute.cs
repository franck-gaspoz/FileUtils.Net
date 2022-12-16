using AnsiVtConsole.NetCore.Component.Console;

namespace FileDuplicateAnalyzer.Services.IO;

internal sealed class ConsoleMute : IConsole
{
    public ConsoleTextWriterWrapper? Out => null;

    public Err? Err => null;

    public Warn? Warn => null;
}

