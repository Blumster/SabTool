using System.Drawing;
using System.Numerics;
using System.Text;

namespace SabTool.Data.Graphics;

public sealed class VertexHolder
{
    public List<VertexDeclaration> Decl1 { get; set; }
    public List<VertexDeclaration> Decl2 { get; set; }
    public byte[][] Vertices { get; set; } = new byte[4][];
    public byte[] Indices { get; set; }

    public int[] Counts { get; } = new int[4];
    public int[] Formats { get; } = new int[4];
    public long[] UVFormats { get; } = new long[4];
    public int[] ArrayOffsets { get; } = new int[4];
    public int[] ArraySizes { get; } = new int[4];
    public byte[] Sizes { get; } = new byte[4];
    public int IndexArrayOffset { get; set; }
    public int IndexArraySize { get; set; }
    public uint SomeFlags { get; set; }
    public int ArrayCount { get; set; }
    public int IndexCount { get; set; }

    public string DumpVertices(int indentCount = 0)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < ArrayCount; ++i)
        {
            sb.Append(' ', indentCount).AppendLine($"VertexArray[{i}] =");
            sb.Append(' ', indentCount).AppendLine("[");

            indentCount += 2;

            for (var j = 0; j < Counts[i]; ++j)
            {
                sb.Append(' ', indentCount).AppendLine($"Vertex[{j}] =");
                sb.Append(' ', indentCount).AppendLine("{");

                indentCount += 2;

                foreach (var vertexDeclaration in Decl1)
                {
                    if (vertexDeclaration.Type == VDType.Unused)
                        continue;

                    sb.Append(' ', indentCount).Append($"{vertexDeclaration.Usage}: ");

                    var arr = Vertices[i];
                    var off = Sizes[i] * j + vertexDeclaration.Offset;

                    object val = vertexDeclaration.Type switch
                    {
                        VDType.Float1 => new Vector4(BitConverter.ToSingle(arr, off), 0.0f, 0.0f, 1.0f),
                        VDType.Float2 => new Vector4(BitConverter.ToSingle(arr, off), BitConverter.ToSingle(arr, off + 4), 0.0f, 1.0f),
                        VDType.Float3 => new Vector4(BitConverter.ToSingle(arr, off), BitConverter.ToSingle(arr, off + 4), BitConverter.ToSingle(arr, off + 8), 1.0f),
                        VDType.Float4 => new Vector4(BitConverter.ToSingle(arr, off), BitConverter.ToSingle(arr, off + 4), BitConverter.ToSingle(arr, off + 8), BitConverter.ToSingle(arr, off + 12)),
                        VDType.D3DColor => Color.FromArgb(arr[off + 3], arr[off], arr[off + 1], arr[off + 2]),
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
                        VDType.Float16_2 => new Vector4((float)BitConverter.ToHalf(arr, off), (float)BitConverter.ToHalf(arr, off + 2), 0.0f, 1.0f),
                        VDType.Float16_4 => new Vector4((float)BitConverter.ToHalf(arr, off), (float)BitConverter.ToHalf(arr, off + 2), (float)BitConverter.ToHalf(arr, off + 4), (float)BitConverter.ToHalf(arr, off + 6)),
                        VDType.Unused => "Unused",
                        _ => "Unknown type",
                    };

                    sb.AppendLine(val.ToString());
                }

                indentCount -= 2;

                sb.Append(' ', indentCount).AppendLine("}");
            }

            indentCount -= 2;

            sb.Append(' ', indentCount).AppendLine("]");
        }

        if (IndexCount > 0)
        {
            sb.Append(' ', indentCount).AppendLine($"IndexArray =");
            sb.Append(' ', indentCount).AppendLine("[");

            indentCount += 2;

            var off = 0;
            var is16Bit = ArrayCount <= 1;

            for (var i = 0; i < IndexCount; ++i)
            {
                if (i % 3 == 0)
                {
                    if (i > 0)
                        sb.AppendLine();

                    sb.Append(' ', indentCount);
                }

                if (is16Bit)
                {
                    sb.Append(BitConverter.ToInt16(Indices, off)).Append(" ");

                    off += 2;
                }
                else
                {
                    sb.Append(BitConverter.ToInt32(Indices, off)).Append(" ");

                    off += 4;
                }
            }

            indentCount -= 2;

            sb.AppendLine().Append(' ', indentCount).AppendLine("]");
        }

        return sb.ToString();
    }

    public string DumpString(int indentCount = 0)
    {
        var sb = new StringBuilder();

        sb.Append(' ', indentCount).AppendLine($"{nameof(VertexHolder)}()");
        sb.Append(' ', indentCount).AppendLine("{");

        indentCount += 2;

        for (var i = 0; i < ArrayCount; ++i)
            sb.Append(' ', indentCount).AppendLine($"{nameof(Counts)}[{i}] = {Counts[i]}");

        for (var i = 0; i < ArrayCount; ++i)
            sb.Append(' ', indentCount).AppendLine($"{nameof(Formats)}[{i}] = 0x{Formats[i]:X8}");

        for (var i = 0; i < ArrayCount; ++i)
            sb.Append(' ', indentCount).AppendLine($"{nameof(UVFormats)}[{i}] = 0x{UVFormats[i]:X8}");

        for (var i = 0; i < ArrayCount; ++i)
            sb.Append(' ', indentCount).AppendLine($"{nameof(ArrayOffsets)}[{i}] = {ArrayOffsets[i]}");

        for (var i = 0; i < ArrayCount; ++i)
            sb.Append(' ', indentCount).AppendLine($"{nameof(ArraySizes)}[{i}] = {ArraySizes[i]}");

        for (var i = 0; i < ArrayCount; ++i)
            sb.Append(' ', indentCount).AppendLine($"{nameof(Sizes)}[{i}] = {Sizes[i]}");

        sb.Append(' ', indentCount).AppendLine($"{nameof(IndexArrayOffset)} = {IndexArrayOffset}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(IndexArraySize)} = {IndexArraySize}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(SomeFlags)} = 0x{SomeFlags:X8}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(ArrayCount)} = {ArrayCount}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(IndexCount)} = {IndexCount}");

        for (var i = 0; i < Decl1.Count; ++i)
        {
            sb.Append(' ', indentCount).AppendLine($"{nameof(Decl1)}[{i}] = {Decl1[i]}");
        }

        for (var i = 0; i < Decl2.Count; ++i)
        {
            sb.Append(' ', indentCount).AppendLine($"{nameof(Decl2)}[{i}] = {Decl2[i]}");
        }

        sb.Append(' ', indentCount).AppendLine("Vertexes =");
        sb.Append(' ', indentCount).AppendLine("[");

        sb.Append(DumpVertices(indentCount + 2));

        sb.Append(' ', indentCount).AppendLine("]");

        indentCount -= 2;

        sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
