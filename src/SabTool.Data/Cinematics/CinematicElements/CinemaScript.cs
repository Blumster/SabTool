namespace SabTool.Data.Cinematics.CinematicElements;

public sealed class CinemaScript : CinemaElement
{
    public float EndTime { get; set; }
    public string Script { get; set; }
    public byte Flags { get; set; }
}
