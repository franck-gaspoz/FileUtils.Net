namespace FileDuplicateAnalyzer.SharedModels;

[Serializable]
internal sealed class FodlerItem : FileSystemItem
{
    public FodlerItem(
        string name,
        string path) : base(name, path)
    {
    }
}

