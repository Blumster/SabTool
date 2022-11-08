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
        var sb = new StringBuilder();

        sb.Append(' ', indentCount).AppendLine($"{nameof(Model)}({Name})");
        sb.Append(' ', indentCount).AppendLine("{");

        indentCount += 2;

        sb.Append(' ', indentCount).AppendLine($"{nameof(CullingOffset)} = {CullingOffset}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(BoxAndRadius)} = {BoxAndRadius}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field68)} = {Field68}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field78)} = {Field78}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(FieldB0)} = {FieldB0}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(FieldB9)} = {FieldB9}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(FieldBB)} = {FieldBB}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(FieldBF)} = {FieldBF}");

        indentCount -= 2;

        sb.Append(' ', indentCount).AppendLine("}");

        sb.AppendLine(Mesh.DumpString(indentCount));

        return sb.ToString();
    }
}
