namespace SabTool.Data.Cinematics.CinematicElements;

using SabTool.Utils;

public sealed class CinemaCameraShake : CinemaElement
{
    public float EndTime { get; set; }
    public Crc UnkCrc { get; set; }
    public float Strength { get; set; }
    public float UnkFloat1 { get; set; }
    public float UnkFloat2 { get; set; }
}
