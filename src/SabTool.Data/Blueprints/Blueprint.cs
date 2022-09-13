namespace SabTool.Data.Blueprints;

using SabTool.Data.Misc;

public sealed class Blueprint
{
    private const int NamePad = -30;

    public BlueprintType Type { get; }
    public string Name { get; }
    public List<Property> Properties { get; } = new();

    public Blueprint(BlueprintType type, string name)
    {
        Type = type;
        Name = name;
    }

    public Blueprint(BlueprintType type, string name, IEnumerable<Property> properties)
        : this(type, name)
    {
        Properties.AddRange(properties);
    }

    public string Dump()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Blueprint({Type}, {Name}):");

        var allTypes = BlueprintFieldTypes.GetAllTypes(Type);
        if (allTypes == null)
            sb.AppendLine("Unknown blueprint type!");

        foreach (var prop in Properties)
        {
            var name = prop.Name.GetString();
            if (string.IsNullOrEmpty(name))
                name = "UNKNOWN";

            sb.Append($"[{prop.Name.GetHexString()}][{BlueprintFieldTypes.GetRealBlueprintType(Type, prop),NamePad}][{name,NamePad}]: ");

            try
            {
                var value = BlueprintFieldTypes.ReadProperty(Type, prop);
                if (value is not null)
                    sb.AppendLine(value.ToString());
                else
                    sb.AppendLine(FormatEmptyType(prop.Data));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while reading field {prop.Name} from type {BlueprintFieldTypes.GetRealBlueprintType(Type, prop)}!");
                Console.WriteLine(ex);

                sb.AppendLine(FormatEmptyType(prop.Data));
            }
        }    

        return sb.ToString();
    }

    private static string FormatEmptyType(byte[] bytes)
    {
        var bytesStr = BitConverter.ToString(bytes).Replace('-', ' ');
        var guessStr = "";

        if (bytes.Length >= 4)
        {
            var intStr = bytes.Length >= 4 ? BitConverter.ToInt32(bytes, 0).ToString() : "";
            var floatStr = bytes.Length >= 4 ? BitConverter.ToSingle(bytes, 0).ToString("0.00") : "";
            var stringVal = Encoding.UTF8.GetString(bytes);
            var crcVal = bytes.Length >= 4 ? new Crc(BitConverter.ToUInt32(bytes, 0)).ToString() : "";

            for (var i = 0; i < bytes.Length; ++i)
            {
                // Don't check nulltermination
                if (i == bytes.Length - 1 && bytes[i] == 0)
                    break;

                // Check every character to be valid
                if (bytes[i] is < 0x20 or > 0x7E)
                {
                    stringVal = null;
                    break;
                }
            }

            if (string.IsNullOrEmpty(stringVal))
            {
                guessStr = $" (I: {intStr,-15} | F: {floatStr,-15} | Crc: {crcVal})";
            }
            else
            {
                if (stringVal[^1] == '\0')
                    stringVal = stringVal[0..^1];

                guessStr = $" (I: {intStr,-15} | F: {floatStr,-15} | S: {stringVal,-15} | Crc: {crcVal})";
            }
        }

        return $"{bytesStr,-30}{guessStr}";
    }
}
