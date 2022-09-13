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

    public void Import(string filePath)
    {
        throw new NotImplementedException();
    }

    public void Export(string outputPath)
    {
        if (DDSFile != null)
            File.WriteAllBytes(Path.Combine(outputPath, $"{Name}.dds"), DDSFile);
    }
}
