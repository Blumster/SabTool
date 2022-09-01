using System.IO;
using System.Text;

namespace SabTool.Serializers.Misc;

using SabTool.Data.Misc;

public static class PropertySerializer
{
    public static Property DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        return DeserializeRaw(reader);
    }

    public static Property DeserializeRaw(BinaryReader reader)
    {
        return new Property
        {
            Name = new(reader.ReadUInt32()),
            Data = reader.ReadBytes(reader.ReadInt32())
        };
    }

    public static void SerializeRaw(Property prop, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        SerializeRaw(prop, writer);
    }

    public static void SerializeRaw(Property prop, BinaryWriter writer)
    {
        writer.Write(prop.Name.Value);
        writer.Write(prop.Data.Length);
        writer.Write(prop.Data);
    }
}
