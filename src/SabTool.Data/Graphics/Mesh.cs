namespace SabTool.Data.Graphics
{
    public class Mesh
    {
        public Skeleton Skeleton { get; set; }
        public Segment[] Segments { get; set; }
        public Primitive[] Primitives { get; set; }
        public VertexHolder[] VertexHolders { get; set; }
        public Unk1[] Unk1s { get; set; }
        public Unk3[] Unk3s { get; set; }
        public int NumBones { get; set; }
        public int NumUnk1 { get; set; }
        public int Field14 { get; set; }
        public short NumVertexHolder { get; set; }
        public short NumPrimitives { get; set; }
        public short NumUnk3 { get; set; }
        public short Field1E { get; set; }
        public int Field20 { get; set; }
        public int Field24 { get; set; }
        public short NumSegments { get; set; }
        public byte Field2A { get; set; }
        public byte Field2B { get; set; }
        public int Field2C { get; set; }
        public byte Field30 { get; set; }
        public byte Field31 { get; set; }
        public byte Field32 { get; set; }
        public byte Field33 { get; set; }
    }
}
