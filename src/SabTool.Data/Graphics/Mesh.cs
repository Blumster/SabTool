namespace SabTool.Data.Graphics;

public sealed class Mesh
{
    public Skeleton Skeleton { get; set; }
    public Segment[] Segments { get; set; }
    public Primitive[] Primitives { get; set; }
    public VertexHolder[] VertexHolders { get; set; }
    public SkeletonRemap[] SkeletonRemaps { get; set; }
    public Shadow[] Shadows { get; set; }
    public int NumSkeletonRemapBones { get; set; }
    public int NumBones { get; set; }
    public int NumSkeletonRemaps { get; set; }
    public int Field14 { get; set; }
    public short NumVertexHolder { get; set; }
    public short NumPrimitives { get; set; }
    public short NumShadows { get; set; }
    public short Field1E { get; set; }
    public int Field20 { get; set; }
    public int Field24 { get; set; }
    public short NumSegments { get; set; }
    public byte Field2A { get; set; }
    public byte Field2B { get; set; }
    public int Field2C { get; set; }
    public byte Field30 { get; set; }
    public byte Field31 { get; set; }
    public byte Field32 { get; set; }
    public byte Field33 { get; set; }

    public string DumpString(int indentCount = 0)
    {
        StringBuilder sb = new();

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Mesh)}()");
        _ = sb.Append(' ', indentCount).AppendLine("{");

        indentCount += 2;

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(NumBones)} = {NumBones}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(NumSkeletonRemaps)} = {NumSkeletonRemaps}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field14)} = {Field14}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(NumVertexHolder)} = {NumVertexHolder}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(NumPrimitives)} = {NumPrimitives}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(NumShadows)} = {NumShadows}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field1E)} = {Field1E}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field20)} = {Field20}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field24)} = {Field24}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(NumSegments)} = {NumSegments}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field2A)} = {Field2A}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field2B)} = {Field2B}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field2C)} = {Field2C}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field30)} = {Field30}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field31)} = {Field31}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field32)} = {Field32}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Field33)} = {Field33}");

        if (Skeleton == Skeleton.SingleBoneInstance)
        {
            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Skeleton)} = SingleBoneInstance");
        }
        else
        {
            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Skeleton)} =");
            _ = sb.Append(Skeleton.DumpString(indentCount + 2));
        }

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(SkeletonRemaps)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumSkeletonRemaps; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            _ = sb.Append(SkeletonRemaps[i].DumpString(indentCount + 4));
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Shadows)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumShadows; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            _ = sb.Append(Shadows[i].DumpString(indentCount + 4));
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(VertexHolders)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumVertexHolder; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            _ = sb.Append(VertexHolders[i].DumpString(indentCount + 4));
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Primitives)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumPrimitives; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            _ = sb.Append(Primitives[i].DumpString(indentCount + 4));
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Segments)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumSegments; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            _ = sb.Append(Segments[i].DumpString(indentCount + 4));
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");

        indentCount -= 2;

        _ = sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
