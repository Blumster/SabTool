namespace SabTool.Data.Misc;

public sealed class Waterflow
{
    public List<WaterflowPoint> Points { get; set; } = new ();
}

public sealed class WaterflowPoint
{
    public Crc Name { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public float Unknown { get; set; }  // always 5 except for reference point 83 and 84, maybe has to do with fog or the bridge there
}
