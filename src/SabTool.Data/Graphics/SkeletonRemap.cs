namespace SabTool.Data.Graphics;

public sealed class SkeletonRemap
{
    public int Bone { get; set; }
    public Matrix4x4 BasePose { get; set; }

    public string DumpString(int indentCount = 0)
    {
        StringBuilder sb = new();

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(SkeletonRemap)}()");
        _ = sb.Append(' ', indentCount).AppendLine("{");
        _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Bone)} = {Bone}");
        _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Matrix4x4)} = {BasePose}");
        _ = sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
