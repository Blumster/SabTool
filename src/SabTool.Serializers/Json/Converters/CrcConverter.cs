using System;
using System.Globalization;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace SabTool.Serializers.Json.Converters;

using SabTool.Utils;

internal class CrcConverter : JsonConverter
{
    private static readonly Regex CrcMatchRegex = new(@"^0x([0-9A-F]{8})\s*\([^)]*\)$", RegexOptions.Compiled);

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Crc);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var value = reader.ReadAsString()!;
        var match = CrcMatchRegex.Match(value);
        if (match.Success)
        {
            var hexString = match.Groups[1].Value;

            if (uint.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint crc))
                return new Crc(crc);
        }

        throw new Exception($"CrcConverter can't parse stringified Crc value: {value}!");
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is Crc crc)
            writer.WriteValue(crc.ToString());
    }
}
