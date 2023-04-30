namespace FileUtils.Net.SharedModels.ScanDb;

[Serializable]
class FileItem : FileSystemItem
{
    public FileItem(
        string name,
        string path,
        long size,
        long? hash = null)
        : base(name, path)
    {
        Size = size;
        Hash = hash;
    }

    public long? Hash { get; set; }

    public long Size { get; set; }
}

