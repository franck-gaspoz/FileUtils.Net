namespace FileUtils.Net.SharedModels.ScanDb;

[Serializable]
class FodlerItem : FileSystemItem
{
    public FodlerItem(
        string name,
        string path) : base(name, path)
    {
    }
}

