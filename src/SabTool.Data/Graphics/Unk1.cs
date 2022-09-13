namespace SabTool.Data.Graphics;

public sealed class Unk1
{
    public string DumpString(int indentCount = 0)
    {
        var sb = new StringBuilder();

        sb.Append(' ', indentCount).AppendLine($"{nameof(Unk1)}()");
        sb.Append(' ', indentCount).AppendLine("{");
        sb.Append(' ', indentCount + 2).AppendLine("TODO");
        sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
