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
