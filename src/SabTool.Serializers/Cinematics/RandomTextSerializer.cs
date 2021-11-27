using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Cinematics
{
    using Data.Cinematics;
    using Json.Converters;
    using Utils;

    public static class RandomTextSerializer
    {
        private const int Version = 5;

        public static List<RandomText> DeserializeRaw(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            var version = reader.ReadInt32();
            if (version != Version)
                throw new Exception($"Invalid version {version} for RandomText!");

            var textCount = reader.ReadInt32();
            var randomTexts = new List<RandomText>(textCount);

            _ = reader.ReadInt32();

            while (true)
            {
                var tag = reader.ReadUInt32();
                if (tag == 0x43454E44) // CEND
                    break;

                if (tag != 0x52545854) // RTXT
                    continue;

                var randomText = new RandomText
                {
                    Id = new(reader.ReadUInt32()),
                    Flags = RandomTextFlags.EqualProbability
                };

                var previousProbability = 0.0f;

                while (true)
                {
                    tag = reader.ReadUInt32();
                    if (tag == 0x43454E44) // CEND
                        break;

                    if (tag != 0x52414E44) // RAND
                        continue;

                    var text = new Crc(reader.ReadUInt32());
                    var probability = reader.ReadSingle();

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

        public static List<RandomText> DeserializeJSON(Stream stream)
        {
            return null;
        }

        public static void SerializeJSON(List<RandomText> randomTexts, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            writer.Write(JsonConvert.SerializeObject(randomTexts, Formatting.Indented, new CrcConverter()));
        }
    }
}
