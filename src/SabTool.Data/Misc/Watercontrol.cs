using System.Collections.Generic;

namespace SabTool.Data.Misc;

using SabTool.Utils;

public class Watercontrol
{
    public List<WatercontrolPoint> Points { get; set; } = new();
}

public class WatercontrolPoint
{
    public Crc Name { get; set; } // Not unique for each entry
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}
