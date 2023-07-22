namespace SabTool.Data.Graphics;

public sealed class Segment
{
    public Mesh Mesh { get; set; }
    public int PrimitiveIndex { get; set; }
    public Primitive Primitive { get; set; }
    public Crc MaterialCrc { get; set; }
    public short BoneIndex { get; set; }
    public short Flags { get; set; }

    public string DumpString(int indentCount = 0)
    {
        StringBuilder sb = new();

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Segment)}()");
        _ = sb.Append(' ', indentCount).AppendLine("{");
        _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(PrimitiveIndex)} = {PrimitiveIndex}");
        _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(MaterialCrc)} = {MaterialCrc}");
        _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(BoneIndex)} = {BoneIndex}");
        _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Flags)} = 0x{Flags:X4}");
        _ = sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
