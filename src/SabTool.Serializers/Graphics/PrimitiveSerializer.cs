using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Graphics;

using SabTool.Data.Graphics;

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

        primitive.Unk3Index = reader.ReadInt32();
        if (primitive.Unk3Index >= 0)
            primitive.Unk3 = primitive.Mesh.Unk3s[primitive.Unk3Index];

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
