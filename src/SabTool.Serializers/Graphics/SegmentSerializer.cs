using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Graphics;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;

namespace SabTool.Serializers.Graphics;
public static class SegmentSerializer
{
    public static Segment DeserializeRaw(Stream stream, Mesh mesh)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        Segment segment = new()
        {
            Mesh = mesh
        };

        long currentStart = stream.Position;

        segment.PrimitiveIndex = reader.ReadInt32();

        segment.Primitive = segment.Mesh.Primitives[segment.PrimitiveIndex];
        segment.MaterialCrc = new Crc(reader.ReadUInt32());

        stream.Position += 0x4;

        segment.BoneIndex = reader.ReadInt16();
        segment.Flags = reader.ReadInt16();

        return currentStart + 0x10 != stream.Position
            ? throw new Exception($"Under or orver read of the Segment part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x10}")
            : segment;
    }

    public static void SerializeRaw(Segment segment, Stream stream)
    {

    }

    public static Segment? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Segment segment, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(segment, Formatting.Indented, new CrcConverter()));
    }
}
