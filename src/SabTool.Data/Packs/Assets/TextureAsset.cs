namespace SabTool.Data.Packs.Assets;

public sealed class TextureAsset
{
    public Crc NameCrc { get; }
    public string Name { get; set; }

    public byte[] DDSFile { get; set; }

    public TextureMetadata Metadata { get; set; }

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

/// <summary>
/// Game specific texture data that is not representable in a DDS file, kept to allow a lossless repack.
/// </summary>
public sealed class TextureMetadata
{
    public string OriginalName { get; set; }
    public int NameLength { get; set; }
    public uint Format { get; set; }
    public int Flags { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int MipCount { get; set; }
    public int NumChunks { get; set; }
    public int[] ChunkSizes { get; set; }
    public byte[][] MipHeaders { get; set; }
}
