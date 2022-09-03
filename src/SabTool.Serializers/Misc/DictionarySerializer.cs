using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Misc;

using SabTool.Data.Misc;
using SabTool.Serializers.Json.Converters;

public static class DictionarySerializer
{
    public static Dictionary DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var dict = new Dictionary
        {
            Name = new(reader.ReadUInt32())
        };

        var objectCount = reader.ReadUInt32();
        for (var i = 0; i < objectCount; ++i)
        {
            var obj = new DictionaryObject
            {
                Name = new(reader.ReadUInt32())
            };

            var propertyCount = reader.ReadInt32();
            for (var j = 0; j < propertyCount; ++j)
            {
                var prop = PropertySerializer.DeserializeRaw(reader);

                obj.Properties.Add(prop);
            }

            dict.Objects.Add(obj);
        }

        return dict;
    }

    public static void SerializeRaw(Dictionary dict, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        writer.Write(dict.Name.Value);
        writer.Write(dict.Objects.Count);

        foreach (var obj in dict.Objects)
        {
            writer.Write(obj.Name.Value);
            writer.Write(obj.Properties.Count);

            foreach (var prop in obj.Properties)
                PropertySerializer.SerializeRaw(prop, writer);
        }
            
    }

    public static void SerializeJSON(Dictionary dict, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(dict, Formatting.Indented, 
            new CrcConverter(),
            new DictionaryConverter(),
            new DictionaryObjectConverter(),
            new PropertyConverter()));
    }
}
