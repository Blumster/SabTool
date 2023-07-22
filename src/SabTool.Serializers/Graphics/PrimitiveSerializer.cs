using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Graphics;

namespace SabTool.Serializers.Graphics;
public static class PrimitiveSerializer
{
    public static Primitive DeserializeRaw(Stream stream, Mesh mesh)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        Primitive primitive = new()
        {
            Mesh = mesh
        };

        long currentStart = stream.Position;

        stream.Position += 0x4;

        primitive.ShadowIndex = reader.ReadInt32();
        if (primitive.ShadowIndex >= 0)
            primitive.Shadow = primitive.Mesh.Shadows[primitive.ShadowIndex];

        stream.Position += 0x28;

        primitive.Float30 = reader.ReadSingle();
        primitive.Float34 = reader.ReadSingle();
        primitive.Float38 = reader.ReadSingle();
        primitive.Int3C = reader.ReadInt32();
        primitive.Float40 = reader.ReadSingle();
        primitive.Float44 = reader.ReadSingle();
        primitive.Float48 = reader.ReadSingle();
        primitive.Int4C = reader.ReadInt32();

        primitive.VertexHolderIndex = reader.ReadInt32();
        primitive.VertexHolder = primitive.Mesh.VertexHolders[primitive.VertexHolderIndex];

        primitive.Int54 = reader.ReadInt32();
        primitive.IndexStartOffset = reader.ReadInt32();
        primitive.NumVertex = reader.ReadInt32();
        primitive.NumIndices = reader.ReadInt32();

        return currentStart + 0x64 != stream.Position
            ? throw new Exception($"Under or orver read of the primitive part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x64}")
            : primitive;
    }

    public static void SerializeRaw(Primitive primitive, Stream stream)
    {

    }

    public static Primitive? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Primitive primitive, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(primitive, Formatting.Indented));
    }
}
