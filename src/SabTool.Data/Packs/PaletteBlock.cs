namespace SabTool.Data.Packs;

using SabTool.Utils;

public sealed class PaletteBlock
{
    public Crc Crc { get; set; }
    public float X { get; set; }
    public float Z { get; set; }
    public ushort Index { get; set; }
    public List<Crc> Palettes { get; set; }
}
