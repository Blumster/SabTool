namespace SabTool.Data.Misc;

public sealed class LooseFile
{
    public Crc Crc { get; set; }
    public string Name { get; set; }
    public long DataOffset { get; set; }
    public int Size { get; set; }
    public string FilePath { get; set; }

    public LooseFile(Crc crc, string fileName, string filePath)
    {
        Crc = crc;
        Name = fileName;
        FilePath = filePath;
    }

    public LooseFile(Crc crc, string fileName, long dataOffset, int size)
    {
        Crc = crc;
        Name = fileName;
        DataOffset = dataOffset;
        Size = size;
    }

    public override string ToString()
    {
        return string.IsNullOrEmpty(FilePath)
            ? $"LooseFile(Crc: 0x{Crc:X8}, Name: \"{Name}\", Offset: {DataOffset}, Size: {Size})"
            : $"LooseFile(Crc: 0x{Crc:X8}, Name: \"{Name}\", FilePath: {FilePath})";
    }
}
