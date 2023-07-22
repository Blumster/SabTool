using SharpGLTF.Transforms;

namespace SabTool.Data.Graphics;

public sealed class Skeleton
{
    public static Skeleton SingleBoneInstance { get; } = new Skeleton
    {
        NumBones = 1,
        Indices = new short[] { -1 },
        Bones = new Bone[1] { Bone.DefaultBone },
        UnkBasePoses = new AffineTransform[1] { AffineTransform.Identity }
    };

    public int NumBones { get; set; }
    public int SomeSize { get; set; }
    public int Int4 { get; set; }
    public int Int8 { get; set; }
    public int IntC { get; set; }
    public int SomeSize2 { get; set; }
    public int Int14 { get; set; }
    public int Int18 { get; set; }
    public int Int1C { get; set; }
    public int Int20 { get; set; }
    public int Int24 { get; set; }
    public Matrix4x4[] BasePoses { get; set; }
    public AffineTransform[] UnkBasePoses { get; set; }
    public short[] Indices { get; set; }
    public Bone[] Bones { get; set; }

    public string DumpString(int indentCount = 0)
    {
        StringBuilder sb = new();

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Skeleton)}()");
        _ = sb.Append(' ', indentCount).AppendLine("{");

        indentCount += 2;

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(SomeSize)} = {SomeSize}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Int4)} = {Int4}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Int8)} = {Int8}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(IntC)} = {IntC}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(SomeSize2)} = {SomeSize2}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Int14)} = {Int14}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Int18)} = {Int18}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Int1C)} = {Int1C}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Int20)} = {Int20}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Int24)} = {Int24}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(BasePoses)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumBones; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}: {nameof(Matrix4x4)} = {BasePoses[i]}");
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Bones)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumBones; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            _ = sb.Append(Bones[i].DumpString(indentCount + 4));
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(UnkBasePoses)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumBones; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}: {nameof(AffineTransform)} = {{ Scale: {UnkBasePoses[i].Scale} Rotation: {UnkBasePoses[i].Rotation} Translation: {UnkBasePoses[i].Translation} }}");
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Indices)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumBones; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Indices)} = {Indices[i]}");
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");

        indentCount -= 2;

        _ = sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
