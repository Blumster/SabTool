using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Graphics;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Graphics;
public static class ShadowSerializer
{
    public static Shadow DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        Shadow shadow = new();

        long currentStart = stream.Position;

        int dataSize = reader.ReadInt32();

        shadow.UnkSize = reader.ReadInt32();
        shadow.NumVertices = reader.ReadInt32();
        shadow.NumTriangles = reader.ReadInt32();
        shadow.NumWeights = reader.ReadInt32();

        int expectedDataSize = (shadow.NumVertices * 0x30) + (shadow.NumTriangles * 0xC) + (shadow.NumWeights * 0x10);
        if (expectedDataSize != dataSize)
            throw new Exception($"Invalid Shadow data size: {dataSize} | Expected: {expectedDataSize}");

        int expectedSize = 0x2C + shadow.UnkSize + expectedDataSize;

        // Skip the rest of the structure and some unknown data
        stream.Position += 0x18 + shadow.UnkSize;

        shadow.Vertices = new Shadow.Vertex[shadow.NumVertices];
        for (int i = 0; i < shadow.NumVertices; ++i)
        {
            shadow.Vertices[i] = new Shadow.Vertex
            {
                Unk = reader.ReadVector4(),
                Normal = reader.ReadVector4(),
                Position = reader.ReadVector4()
            };
        }

        shadow.Triangles = new Shadow.Index[shadow.NumTriangles];
        for (int i = 0; i < shadow.NumTriangles; ++i)
        {
            shadow.Triangles[i] = new Shadow.Index
            {
                I1 = reader.ReadInt32(),
                I2 = reader.ReadInt32(),
                I3 = reader.ReadInt32()
            };
        }

        shadow.Weights = new Shadow.Weight[shadow.NumWeights];
        for (int i = 0; i < shadow.NumWeights; ++i)
        {
            shadow.Weights[i] = new Shadow.Weight
            {
                Bones = new int[] { reader.ReadInt32(), reader.ReadInt32() },
                Weights = new float[2] { reader.ReadSingle(), reader.ReadSingle() }
            };
        }

        return currentStart + expectedSize != stream.Position
            ? throw new Exception($"Under or orver read of the Shadow part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + expectedSize}")
            : shadow;
    }

    public static void SerializeRaw(Shadow shadow, Stream stream)
    {

    }

    public static Shadow? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Shadow shadow, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(shadow, Formatting.Indented));
    }
}
