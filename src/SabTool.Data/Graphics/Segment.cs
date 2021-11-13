using System;
using System.IO;
using System.Text;

namespace SabTool.Data.Graphics
{
    using Utils;

    public class Segment
    {
        public Mesh Mesh { get; }
        public int PrimitiveIndex { get; set; }
        public Primitive Primitive { get; set; }
        public Crc MaterialCrc { get; set; }
        public short BoneIndex { get; set; }
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

            reader.BaseStream.Position += 0x4;

            BoneIndex = reader.ReadInt16();
            Flags = reader.ReadInt16();

            if (currentStart + 0x10 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the Segment part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x10}");
                return false;
            }

            return true;
        }

        public string DumpString(int indentCount = 0)
        {
            var sb = new StringBuilder();

            sb.Append(' ', indentCount).AppendLine($"{nameof(Segment)}()");
            sb.Append(' ', indentCount).AppendLine("{");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(PrimitiveIndex)} = {PrimitiveIndex}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(MaterialCrc)} = {MaterialCrc}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(BoneIndex)} = {BoneIndex}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Flags)} = 0x{Flags:X4}");
            sb.Append(' ', indentCount).AppendLine("}");

            return sb.ToString();
        }
    }
}
