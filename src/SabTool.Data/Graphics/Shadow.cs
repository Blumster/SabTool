using static SabTool.Data.Graphics.Material;

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
        var sb = new StringBuilder();

        sb.Append(' ', indentCount).AppendLine($"{nameof(Shadow)}()");
        sb.Append(' ', indentCount).AppendLine("{");

        indentCount += 2;

        sb.Append(' ', indentCount).AppendLine($"{nameof(UnkSize)} = {UnkSize}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(NumVertices)} = {NumVertices}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(NumTriangles)} = {NumTriangles}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(NumWeights)} = {NumWeights}");
        sb.Append(' ', indentCount).AppendLine($"{nameof(Vertices)} =");
        sb.Append(' ', indentCount).AppendLine("[");

        for (var i = 0; i < NumVertices; ++i)
        {
            sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            sb.Append(Vertices[i].DumpString(indentCount + 4));
        }

        sb.Append(' ', indentCount).AppendLine("]");

        sb.Append(' ', indentCount).AppendLine($"{nameof(Triangles)} =");
        sb.Append(' ', indentCount).AppendLine("[");

        for (var i = 0; i < NumTriangles; ++i)
            sb.Append(' ', indentCount + 2).AppendLine($"{i}: [ {Triangles[i].I1}, {Triangles[i].I2}, {Triangles[i].I3} ]");

        sb.Append(' ', indentCount).AppendLine("]");

        sb.Append(' ', indentCount).AppendLine($"{nameof(Weights)} =");
        sb.Append(' ', indentCount).AppendLine("[");

        for (var i = 0; i < NumWeights; ++i)
        {
            sb.Append(' ', indentCount + 2).AppendLine($"{i}:");
            sb.Append(Weights[i].DumpString(indentCount + 4));
        }

        sb.Append(' ', indentCount).AppendLine("]");

        indentCount -= 2;

        sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }

    public class Vertex
    {
        public Vector4 Position { get; set; }
        public Vector4 Normal { get; set; }
        public Vector4 Unk { get; set; }

        public string DumpString(int indentCount = 0)
        {
            var sb = new StringBuilder();

            sb.Append(' ', indentCount).AppendLine($"{nameof(Vertex)}()");
            sb.Append(' ', indentCount).AppendLine("{");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Position)} = {Position}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Normal)} = {Normal}");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Unk)} = {Unk}");
            sb.Append(' ', indentCount).AppendLine("}");

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
            var sb = new StringBuilder();

            sb.Append(' ', indentCount).AppendLine($"{nameof(Weight)}()");
            sb.Append(' ', indentCount).AppendLine("{");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Bones)} = [ {Bones[0]}, {Bones[1]} ]");
            sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Weights)} = [ {Weights[0]}, {Weights[1]} ]");
            sb.Append(' ', indentCount).AppendLine("}");

            return sb.ToString();
        }
    }
}
