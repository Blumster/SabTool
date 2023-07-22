using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Graphics;
using SabTool.Serializers.Json.Converters;

namespace SabTool.Serializers.Graphics;
public static class VertexHolderSerializer
{
    public static VertexHolder DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        VertexHolder vertexHolder = new();

        long currentStart = stream.Position;

        stream.Position += 0x18;

        for (int i = 0; i < 4; ++i)
            vertexHolder.Counts[i] = reader.ReadInt32();

        for (int i = 0; i < 4; ++i)
            vertexHolder.Formats[i] = reader.ReadInt32();

        for (int i = 0; i < 4; ++i)
            vertexHolder.UVFormats[i] = reader.ReadInt64();

        for (int i = 0; i < 4; ++i)
            vertexHolder.ArrayOffsets[i] = reader.ReadInt32();

        for (int i = 0; i < 4; ++i)
            vertexHolder.ArraySizes[i] = reader.ReadInt32();

        for (int i = 0; i < 4; ++i)
            vertexHolder.Sizes[i] = reader.ReadByte();

        stream.Position += 0x4;

        vertexHolder.IndexArrayOffset = reader.ReadInt32();
        vertexHolder.IndexArraySize = reader.ReadInt32();
        vertexHolder.SomeFlags = reader.ReadUInt32();
        vertexHolder.ArrayCount = reader.ReadInt32();
        vertexHolder.IndexCount = reader.ReadInt32();

        stream.Position += 0x4;

        if (currentStart + 0x98 != stream.Position)
            throw new Exception($"Under or orver read of the unk4 part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x98}");

        int flags = 0x21;
        byte[] flagBytes = BitConverter.GetBytes(flags);

        vertexHolder.Decl1 = VertexDeclaration.Build(flagBytes, vertexHolder.Formats, vertexHolder.ArrayCount, 0);
        vertexHolder.Decl2 = VertexDeclaration.Build(flagBytes, vertexHolder.Formats, vertexHolder.ArrayCount, 1);

        return vertexHolder;
    }

    public static void DeserializeVerticesRaw(VertexHolder vertexHolder, Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        for (int i = 0; i < vertexHolder.ArrayCount; ++i)
        {
            stream.Position = vertexHolder.ArrayOffsets[i];

            vertexHolder.Vertices[i] = reader.ReadBytes(vertexHolder.ArraySizes[i]);
        }

        stream.Position = vertexHolder.IndexArrayOffset;

        vertexHolder.Indices = reader.ReadBytes(vertexHolder.IndexArraySize);
    }

    public static void SerializeRaw(VertexHolder vertexHolder, Stream stream)
    {

    }

    public static VertexHolder? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(VertexHolder vertexHolder, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(vertexHolder, Formatting.Indented, new CrcConverter()));
    }
}
