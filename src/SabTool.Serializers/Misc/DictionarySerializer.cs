using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Misc;
using SabTool.Serializers.Json.Converters;

namespace SabTool.Serializers.Misc;
public static class DictionarySerializer
{
    public static Dictionary DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        Dictionary dict = new()
        {
            Name = new(reader.ReadUInt32())
        };

        uint objectCount = reader.ReadUInt32();
        for (int i = 0; i < objectCount; ++i)
        {
            DictionaryObject obj = new()
            {
                Name = new(reader.ReadUInt32())
            };

            int propertyCount = reader.ReadInt32();
            for (int j = 0; j < propertyCount; ++j)
            {
                Property prop = PropertySerializer.DeserializeRaw(reader);

                obj.Properties.Add(prop);
            }

            dict.Objects.Add(obj);
        }

        return dict;
    }

    public static void SerializeRaw(Dictionary dict, Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        writer.Write(dict.Name.Value);
        writer.Write(dict.Objects.Count);

        foreach (DictionaryObject? obj in dict.Objects)
        {
            writer.Write(obj.Name.Value);
            writer.Write(obj.Properties.Count);

            foreach (Property? prop in obj.Properties)
                PropertySerializer.SerializeRaw(prop, writer);
        }

    }

    public static void SerializeJSON(Dictionary dict, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(dict, Formatting.Indented,
            new CrcConverter(),
            new DictionaryConverter(),
            new DictionaryObjectConverter(),
            new PropertyConverter()));
    }
}
