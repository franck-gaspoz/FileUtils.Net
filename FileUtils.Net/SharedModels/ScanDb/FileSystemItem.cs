namespace FileUtils.Net.SharedModels.ScanDb;

[Serializable]
abstract class FileSystemItem
{
    public string Name { get; private set; }

    public string Path { get; private set; }

    public string FullName => Path + Name;

    protected FileSystemItem(string name, string path)
    {
        Name = name;
        Path = path;
    }
}
