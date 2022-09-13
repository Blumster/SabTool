using System.Text;

namespace SabTool.Data.Graphics;

public sealed class Unk3
{
    public int UnkSize { get; set; }
    public int NumUnk1 { get; set; }
    public int NumUnk2 { get; set; }
    public int NumUnk3 { get; set; }

    public string DumpString(int indentCount = 0)
    {
        var sb = new StringBuilder();

        sb.Append(' ', indentCount).AppendLine($"{nameof(Unk3)}()");
        sb.Append(' ', indentCount).AppendLine("{");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(UnkSize)} = {UnkSize}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(NumUnk1)} = {NumUnk1}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(NumUnk2)} = {NumUnk2}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(NumUnk3)} = {NumUnk3}");
        sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
