using System;
using System.IO;
using System.Numerics;
using System.Text;

using SharpGLTF.Transforms;

namespace SabTool.Data.Graphics
{
    using Utils.Extensions;

    public class Skeleton
    {
        public static Skeleton SingleBoneInstance { get; } = new Skeleton
        {
            NumBones = 1
        };

        public int NumBones { get; set; }
        public int SomeSize { get; set; }
        public int Int4 { get; set; }
        public int Int8 { get; set; }
        public int IntC { get; set; }
        public int SomeSize2 { get; set; }
        public int Int14 { get; set; }
        public int Int18 { get; set; }
        public int Int1C { get; set; }
        public int Int20 { get; set; }
        public int Int24 { get; set; }
        public Matrix4x4[] BasePoses { get; set; }
        public AffineTransform[] UnkBasePoses { get; set; }
        public short[] Indices { get; set; }
        public Bone[] Bones { get; set; }

        public bool Read(BinaryReader reader)
        {
            SomeSize = reader.ReadInt32();
            Int4 = reader.ReadInt32();
            Int8 = reader.ReadInt32();
            IntC = reader.ReadInt32();
            SomeSize2 = reader.ReadInt32();
            Int14 = reader.ReadInt32();
            Int18 = reader.ReadInt32();
            Int1C = reader.ReadInt32();
            Int20 = reader.ReadInt32();
            Int24 = reader.ReadInt32();

            reader.BaseStream.Position += 0x4;

            // Unused/unknown bone indexes, overwritten almost instantly after loading
            reader.BaseStream.Position += NumBones;

            // Unused/unknown data, found to be zeros all the time
            reader.BaseStream.Position += SomeSize;

            BasePoses = new Matrix4x4[NumBones];
            for (var i = 0; i < NumBones; ++i)
            {
                BasePoses[i] = reader.ReadMatrix4x4();
            }

            Bones = new Bone[NumBones];
            for (var i = 0; i < NumBones; ++i)
            {
                Bones[i] = new Bone();
                Bones[i].Read(reader);
                Bones[i].Index = (short)i;
            }

            UnkBasePoses = new AffineTransform[NumBones];
            for (var i = 0; i < NumBones; ++i)
                UnkBasePoses[i] = reader.ReadAffineTransform();

            Indices = new short[NumBones];
            for (var i = 0; i < NumBones; ++i)
                Indices[i] = reader.ReadInt16();

            reader.BaseStream.Position += SomeSize2;

            // Pointer array without data to skip allocation
            reader.BaseStream.Position += 4 * NumBones;

            return true;
        }

        public string DumpString(int indentCount = 0)
        {
            var sb = new StringBuilder();

            sb.Append(' ', indentCount).AppendLine($"{nameof(Skeleton)}()");
            sb.Append(' ', indentCount).AppendLine("{");

            indentCount += 2;

            sb.Append(' ', indentCount).AppendLine($"{nameof(SomeSize)} = {SomeSize}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Int4)} = {Int4}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Int8)} = {Int8}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(IntC)} = {IntC}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(SomeSize2)} = {SomeSize2}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Int14)} = {Int14}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Int18)} = {Int18}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Int1C)} = {Int1C}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Int20)} = {Int20}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Int24)} = {Int24}");
            sb.Append(' ', indentCount).AppendLine($"{nameof(BasePoses)} =");
            sb.Append(' ', indentCount).AppendLine("[");

            for (var i = 0; i < NumBones; ++i)
            {
                sb.Append(' ', indentCount + 2).AppendLine($"{i}: {nameof(Matrix4x4)} = {BasePoses[i]}");
            }

            sb.Append(' ', indentCount).AppendLine("]");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Bones)} =");
            sb.Append(' ', indentCount).AppendLine("[");

            for (var i = 0; i < NumBones; ++i)
            {
                sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
                sb.Append(Bones[i].DumpString(indentCount + 4));
            }

            sb.Append(' ', indentCount).AppendLine("]");
            sb.Append(' ', indentCount).AppendLine($"{nameof(UnkBasePoses)} =");
            sb.Append(' ', indentCount).AppendLine("[");

            for (var i = 0; i < NumBones; ++i)
            {
                sb.Append(' ', indentCount + 2).AppendLine($"{i}: {nameof(AffineTransform)} = {{ Scale: {UnkBasePoses[i].Scale} Rotation: {UnkBasePoses[i].Rotation} Translation: {UnkBasePoses[i].Translation} }}");
            }

            sb.Append(' ', indentCount).AppendLine("]");
            sb.Append(' ', indentCount).AppendLine($"{nameof(Indices)} =");
            sb.Append(' ', indentCount).AppendLine("[");

            for (var i = 0; i < NumBones; ++i)
            {
                sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Indices)} = {Indices[i]}");
            }

            sb.Append(' ', indentCount).AppendLine("]");

            indentCount -= 2;

            sb.Append(' ', indentCount).AppendLine("}");

            return sb.ToString();
        }
    }
}
