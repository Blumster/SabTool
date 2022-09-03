using System;

using Newtonsoft.Json;

namespace SabTool.Serializers.Json.Converters;

using SabTool.Data.Misc;

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

        var data = DictionaryPropertyTypes.ConvertData(value.Name, value.Data);
        
        serializer.Serialize(writer, data ?? BitConverter.ToString(value.Data));
    }
}
