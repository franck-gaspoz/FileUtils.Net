namespace FileDuplicateAnalyzer.Services.Text;

internal sealed class Texts
{
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
    private static Texts _messages;
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

    public static Texts Instance => _messages ??= new Texts();

    private readonly Dictionary<int, string> _texts = new()
    {
        { UnknownText, "unknown text: %1" },
        { MissingArguments, "missing arguments" },
        { UnknownCommand, "unknown command: %1" },
    };

    private static int _counter = 0;
    public static int MissingArguments = _counter++;
    public static int UnknownCommand = _counter++;
    public static int UnknownText = _counter++;

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
