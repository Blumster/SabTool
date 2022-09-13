namespace SabTool.Data.Misc;

public sealed class Freeplay
{    
    public int Unknown4TotalCount { get; set; }
    public List<FreeplayPoint> Points { get; set; } = new();
}

public sealed class FreeplayPoint
{
    public uint Unknown1 { get; set; }
    public uint Unknown2 { get; set; }
    public Crc Crc { get; set; } // Not unique for all freeplay points
    public int Unknown3 { get; set; } // Always 0 except when count of Unknown4 is not 1
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public List<uint> Unknown4 { get; set; } = new(); // A single freeplay point has two of these, all others have one
}
