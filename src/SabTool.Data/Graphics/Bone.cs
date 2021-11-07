using System;
using System.IO;

namespace SabTool.Data.Graphics
{
    using Utils;

    public class Bone
    {
        public Crc UnkNamePtr { get; set; }
        public byte UnkByte { get; set; }
        public Crc Crc { get; set; }
        public short Flags { get; set; }
        public short Index { get; set; }
        public byte UnkFlags { get; set; }
        public int Field20 { get; set; }
        public int Field24 { get; set; }
        public int Field28 { get; set; }
        public int Field2C { get; set; }
        public int Field30 { get; set; }
        public int Field34 { get; set; }
        public int Field38 { get; set; }
        public int Field3C { get; set; }

        public bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;

            UnkNamePtr = new Crc(reader.ReadUInt32());
            UnkByte = reader.ReadByte();

            reader.BaseStream.Position += 0xF;

            Crc = new Crc(reader.ReadUInt32());
            Flags = reader.ReadInt16();
            Index = reader.ReadInt16();
            UnkFlags = reader.ReadByte();

            reader.BaseStream.Position += 0x3;

            Field20 = reader.ReadInt32();
            Field24 = reader.ReadInt32();
            Field28 = reader.ReadInt32();
            Field2C = reader.ReadInt32();
            Field30 = reader.ReadInt32();
            Field34 = reader.ReadInt32();
            Field38 = reader.ReadInt32();
            Field3C = reader.ReadInt32();

            if (currentStart + 0x40 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the Bone part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x40}");
                return false;
            }

            return true;
        }
    }
}
