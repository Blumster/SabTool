using System.Numerics;

namespace SabTool.Data.Graphics
{
    using Utils;

    public class Model
    {
        public Mesh Mesh { get; set; }

        public Vector3 Field4C { get; set; }
        public Vector4 BoxAndRadius { get; set; }
        public int Field68 { get; set; }
        public int Field78 { get; set; }
        public Crc Name { get; set; }
        public int FieldB0 { get; set; }
        public byte FieldB9 { get; set; }
        public byte FieldBB { get; set; }
        public byte FieldBF { get; set; }
    }
}
