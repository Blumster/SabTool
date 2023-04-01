namespace SabTool.Data.Packs.Assets;

public sealed class TextureAsset
{
    public Crc NameCrc { get; }
    public string Name { get; set; }

    public byte[] DDSFile { get; set; }

    public TextureAsset(Crc nameCrc)
    {
        NameCrc = nameCrc;
    }

    public static TextureAsset Import(string filePath)
    {
        var name = Path.GetFileNameWithoutExtension(filePath);

        return new TextureAsset(Hash.StringToHash(name))
        {
            Name = name,
            DDSFile = File.ReadAllBytes(filePath)
        };
    }

    public void Export(string outputPath)
    {
        if (DDSFile != null)
            File.WriteAllBytes(Path.Combine(outputPath, $"{Name}.dds"), DDSFile);
    }
}
