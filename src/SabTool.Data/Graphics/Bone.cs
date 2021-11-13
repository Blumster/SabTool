using System;
using System.IO;
using System.Text;

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
        public float Field20 { get; set; }
        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public int Field2C { get; set; }
        public float Field30 { get; set; }
        public float Field34 { get; set; }
        public float Field38 { get; set; }
        public float Field3C { get; set; }

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

            Field20 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadInt32();
            Field30 = reader.ReadSingle();
            Field34 = reader.ReadSingle();
            Field38 = reader.ReadSingle();
            Field3C = reader.ReadSingle();

            if (currentStart + 0x40 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the Bone part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x40}");
                return false;
            }

            return true;
        }

        public string DumpString(int indentCount = 0)
        {
            var sb = new StringBuilder();

            sb.Append(' ', indentCount).AppendLine($"{nameof(Bone)}()");
            sb.Append(' ', indentCount).AppendLine("{");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(UnkNamePtr)} = {UnkNamePtr}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(UnkByte)} = {UnkByte}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Crc)} = {Crc}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Flags)} = 0x{Flags:X4}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Index)} = {Index}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(UnkFlags)} = 0x{UnkFlags:X2}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Field20)} = {Field20}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Field24)} = {Field24}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Field28)} = {Field28}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Field2C)} = {Field2C}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Field30)} = {Field30}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Field34)} = {Field34}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Field38)} = {Field38}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Field3C)} = {Field3C}");
            sb.Append(' ', indentCount).AppendLine("}");

            return sb.ToString();
        }
    }
}
