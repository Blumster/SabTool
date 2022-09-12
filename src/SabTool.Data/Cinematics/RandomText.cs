namespace SabTool.Data.Cinematics;

using SabTool.Utils;

[Flags]
public enum RandomTextFlags
{
    None = 0,
    EqualProbability = 0x1
}

public class RandomText
{
    public Crc Id { get; set; }
    public int NumTexts { get; set; }
    public float TotalProbability { get; set; }
    public List<Crc> Texts { get; } = new();
    public List<float> Probabilities { get; } = new();
    public RandomTextFlags Flags { get; set; }
}
