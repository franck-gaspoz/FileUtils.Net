namespace FileDuplicateAnalyzer.Services.IO;

internal sealed class Output : IOutput
{
    public void WriteLine(string s) => Console.WriteLine(s);
}

