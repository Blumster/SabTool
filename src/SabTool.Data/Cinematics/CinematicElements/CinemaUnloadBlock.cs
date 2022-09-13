namespace SabTool.Data.Cinematics.CinematicElements;

using SabTool.Utils;

public sealed class CinemaUnloadBlock : CinemaElement
{
    public float EndTime { get; set; }
    public Crc UnkCrc { get; set; }
}
