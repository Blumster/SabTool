namespace SabTool.Data.Cinematics.ComplexAnimationElements
{
    public class AnimElement : ComplexAnimElement
    {
        public float UnkFloat { get; set; }
        public bool UnkBool1 { get; set; }
        public bool UnkBool2 { get; set; }

        public AnimElement(ElementType type = ElementType.Anim)
            : base(type)
        {
        }
    }
}
