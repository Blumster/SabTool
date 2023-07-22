namespace SabTool.Data.Graphics;

public sealed class Shadow
{
    public int UnkSize { get; set; }
    public int NumVertices { get; set; }
    public int NumTriangles { get; set; }
    public int NumWeights { get; set; }

    public Vertex[] Vertices { get; set; }
    public Index[] Triangles { get; set; }
    public Weight[] Weights { get; set; }

    public string DumpString(int indentCount = 0)
    {
        StringBuilder sb = new();

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Shadow)}()");
        _ = sb.Append(' ', indentCount).AppendLine("{");

        indentCount += 2;

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(UnkSize)} = {UnkSize}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(NumVertices)} = {NumVertices}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(NumTriangles)} = {NumTriangles}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(NumWeights)} = {NumWeights}");
        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Vertices)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumVertices; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            _ = sb.Append(Vertices[i].DumpString(indentCount + 4));
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Triangles)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumTriangles; ++i)
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}: [ {Triangles[i].I1}, {Triangles[i].I2}, {Triangles[i].I3} ]");

        _ = sb.Append(' ', indentCount).AppendLine("]");

        _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Weights)} =");
        _ = sb.Append(' ', indentCount).AppendLine("[");

        for (int i = 0; i < NumWeights; ++i)
        {
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            _ = sb.Append(Weights[i].DumpString(indentCount + 4));
        }

        _ = sb.Append(' ', indentCount).AppendLine("]");

        indentCount -= 2;

        _ = sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }

    public class Vertex
    {
        public Vector4 Position { get; set; }
        public Vector4 Normal { get; set; }
        public Vector4 Unk { get; set; }

        public string DumpString(int indentCount = 0)
        {
            StringBuilder sb = new();

            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Vertex)}()");
            _ = sb.Append(' ', indentCount).AppendLine("{");
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Position)} = {Position}");
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Normal)} = {Normal}");
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Unk)} = {Unk}");
            _ = sb.Append(' ', indentCount).AppendLine("}");

            return sb.ToString();
        }
    }

    public class Index
    {
        public int I1 { get; set; }
        public int I2 { get; set; }
        public int I3 { get; set; }
    }

    public class Weight
    {
        public int[] Bones { get; set; }
        public float[] Weights { get; set; }

        public string DumpString(int indentCount = 0)
        {
            StringBuilder sb = new();

            _ = sb.Append(' ', indentCount).AppendLine($"{nameof(Weight)}()");
            _ = sb.Append(' ', indentCount).AppendLine("{");
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Bones)} = [ {Bones[0]}, {Bones[1]} ]");
            _ = sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Weights)} = [ {Weights[0]}, {Weights[1]} ]");
            _ = sb.Append(' ', indentCount).AppendLine("}");

            return sb.ToString();
        }
    }
}
