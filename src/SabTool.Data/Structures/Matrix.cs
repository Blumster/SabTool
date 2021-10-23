using System.IO;

namespace SabTool.Data.Structures
{
    public class Matrix
    {
        public Vector4 X { get; set; }
        public Vector4 Y { get; set; }
        public Vector4 Z { get; set; }
        public Vector4 W { get; set; }

        public Matrix(BinaryReader reader)
        {
            X = new Vector4(reader);
            Y = new Vector4(reader);
            Z = new Vector4(reader);
            W = new Vector4(reader);
        }

        // TODO: maybe indexer operator?

        public override string ToString()
        {
            return $"Matrix4x4({X}, {Y}, {Z}, {W})";
        }
    }
}
