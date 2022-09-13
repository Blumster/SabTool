namespace SabTool.Data.Cinematics.ComplexAnimationElements;

public sealed class SoundElement : ComplexAnimElement
{
    public bool UnkBool { get; set; }

    public SoundElement()
        : base(ElementType.Sound)
    {
    }
}
