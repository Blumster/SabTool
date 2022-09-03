using System;

using Newtonsoft.Json;

namespace SabTool.Serializers.Json.Converters;

using SabTool.Data.Misc;

internal class DictionaryObjectConverter : JsonConverter<DictionaryObject>
{
    public override DictionaryObject? ReadJson(JsonReader reader, Type objectType, DictionaryObject? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, DictionaryObject? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();

        foreach (var prop in value.Properties)
        {
            writer.WritePropertyName(prop.Name.GetStringOrHexString());

            serializer.Serialize(writer, prop, typeof(Property));
        }

        writer.WriteEndObject();
    }
}
