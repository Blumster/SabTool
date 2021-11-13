using System;

using Newtonsoft.Json;

namespace SabTool.Serializers.Converters
{
    using Utils;

    internal class CrcConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Crc);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Crc crc)
                writer.WriteValue(crc.ToString());
        }
    }
}
