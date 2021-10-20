using System;
using System.IO;

namespace SabTool.Data.Graphics
{
    public class Mesh : BaseMesh
    {
        public int Field2C { get; set; }
        public byte Field30 { get; set; }
        public byte Field31 { get; set; }
        public byte Field32 { get; set; }
        public byte Field33 { get; set; }

        public override bool Read(BinaryReader reader)
        {
            base.Read(reader);

            var currentStart = reader.BaseStream.Position;

            Field2C = reader.ReadInt32();
            Field30 = reader.ReadByte();
            Field31 = reader.ReadByte();
            Field32 = reader.ReadByte();
            Field33 = reader.ReadByte();

            if (currentStart + 0x8 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the Mesh part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x8}");
                return false;
            }

            return true;
        }
    }
}
