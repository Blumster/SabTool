using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace SabTool.Data.Graphics
{
    using Utils;

    public class Primitive
    {
        public Mesh Mesh { get; set; }
        public Unk3 Unk3 { get; set; }
        public VertexHolder VertexHolder { get; set; }
        public int Unk3Index { get; set; }
        public int VertexHolderIndex { get; set; }
        public float Float30 { get; set; }
        public float Float34 { get; set; }
        public float Float38 { get; set; }
        public int Int3C { get; set; }
        public float Float40 { get; set; }
        public float Float44 { get; set; }
        public float Float48 { get; set; }
        public int Int4C { get; set; }
        public int Int54 { get; set; }
        public int IndexStartOffset { get; set; }
        public int NumVertex { get; set; }
        public int NumIndices { get; set; }

        public Primitive(Mesh mesh)
        {
            Mesh = mesh;
        }

        public bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;

            reader.BaseStream.Position += 0x4;

            Unk3Index = reader.ReadInt32();
            if (Unk3Index >= 0)
                Unk3 = Mesh.Unk3s[Unk3Index];

            reader.BaseStream.Position += 0x28;

            Float30 = reader.ReadSingle();
            Float34 = reader.ReadSingle();
            Float38 = reader.ReadSingle();
            Int3C = reader.ReadInt32();
            Float40 = reader.ReadSingle();
            Float44 = reader.ReadSingle();
            Float48 = reader.ReadSingle();
            Int4C = reader.ReadInt32();

            VertexHolderIndex = reader.ReadInt32();
            VertexHolder = Mesh.VertexHolders[VertexHolderIndex];

            Int54 = reader.ReadInt32();
            IndexStartOffset = reader.ReadInt32();
            NumVertex = reader.ReadInt32();
            NumIndices = reader.ReadInt32();

            if (currentStart + 0x64 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the primitive part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x64}");
                return false;
            }

            return true;
        }

        public IReadOnlyList<Vector2> GetVertexVector2(int index, VDUsage usage, int usageIndex = -1)
        {
            if (index >= VertexHolder.ArrayCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            var vertices = new List<Vector2>();
            for (var i = 0; i < VertexHolder.Counts[index]; ++i)
            {
                foreach (var vDecl in VertexHolder.Decl1)
                {
                    if (vDecl.Type == VDType.Unused || vDecl.Usage != usage || (usageIndex != -1 && usageIndex != vDecl.UsageIndex))
                        continue;

                    var arr = VertexHolder.Vertices[index];
                    var off = VertexHolder.Sizes[index] * i + vDecl.Offset;

                    var vertex = vDecl.Type switch
                    {
                        VDType.Float1 => new Vector2(BitConverter.ToSingle(arr, off), 0.0f),
                        VDType.Float2 => new Vector2(BitConverter.ToSingle(arr, off), BitConverter.ToSingle(arr, off + 4)),
                        VDType.Short2 => new Vector2(BitConverter.ToInt16(arr, off), BitConverter.ToInt16(arr, off + 2)),
                        VDType.Short2N => new Vector2(BitConverter.ToInt16(arr, off) / 32767.0f, BitConverter.ToInt16(arr, off + 2) / 32767.0f),
                        VDType.UShort2N => new Vector2(BitConverter.ToUInt16(arr, off) / 65535.0f, BitConverter.ToUInt16(arr, off + 2) / 65535.0f),
                        VDType.Float16_2 => new Vector2((float)ExtBitConverter.ToHalf(arr, off), (float)ExtBitConverter.ToHalf(arr, off + 2)),
                        _ => throw new NotSupportedException("This Vertex Declaration type is not supported for vertexes!")
                    };

                    vertices.Add(vertex);
                }
            }

            return vertices;
        }

        public IReadOnlyList<Vector3> GetVertexVector3(int index, VDUsage usage, int usageIndex = -1)
        {
            if (index >= VertexHolder.ArrayCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            var vertices = new List<Vector3>();
            for (var i = 0; i < VertexHolder.Counts[index]; ++i)
            {
                foreach (var vDecl in VertexHolder.Decl1)
                {
                    if (vDecl.Type == VDType.Unused || vDecl.Usage != usage || (usageIndex != -1 && usageIndex != vDecl.UsageIndex))
                        continue;

                    var arr = VertexHolder.Vertices[index];
                    var off = VertexHolder.Sizes[index] * i + vDecl.Offset;

                    var vertex = vDecl.Type switch
                    {
                        VDType.Float1 => new Vector3(BitConverter.ToSingle(arr, off), 0.0f, 0.0f),
                        VDType.Float2 => new Vector3(BitConverter.ToSingle(arr, off), BitConverter.ToSingle(arr, off + 4), 0.0f),
                        VDType.Float3 => new Vector3(BitConverter.ToSingle(arr, off), BitConverter.ToSingle(arr, off + 4), BitConverter.ToSingle(arr, off + 8)),
                        VDType.Short2 => new Vector3(BitConverter.ToInt16(arr, off), BitConverter.ToInt16(arr, off + 2), 0.0f),
                        VDType.Short2N => new Vector3(BitConverter.ToInt16(arr, off) / 32767.0f, BitConverter.ToInt16(arr, off + 2) / 32767.0f, 0.0f),
                        VDType.UShort2N => new Vector3(BitConverter.ToUInt16(arr, off) / 65535.0f, BitConverter.ToUInt16(arr, off + 2) / 65535.0f, 0.0f),
                        VDType.Float16_2 => new Vector3((float)ExtBitConverter.ToHalf(arr, off), (float)ExtBitConverter.ToHalf(arr, off + 2), 0.0f),
                        VDType.Float16_4 => new Vector3((float)ExtBitConverter.ToHalf(arr, off), (float)ExtBitConverter.ToHalf(arr, off + 2), (float)ExtBitConverter.ToHalf(arr, off + 4)),
                        _ => throw new NotSupportedException("This Vertex Declaration type is not supported for vertexes!")
                    };

                    vertices.Add(vertex);
                }
            }

            return vertices;
        }

        public IReadOnlyList<Vector4> GetVertexVector4(int index, VDUsage usage, int usageIndex = -1)
        {
            if (index >= VertexHolder.ArrayCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            var vertices = new List<Vector4>();
            for (var i = 0; i < VertexHolder.Counts[index]; ++i)
            {
                foreach (var vDecl in VertexHolder.Decl1)
                {
                    if (vDecl.Type == VDType.Unused || vDecl.Usage != usage || (usageIndex != -1 && usageIndex != vDecl.UsageIndex))
                        continue;

                    var arr = VertexHolder.Vertices[index];
                    var off = VertexHolder.Sizes[index] * i + vDecl.Offset;

                    var vertex = vDecl.Type switch
                    {
                        VDType.Float1 => new Vector4(BitConverter.ToSingle(arr, off), 0.0f, 0.0f, 1.0f),
                        VDType.Float2 => new Vector4(BitConverter.ToSingle(arr, off), BitConverter.ToSingle(arr, off + 4), 0.0f, 1.0f),
                        VDType.Float3 => new Vector4(BitConverter.ToSingle(arr, off), BitConverter.ToSingle(arr, off + 4), BitConverter.ToSingle(arr, off + 8), 1.0f),
                        VDType.Float4 => new Vector4(BitConverter.ToSingle(arr, off), BitConverter.ToSingle(arr, off + 4), BitConverter.ToSingle(arr, off + 8), BitConverter.ToSingle(arr, off + 12)),
                        VDType.UByte4 => new Vector4(arr[off], arr[off + 1], arr[off + 2], arr[off + 3]),
                        VDType.Short2 => new Vector4(BitConverter.ToInt16(arr, off), BitConverter.ToInt16(arr, off + 2), 0.0f, 1.0f),
                        VDType.Short4 => new Vector4(BitConverter.ToInt16(arr, off), BitConverter.ToInt16(arr, off + 2), BitConverter.ToInt16(arr, off + 4), BitConverter.ToInt16(arr, off + 6)),
                        VDType.UByte4N => new Vector4(arr[off] / 255.0f, arr[off + 1] / 255.0f, arr[off + 2] / 255.0f, arr[off + 3] / 255.0f),
                        VDType.Short2N => new Vector4(BitConverter.ToInt16(arr, off) / 32767.0f, BitConverter.ToInt16(arr, off + 2) / 32767.0f, 0.0f, 1.0f),
                        VDType.Short4N => new Vector4(BitConverter.ToInt16(arr, off) / 32767.0f, BitConverter.ToInt16(arr, off + 2) / 32767.0f, BitConverter.ToInt16(arr, off + 4) / 32767.0f, BitConverter.ToInt16(arr, off + 6) / 32767.0f),
                        VDType.UShort2N => new Vector4(BitConverter.ToUInt16(arr, off) / 65535.0f, BitConverter.ToUInt16(arr, off + 2) / 65535.0f, 0.0f, 1.0f),
                        VDType.UShort4N => new Vector4(BitConverter.ToUInt16(arr, off) / 65535.0f, BitConverter.ToUInt16(arr, off + 2) / 65535.0f, BitConverter.ToUInt16(arr, off + 4) / 65535.0f, BitConverter.ToUInt16(arr, off + 6) / 65535.0f),
                        VDType.UDec3 => new Vector4(BitConverter.ToUInt32(arr, off) & 0x3FF, (BitConverter.ToUInt32(arr, off) & 0xFFC00) >> 10, (BitConverter.ToUInt32(arr, off) & 0x3FF00000) >> 20, 1.0f),
                        VDType.Dec3N => new Vector4((BitConverter.ToInt32(arr, off) & 0x3FF) / 511.0f, ((BitConverter.ToInt32(arr, off) & 0xFFC00) >> 10) / 511.0f, ((BitConverter.ToInt32(arr, off) & 0x3FF00000) >> 20) / 511.0f, 1.0f),
                        VDType.Float16_2 => new Vector4((float)ExtBitConverter.ToHalf(arr, off), (float)ExtBitConverter.ToHalf(arr, off + 2), 0.0f, 1.0f),
                        VDType.Float16_4 => new Vector4((float)ExtBitConverter.ToHalf(arr, off), (float)ExtBitConverter.ToHalf(arr, off + 2), (float)ExtBitConverter.ToHalf(arr, off + 4), (float)ExtBitConverter.ToHalf(arr, off + 6)),
                        _ => throw new NotSupportedException("This Vertex Declaration type is not supported for vertexes!")
                    };

                    vertices.Add(vertex);
                }
            }

            return vertices;
        }

        public IReadOnlyList<int> GetIndices(int index)
        {
            if (index >= VertexHolder.ArrayCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            var indices = new List<int>();

            var is16Bit = VertexHolder.ArrayCount <= 1;
            var off = IndexStartOffset * (is16Bit ? 2 : 4);

            for (var i = 0; i < NumIndices; ++i)
            {
                if (is16Bit)
                {
                    indices.Add(BitConverter.ToInt16(VertexHolder.Indices, off));

                    off += 2;
                }
                else
                {
                    indices.Add(BitConverter.ToInt32(VertexHolder.Indices, off));

                    off += 4;
                }
            }

            return indices;
        }

        public string DumpString(int indentCount = 0)
        {
            var sb = new StringBuilder();

            sb.Append(' ', indentCount).AppendLine($"{nameof(Primitive)}()");
            sb.Append(' ', indentCount).AppendLine("{");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Unk3Index)} = {Unk3Index}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(VertexHolderIndex)} = {VertexHolderIndex}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Float30)} = {Float30}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Float34)} = {Float34}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Float38)} = {Float38}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Int3C)} = {Int3C}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Float40)} = {Float40}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Float44)} = {Float44}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Float48)} = {Float48}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Int4C)} = {Int4C}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Int54)} = {Int54}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(IndexStartOffset)} = {IndexStartOffset}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(NumVertex)} = {NumVertex}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(NumIndices)} = {NumIndices}");
            sb.Append(' ', indentCount).AppendLine("}");

            return sb.ToString();
        }
    }
}
