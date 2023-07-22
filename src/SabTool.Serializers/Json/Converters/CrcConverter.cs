using System.Globalization;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using SabTool.Utils;

namespace SabTool.Serializers.Json.Converters;
internal class CrcConverter : JsonConverter<Crc>
{
    private static readonly Regex CrcMatchRegex = new(@"^0x([0-9A-F]{8})\s*\([^)]*\)$", RegexOptions.Compiled);

    public bool PreferString { get; }

    public CrcConverter(bool preferString = false)
    {
        PreferString = preferString;
    }

    public override Crc? ReadJson(JsonReader reader, Type objectType, Crc? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string value = reader.ReadAsString()!;
        Match match = CrcMatchRegex.Match(value);
        if (match.Success)
        {
            string hexString = match.Groups[1].Value;

            if (uint.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint crc))
                return new Crc(crc);
        }

        throw new Exception($"CrcConverter can't parse stringified Crc value: {value}!");
    }

    public override void WriteJson(JsonWriter writer, Crc? value, JsonSerializer serializer)
    {
        if (value is Crc crc)
            writer.WriteValue(PreferString ? crc.GetStringOrHexString() : crc.ToString());
    }
}
