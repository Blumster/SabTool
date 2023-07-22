using System.Drawing;

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
        StringBuilder sb = new();

        for (int i = 0; i < ArrayCount; ++i)
        {
            _ = sb.Append(' ', indentCount).AppendLine($"VertexArray[{i}] =");
            _ = sb.Append(' ', indentCount).AppendLine("[");

            indentCount += 2;

            for (int j = 0; j < Counts[i]; ++j)
            {
                _ = sb.Append(' ', indentCount).AppendLine($"Vertex[{j}] =");
                _ = sb.Append(' ', indentCount).AppendLine("{");

                indentCount += 2;

                foreach (VertexDeclaration vertexDeclaration in Decl1)
                {
                    if (vertexDeclaration.Type == VDType.Unused)
                        continue;

                    _ = sb.Append(' ', indentCount).Append($"{vertexDeclaration.Usage}: ");

                    byte[] arr = Vertices[i];
                    int off = (Sizes[i] * j) + vertexDeclaration.Offset;

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

                    _ = sb.AppendLine(val.ToString());
                }

                indentCount -= 2;

                _ = sb.Append(' ', indentCount).AppendLine("}");
            }

            indentCount -= 2;

            _ = sb.Append(' ', indentCount).AppendLine("]");
        }

        if (IndexCount > 0)
        {
            _ = sb.Append(' ', indentCount).AppendLine($"IndexArray =");
            _ = sb.Append(' ', indentCount).AppendLine("[");

            indentCount += 2;

            int off = 0;
            bool is16Bit = ArrayCount <= 1;

            for (int i = 0; i < IndexCount; ++i)
            {
                if (i % 3 == 0)
                {
                    if (i > 0)
                        _ = sb.AppendLine();

                    _ = sb.Append(' ', indentCount);
                }

                if (is16Bit)
                {
                    _ = sb.Append(BitConverter.ToInt16(Indices, off)).Append(" ");

                    off += 2;
                }
                else
                {
                    _ = sb.Append(BitConverter.ToInt32(Indices, off)).Append(" ");

                    off += 4;
                }
            }

            indentCount -= 2;

            _ = sb.AppendLine().Append(' ', indentCount).AppendLine("]");
        }

        return sb.ToString();
    }

    public string DumpString(int indentCount = 0)
    {
        StringBuilder sb = new();

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(VertexHolder)}()");
        _ = sb.Append(' ', indentCount).AppendLine("{");

        indentCount += 2;

        for (int i = 0; i < ArrayCount; ++i)
            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Counts)}[{i}] = {Counts[i]}");

        for (int i = 0; i < ArrayCount; ++i)
            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Formats)}[{i}] = 0x{Formats[i]:X8}");

        for (int i = 0; i < ArrayCount; ++i)
            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(UVFormats)}[{i}] = 0x{UVFormats[i]:X8}");

        for (int i = 0; i < ArrayCount; ++i)
            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(ArrayOffsets)}[{i}] = {ArrayOffsets[i]}");

        for (int i = 0; i < ArrayCount; ++i)
            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(ArraySizes)}[{i}] = {ArraySizes[i]}");

        for (int i = 0; i < ArrayCount; ++i)
            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Sizes)}[{i}] = {Sizes[i]}");

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(IndexArrayOffset)} = {IndexArrayOffset}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(IndexArraySize)} = {IndexArraySize}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(SomeFlags)} = 0x{SomeFlags:X8}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(ArrayCount)} = {ArrayCount}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(IndexCount)} = {IndexCount}");

        for (int i = 0; i < Decl1.Count; ++i)
        {
            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Decl1)}[{i}] = {Decl1[i]}");
        }

        for (int i = 0; i < Decl2.Count; ++i)
        {
            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Decl2)}[{i}] = {Decl2[i]}");
        }

        _ = sb.Append(' ', indentCount).AppendLine("Vertexes =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        _ = sb.Append(DumpVertices(indentCount + 2));

        _ = sb.Append(' ', indentCount).AppendLine("]");

        indentCount -= 2;

        _ = sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
