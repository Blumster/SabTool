namespace SabTool.Data.Cinematics.CinematicElements;

using SabTool.Utils;

public sealed class CinemaFX : CinemaElement
{
    public float EndTime { get; set; }
    public Crc UnkCrc1 { get; set; }
    public Crc UnkCrc2 { get; set; }
    public Crc UnkCrc3 { get; set; }
}
