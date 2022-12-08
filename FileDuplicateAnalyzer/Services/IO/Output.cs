namespace FileDuplicateAnalyzer.Services.IO;

internal class Output : IOutput
{
    public void WriteLine(string s)
        => System.Console.WriteLine(s);
}

