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
        var sb = new StringBuilder();

        sb.Append(' ', indentCount).AppendLine($"{nameof(Mesh)}()");
        sb.Append(' ', indentCount).AppendLine("{");

        indentCount += 2;

        sb.Append(' ', indentCount).AppendLine($"{nameof(NumBones)} = {NumBones}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(NumSkeletonRemaps)} = {NumSkeletonRemaps}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field14)} = {Field14}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(NumVertexHolder)} = {NumVertexHolder}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(NumPrimitives)} = {NumPrimitives}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(NumShadows)} = {NumShadows}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field1E)} = {Field1E}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field20)} = {Field20}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field24)} = {Field24}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(NumSegments)} = {NumSegments}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field2A)} = {Field2A}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field2B)} = {Field2B}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field2C)} = {Field2C}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field30)} = {Field30}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field31)} = {Field31}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field32)} = {Field32}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Field33)} = {Field33}");

        if (Skeleton == Skeleton.SingleBoneInstance)
        {
            sb.Append(' ', indentCount).AppendLine($"{nameof(Skeleton)} = SingleBoneInstance");
        }
        else
        {
            sb.Append(' ', indentCount).AppendLine($"{nameof(Skeleton)} =");
            sb.Append(Skeleton.DumpString(indentCount + 2));
        }

        sb.Append(' ', indentCount).AppendLine($"{nameof(SkeletonRemaps)} =");
        sb.Append(' ', indentCount).AppendLine("[");

        for (var i = 0; i < NumSkeletonRemaps; ++i)
        {
            sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            sb.Append(SkeletonRemaps[i].DumpString(indentCount + 4));
        }

        sb.Append(' ', indentCount).AppendLine("]");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Shadows)} =");
        sb.Append(' ', indentCount).AppendLine("[");

        for (var i = 0; i < NumShadows; ++i)
        {
            sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            sb.Append(Shadows[i].DumpString(indentCount + 4));
        }

        sb.Append(' ', indentCount).AppendLine("]");
        sb.Append(' ', indentCount).AppendLine($"{nameof(VertexHolders)} =");
        sb.Append(' ', indentCount).AppendLine("[");

        for (var i = 0; i < NumVertexHolder; ++i)
        {
            sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            sb.Append(VertexHolders[i].DumpString(indentCount + 4));
        }

        sb.Append(' ', indentCount).AppendLine("]");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Primitives)} =");
        sb.Append(' ', indentCount).AppendLine("[");

        for (var i = 0; i < NumPrimitives; ++i)
        {
            sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            sb.Append(Primitives[i].DumpString(indentCount + 4));
        }

        sb.Append(' ', indentCount).AppendLine("]");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Segments)} =");
        sb.Append(' ', indentCount).AppendLine("[");

        for (var i = 0; i < NumSegments; ++i)
        {
            sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            sb.Append(Segments[i].DumpString(indentCount + 4));
        }

        sb.Append(' ', indentCount).AppendLine("]");

        indentCount -= 2;

        sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
