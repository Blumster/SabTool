using System;
using System.IO;

namespace SabTool.Data.Graphics
{
    public class Primitive
    {
        public Mesh Mesh { get; set; }
        public Unk3 Unk3 { get; set; }
        public VertexHolder VertexHolder { get; set; }
        public float Float30 { get; set; }
        public float Float34 { get; set; }
        public float Float38 { get; set; }
        public int Int3C { get; set; }
        public float Float40 { get; set; }
        public float Float44 { get; set; }
        public float Float48 { get; set; }
        public int Int4C { get; set; }
        public int Int54 { get; set; }
        public int Int58 { get; set; }
        public int Int5C { get; set; }
        public int NumIndices { get; set; }

        public Primitive(Mesh mesh)
        {
            Mesh = mesh;
        }

        public bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;

            reader.BaseStream.Position += 0x4;

            Unk3 = Mesh.Unk3s[reader.ReadInt32()];

            reader.BaseStream.Position += 0x28;

            // Win32Primitive
            Float30 = reader.ReadSingle();
            Float34 = reader.ReadSingle();
            Float38 = reader.ReadSingle();
            Int3C = reader.ReadInt32();
            Float40 = reader.ReadSingle();
            Float44 = reader.ReadSingle();
            Float48 = reader.ReadSingle();
            Int4C = reader.ReadInt32();

            VertexHolder = Mesh.VertexHolders[reader.ReadInt32()];

            Int54 = reader.ReadInt32();
            Int58 = reader.ReadInt32();
            Int5C = reader.ReadInt32();
            NumIndices = reader.ReadInt32();

            if (currentStart + 0x64 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the primitive part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x64}");
                return false;
            }

            return true;
        }
    }
}
