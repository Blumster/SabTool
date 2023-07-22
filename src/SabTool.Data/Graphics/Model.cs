namespace SabTool.Data.Graphics;

public sealed class Model
{
    public Mesh Mesh { get; set; }

    public Vector3 CullingOffset { get; set; }
    public Vector4 BoxAndRadius { get; set; }
    public int Field68 { get; set; }
    public int Field78 { get; set; }
    public Crc Name { get; set; }
    public int FieldB0 { get; set; }
    public byte FieldB9 { get; set; }
    public byte FieldBB { get; set; }
    public byte FieldBF { get; set; }

    public string DumpString(int indentCount = 0)
    {
        StringBuilder sb = new();

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Model)}({Name})");
        _ = sb.Append(' ', indentCount).AppendLine("{");

        indentCount += 2;

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(CullingOffset)} = {CullingOffset}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(BoxAndRadius)} = {BoxAndRadius}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field68)} = {Field68}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field78)} = {Field78}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(FieldB0)} = {FieldB0}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(FieldB9)} = {FieldB9}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(FieldBB)} = {FieldBB}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(FieldBF)} = {FieldBF}");

        indentCount -= 2;

        _ = sb.Append(' ', indentCount).AppendLine("}");

        _ = sb.AppendLine(Mesh.DumpString(indentCount));

        return sb.ToString();
    }
}
