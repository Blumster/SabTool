namespace SabTool.Data.Cinematics.ComplexAnimationElements;

public sealed class AnimTargetElement : AnimElement
{
    public int UnkInt2 { get; set; }

    public AnimTargetElement()
        : base(ElementType.AnimTarget)
    {
    }
}
