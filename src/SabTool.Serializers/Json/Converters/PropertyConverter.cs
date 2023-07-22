using Newtonsoft.Json;

using SabTool.Data.Misc;

namespace SabTool.Serializers.Json.Converters;
internal class PropertyConverter : JsonConverter<Property>
{
    public override Property? ReadJson(JsonReader reader, Type objectType, Property? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, Property? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        object? data = DictionaryPropertyTypes.ConvertData(value.Name, value.Data);

        serializer.Serialize(writer, data ?? BitConverter.ToString(value.Data));
    }
}
