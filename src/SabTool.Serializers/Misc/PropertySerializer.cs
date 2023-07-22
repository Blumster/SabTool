using System.Text;

using SabTool.Data.Misc;

namespace SabTool.Serializers.Misc;
public static class PropertySerializer
{
    public static Property DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

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

    public static List<Property> DeserializeMultipleRaw(BinaryReader reader, int propertyCount)
    {
        List<Property> properties = new();

        for (int i = 0; i < propertyCount; ++i)
            properties.Add(DeserializeRaw(reader));

        return properties;
    }

    public static void SerializeRaw(Property prop, Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

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
        foreach (Property prop in properties)
            SerializeRaw(prop, writer);
    }
}
