namespace SabTool.Data.Cinematics.CinematicElements;

public class CinemaElement
{
    public string Type => GetType().Name;
    public float StartTime { get; set; }
}
