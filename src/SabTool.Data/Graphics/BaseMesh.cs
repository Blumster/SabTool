using System;
using System.IO;

namespace SabTool.Data.Graphics
{
    public class BaseMesh
    {
        public Skeleton Skeleton { get; set; }
        public uint NumBones { get; set; }
        public int NumUnk1 { get; set; }
        public int Field14 { get; set; }
        public short NumUnk4 { get; set; }
        public short NumPrimitives { get; set; }
        public short NumUnk3 { get; set; }
        public short Field1E { get; set; }
        public int Field20 { get; set; }
        public int Field24 { get; set; }
        public short NumSegments { get; set; }
        public byte Field2A { get; set; }
        public byte Field2B { get; set; }

        public virtual bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;

            reader.BaseStream.Position += 0xC;

            NumBones = reader.ReadUInt32();
            NumUnk1 = reader.ReadInt32();
            Field14 = reader.ReadInt32();
            NumUnk4 = reader.ReadInt16();
            NumPrimitives = reader.ReadInt16();
            NumUnk3 = reader.ReadInt16();
            Field1E = reader.ReadInt16();
            Field20 = reader.ReadInt32();
            Field24 = reader.ReadInt32();
            NumSegments = reader.ReadInt16(); // numUnk5
            Field2A = reader.ReadByte();
            Field2B = reader.ReadByte();

            if (currentStart + 0x2C != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the BaseMesh part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x2C}");
                return false;
            }

            return true;
        }
    }
}
