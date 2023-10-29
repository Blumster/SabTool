using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabTool.Serializers.Misc;

using SabTool.Data.Misc;
using SabTool.Utils;

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

    public static Property DeserializeWithNameAndSizeRaw(BinaryReader reader, Crc name, int size)
    {
        return new Property
        {
            Name = name,
            Data = reader.ReadBytes(size)
        };
    }

    public static List<Property> DeserializeMultipleRaw(BinaryReader reader, int propertyCount)
    {
        var properties = new List<Property>();

        for (var i = 0; i < propertyCount; ++i)
            properties.Add(DeserializeRaw(reader));

        return properties;
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

    public static void SerializeMultipleRaw(List<Property> properties, BinaryWriter writer)
    {
        foreach (var prop in properties)
            SerializeRaw(prop, writer);
    }
}
