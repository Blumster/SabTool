namespace SabTool.Data.Cinematics.ComplexAnimationElements;

public class ExpressionElement : ComplexAnimElement
{
    public int UnkInt2 { get; set; }
    public float UnkFloat { get; set; }

    public ExpressionElement()
        : base(ElementType.Expression)
    {
    }
}
