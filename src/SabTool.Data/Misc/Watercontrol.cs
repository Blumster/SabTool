namespace SabTool.Data.Misc;

using SabTool.Utils;

public sealed class Watercontrol
{
    public List<WatercontrolPoint> Points { get; set; } = new();
}

public sealed class WatercontrolPoint
{
    public Crc Name { get; set; } // Not unique for each entry
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}
