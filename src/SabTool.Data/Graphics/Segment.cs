namespace SabTool.Data.Graphics
{
    using Utils;

    public class Segment
    {
        public Mesh Mesh { get; set; }
        public int PrimitiveIndex { get; set; }
        public Primitive Primitive { get; set; }
        public Crc MaterialCrc { get; set; }
        public short BoneIndex { get; set; }
        public short Flags { get; set; }
    }
}
