using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace SabTool.Data.Graphics
{
    using Utils;
    using Utils.Extensions;

    public class Model
    {
        public Vector3 Field4C { get; set; }
        public Vector4 BoxAndRadius { get; set; }
        public int Field68 { get; set; }
        public int Field78 { get; set; }
        public Crc Name { get; set; }
        public int FieldB0 { get; set; }
        public byte FieldB9 { get; set; }
        public byte FieldBB { get; set; }
        public byte FieldBF { get; set; }

        public bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;

            reader.BaseStream.Position += 0x4C;

            Field4C = reader.ReadVector3();
            BoxAndRadius = reader.ReadVector4();
            Field68 = reader.ReadInt32();

            reader.BaseStream.Position += 0xC;

            Field78 = reader.ReadInt32();

            reader.BaseStream.Position += 0x18;

            Name = new Crc(reader.ReadUInt32());

            reader.BaseStream.Position += 0x18;

            FieldB0 = reader.ReadInt32();

            reader.BaseStream.Position += 0x5;

            FieldB9 = reader.ReadByte();

            reader.BaseStream.Position += 0x1;

            FieldBB = reader.ReadByte();

            reader.BaseStream.Position += 0x3;

            FieldBF = reader.ReadByte();

            if (currentStart + 0xC0 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the model part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0xC0}");
                return false;
            }

            return true;
        }

        public string DumpString(int indentCount = 0)
        {
            var sb = new StringBuilder();

            sb.Append(' ', indentCount).AppendLine($"{nameof(Model)}({Name})");
            sb.Append(' ', indentCount).AppendLine("{");

            indentCount += 2;

            sb.Append(' ', indentCount).AppendLine($"{nameof(Field4C)} = {Field4C}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(BoxAndRadius)} = {BoxAndRadius}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Field68)} = {Field68}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Field78)} = {Field78}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(FieldB0)} = {FieldB0}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(FieldB9)} = {FieldB9}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(FieldBB)} = {FieldBB}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(FieldBF)} = {FieldBF}");

            indentCount -= 2;

            sb.Append(' ', indentCount).AppendLine("}");

            return sb.ToString();
        }
    }
}
