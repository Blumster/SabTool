namespace SabTool.Data.Graphics;

public sealed class SkeletonRemap
{
    public int Bone { get; set; }
    public Matrix4x4 BasePose { get; set; }

    public string DumpString(int indentCount = 0)
    {
        var sb = new StringBuilder();

        sb.Append(' ', indentCount).AppendLine($"{nameof(SkeletonRemap)}()");
        sb.Append(' ', indentCount).AppendLine("{");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Bone)} = {Bone}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Matrix4x4)} = {BasePose}");
        sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
