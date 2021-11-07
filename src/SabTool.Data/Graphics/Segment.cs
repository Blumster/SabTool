using System;
using System.IO;

namespace SabTool.Data.Graphics
{
    using Utils;

    public class Segment
    {
        public Mesh Mesh { get; }
        public int PrimitiveIndex { get; set; }
        public Primitive Primitive { get; set; }
        public Crc MaterialCrc { get; set; }
        public short Flags { get; set; }

        public Segment(Mesh mesh)
        {
            Mesh = mesh;
        }

        public bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;

            PrimitiveIndex = reader.ReadInt32();

            Primitive = Mesh.Primitives[PrimitiveIndex];
            MaterialCrc = new Crc(reader.ReadUInt32());

            reader.BaseStream.Position += 0x6;

            Flags = reader.ReadInt16();

            if (currentStart + 0x10 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the Segment part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x10}");
                return false;
            }

            return true;
        }
    }
}
