using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Cinematics;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;

namespace SabTool.Serializers.Cinematics;
public static class RandomTextSerializer
{
    private const int Version = 5;

    public static List<RandomText> DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        int version = reader.ReadInt32();
        if (version != Version)
            throw new Exception($"Invalid version {version} for RandomText!");

        int textCount = reader.ReadInt32();
        List<RandomText> randomTexts = new(textCount);

        _ = reader.ReadInt32();

        while (true)
        {
            uint tag = reader.ReadUInt32();
            if (tag == 0x43454E44) // CEND
                break;

            if (tag != 0x52545854) // RTXT
                continue;

            RandomText randomText = new()
            {
                Id = new(reader.ReadUInt32()),
                Flags = RandomTextFlags.EqualProbability
            };

            float previousProbability = 0.0f;

            while (true)
            {
                tag = reader.ReadUInt32();
                if (tag == 0x43454E44) // CEND
                    break;

                if (tag != 0x52414E44) // RAND
                    continue;

                Crc text = new(reader.ReadUInt32());
                float probability = reader.ReadSingle();

                if (randomText.NumTexts == 20)
                    continue;

                if (randomText.NumTexts > 0 && (randomText.Flags & RandomTextFlags.EqualProbability) == RandomTextFlags.EqualProbability && probability != previousProbability)
                    randomText.Flags &= ~RandomTextFlags.EqualProbability;

                previousProbability = probability;

                randomText.Texts.Add(text);
                randomText.Probabilities.Add(randomText.TotalProbability + probability);

                ++randomText.NumTexts;
            }

            randomTexts.Add(randomText);
        }

        return randomTexts;
    }

    public static void SerialzieRaw(List<RandomText> randomTexts, Stream stream)
    {

    }

    public static List<RandomText>? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(List<RandomText> randomTexts, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(randomTexts, Formatting.Indented, new CrcConverter()));
    }
}
