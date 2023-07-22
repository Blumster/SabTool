using Newtonsoft.Json;

using SabTool.Data.Misc;

namespace SabTool.Serializers.Json.Converters;
internal class DictionaryConverter : JsonConverter<Dictionary>
{
    public override Dictionary? ReadJson(JsonReader reader, Type objectType, Dictionary? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, Dictionary? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();

        writer.WritePropertyName(nameof(value.Name));
        writer.WriteValue(value.Name.GetStringOrHexString());

        writer.WritePropertyName(nameof(value.Objects));
        writer.WriteStartObject();

        foreach (DictionaryObject? obj in value.Objects)
        {
            writer.WritePropertyName(obj.Name.GetStringOrHexString());

            serializer.Serialize(writer, obj, typeof(DictionaryObject));
        }

        writer.WriteEndObject();

        writer.WriteEndObject();
    }
}
