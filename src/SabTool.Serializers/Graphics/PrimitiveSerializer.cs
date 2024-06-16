using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Graphics;

using SabTool.Data.Graphics;
using SabTool.Utils.Extensions;

public static class PrimitiveSerializer
{
    public static Primitive DeserializeRaw(Stream stream, Mesh mesh)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var primitive = new Primitive
        {
            Mesh = mesh
        };

        var currentStart = stream.Position;

        stream.Position += 0x4;

        primitive.ShadowIndex = reader.ReadInt32();
        if (primitive.ShadowIndex >= 0)
            primitive.Shadow = primitive.Mesh.Shadows[primitive.ShadowIndex];

        stream.Position += 0x28;

        primitive.Vector30 = reader.ReadVector4();
        primitive.Vector40 = reader.ReadVector4();

        primitive.VertexHolderIndex = reader.ReadInt32();
        primitive.VertexHolder = primitive.Mesh.VertexHolders[primitive.VertexHolderIndex];

        primitive.Int54 = reader.ReadInt32();
        primitive.IndexStartOffset = reader.ReadInt32();
        primitive.NumVertex = reader.ReadInt32();
        primitive.NumIndices = reader.ReadInt32();

        if (currentStart + 0x64 != stream.Position)
            throw new Exception($"Under or orver read of the primitive part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x64}");

        return primitive;
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
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(primitive, Formatting.Indented));
    }
}
