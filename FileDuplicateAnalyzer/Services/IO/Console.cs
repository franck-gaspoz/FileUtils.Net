using AnsiVtConsole.NetCore;
using AnsiVtConsole.NetCore.Component.Console;

namespace FileDuplicateAnalyzer.Services.IO;

internal sealed class Console : IConsole
{
    private readonly IAnsiVtConsole _ansiVtConsole;

    public Console(IAnsiVtConsole ansiVtConsole)
        => _ansiVtConsole = ansiVtConsole;

    public ConsoleTextWriterWrapper? Out => _ansiVtConsole.Out;

    public Err? Err => _ansiVtConsole.Err;

    public Warn? Warn => _ansiVtConsole.Warn;
}

