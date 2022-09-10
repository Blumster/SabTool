namespace SabTool.Data.Cinematics.ComplexAnimationElements;

using SabTool.Utils;

public enum ElementType
{
    Anim = 0,
    AnimTarget = 1,
    Say = 2,
    Sound = 3,
    Target = 4,
    Listener = 5,
    Expression = 6,
    Unk7 = 7,
    Unk8 = 8
}

public class ComplexAnimElement
{
    public Crc Id { get; set; }
    public ElementType Type { get; set; }
    public Crc UnkCrc { get; set; }
    public int UnkInt { get; set; }
    public float UnkF1 { get; set; }
    public float UnkF2 { get; set; }

    public ComplexAnimElement(ElementType type)
    {
        Type = type;
    }
}
