namespace FileDuplicateAnalyzer.Services.Text;

internal sealed class Texts
{
    private readonly Dictionary<int, string> _texts = new()
    {
        { UnknownText, "unknown text: %1" },
        { MissingArguments, "missing arguments" },
        { UnknownCommand, "unknown command: %1" },
        { TooManyArguments, "too many arguments. max is %1" },
        { NotEnoughArguments, "not enough arguments. min is %1" },
    };

    private static int _counter = 0;
    public static int MissingArguments = _counter++;
    public static int UnknownCommand = _counter++;
    public static int UnknownText = _counter++;
    public static int TooManyArguments = _counter++;
    public static int NotEnoughArguments = _counter++;

    /// <summary>
    /// returns text from text id
    /// </summary>
    /// <param name="textId">text id</param>
    /// <param name="parameters">parmetrized text parameters (like string.Format)</param>
    /// <returns></returns>
    public string _(
        int textId,
        params object?[] parameters)
            => _texts.TryGetValue(textId, out string? text)
                ? string.Format(text, parameters) :
                _(UnknownCommand, textId);
}
