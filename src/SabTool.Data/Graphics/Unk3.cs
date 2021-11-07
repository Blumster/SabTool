using System;
using System.IO;
using System.Text;

namespace SabTool.Data.Graphics
{
    public class Unk3
    {
        public int UnkSize { get; set; }
        public int NumUnk1 { get; set; }
        public int NumUnk2 { get; set; }
        public int NumUnk3 { get; set; }

        public bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;

            reader.BaseStream.Position += 0x4;

            UnkSize = reader.ReadInt32();
            NumUnk1 = reader.ReadInt32();
            NumUnk2 = reader.ReadInt32();
            NumUnk3 = reader.ReadInt32();

            reader.BaseStream.Position += UnkSize + 0x18;
            reader.BaseStream.Position += NumUnk3 * 0x30;
            reader.BaseStream.Position += NumUnk1 * 0xC;
            reader.BaseStream.Position += NumUnk2 * 0x10;

            var expectedSize = 0x18 + UnkSize + NumUnk1 * 0xC + NumUnk2 * 0x10 + NumUnk3 * 0x30;
            if (currentStart + expectedSize != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the Unk3 part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + expectedSize}");
                return false;
            }

            return true;
        }

        public string DumpString(int indentCount = 0)
        {
            var sb = new StringBuilder();

            sb.Append(' ', indentCount).AppendLine($"{nameof(Unk3)}()");
            sb.Append(' ', indentCount).AppendLine("{");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(UnkSize)} = {UnkSize}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(NumUnk1)} = {NumUnk1}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(NumUnk2)} = {NumUnk2}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(NumUnk3)} = {NumUnk3}");
            sb.Append(' ', indentCount).AppendLine("}");

            return sb.ToString();
        }
    }
}
