namespace FileDuplicateAnalyzer.Services.IO;

internal sealed class Output : IOutput
{
    public void WriteLine(string? s = null) => Console.WriteLine(s ?? string.Empty);
}

