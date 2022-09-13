namespace SabTool.Data.Packs;

public sealed class StreamBlock
{
    public static readonly uint[] OffIndices = new uint[9] { 6, 7, 0, 2, 8, 4, 3, 1, 5 };

    public string FileName { get; set; }
    public Crc[] Palettes { get; } = new Crc[32];
    public Vector3 Midpoint { get; set; }
    public float FieldC0 { get; set; }
    public Vector3[] Extents { get; } = new Vector3[2];
    public uint TotalTextureSize { get; set; }
    public int PointCountX { get; set; }
    public int PointCountY { get; set; }
    public float HeightRangeMin { get; set; }
    public float HeightRangeMax { get; set; }
    public uint Flags { get; set; } // 0x8: dynpack? 0x10: no extension? 0x100: palettepack? no 0x8 and no 0x100: pack?
    public byte[] HeightMapData { get; set; }
    public uint TextureCount { get; set; }
    public TextureInfo[] TextureInfoArray { get; set; }
    public uint TextureCount2 { get; set; }
    public TextureInfo[] TextureInfoArray2 { get; set; }
    public uint[] EntryCounts { get; } = new uint[9];
    public Entry[][] Entries { get; } = new Entry[9][];
    public Dictionary<Crc, Crc[]> FenceTree { get; } = new();
    public uint Count1ACFor1B0And1B4_1AC { get; set; }
    public Crc[] Array1B0 { get; set; }
    public byte[] Array1B4 { get; set; }
    public Crc Id { get; set; }
    public ushort PaletteCount { get; set; }
    public int FenceTreeCount { get; set; }
    public short UnkShort { get; set; }
    public ushort Index { get; set; }
    public uint HeaderEnd { get; set; }

    public override string ToString()
    {
        return $"StreamBlock({Id})";
    }

    public void FreePayloads()
    {
        foreach (var entryArray in Entries)
        {
            if (entryArray == null)
                continue;

            foreach (var entry in entryArray)
                entry.Payload = null;
        }
    }

    public class TextureInfo
    {
        public Crc Crc { get; set; }
        public uint UncompressedSize { get; set; }

        public override string ToString()
        {
            return $"TextureInfo({Crc}, {UncompressedSize})";
        }
    }

    public class Entry
    {
        public Crc Crc { get; set; }
        public int Offset { get; set; }
        public int CompressedSize { get; set; }
        public int UncompressedSize { get; set; }
        public byte[] Payload { get; set; }
        public Crc UnkCrc { get; set; }

        public Entry(BinaryReader reader)
        {
            Crc = new(reader.ReadUInt32());
            Offset = reader.ReadInt32();
            CompressedSize = reader.ReadInt32();
            UncompressedSize = reader.ReadInt32();
            _ = reader.ReadInt32();
            UnkCrc = new(reader.ReadUInt32());
        }
    }
}
