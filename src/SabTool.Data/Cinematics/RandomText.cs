namespace SabTool.Data.Cinematics;

[Flags]
public enum RandomTextFlags
{
    None = 0,
    EqualProbability = 0x1
}

public sealed class RandomText
{
    public Crc Id { get; set; }
    public int NumTexts { get; set; }
    public float TotalProbability { get; set; }
    public List<Crc> Texts { get; } = new();
    public List<float> Probabilities { get; } = new();
    public RandomTextFlags Flags { get; set; }
}
