namespace SabTool.Data.Cinematics.CinematicElements;

public class CinemaScript : CinemaElement
{
    public float EndTime { get; set; }
    public string Script { get; set; }
    public byte Flags { get; set; }
}
