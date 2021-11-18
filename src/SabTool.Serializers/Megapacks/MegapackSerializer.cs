using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Megapacks
{
    using Data.Packs;
    using Utils;
    using Utils.Extensions;

    public static class MegapackSerializer
    {
        public static Megapack DeserializeRaw(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            if (!reader.CheckHeaderString("MP00", reversed: true))
                throw new Exception("Invalid MegaPack header found!");

            var megapack = new Megapack
            {
                FileCount = reader.ReadUInt32()
            };

            for (var i = 0; i < megapack.FileCount; ++i)
            {
                var entry = DeserializeFileEntryRaw(reader);

                // Store the hashes
                if (!string.IsNullOrEmpty(entry.Crc2.GetString()))
                {
                    Hash.StringToHash($"global\\{entry.Crc2.GetString()}.dynpack");
                    Hash.StringToHash($"global\\{entry.Crc2.GetString()}.palettepack");
                }

                megapack.FileEntries.Add(entry.Crc, entry);
            }

            megapack.BlockPathToNameCrcs = new Tuple<Crc, Crc>[megapack.FileCount];

            for (var i = 0; i < megapack.FileCount; ++i)
            {
                megapack.BlockPathToNameCrcs[i] = new(new(reader.ReadUInt32()), new(reader.ReadUInt32()));

                if (!megapack.FileEntries.ContainsKey(megapack.BlockPathToNameCrcs[i].Item1))
                {
                    Console.WriteLine($"ERROR: {megapack.BlockPathToNameCrcs[i].Item1} => {megapack.BlockPathToNameCrcs[i].Item2} is not a valid fileentry!");
                    continue;
                }

                var entry = megapack.FileEntries[megapack.BlockPathToNameCrcs[i].Item1];
                if (entry.Crc == megapack.BlockPathToNameCrcs[i].Item1 && entry.Crc2 == megapack.BlockPathToNameCrcs[i].Item2)
                {
                    continue;
                }

                Console.WriteLine($"BlockPathToNameCrcs Crc mismatch! {entry.Crc} != {megapack.BlockPathToNameCrcs[i].Item1} || {entry.Crc2} != {megapack.BlockPathToNameCrcs[i].Item2}");
            }

            return megapack;
        }

        private static FileEntry DeserializeFileEntryRaw(BinaryReader reader)
        {
            return new FileEntry
            {
                Crc = new(reader.ReadUInt32()),
                Crc2 = new(reader.ReadUInt32()),
                Size = reader.ReadInt32(),
                Offset = reader.ReadInt64()
            };
        }

        public static void SerializeRaw(Megapack globalMegaFile, Stream stream)
        {

        }

        public static Megapack DeserializeJSON(Stream stream)
        {
            return null;
        }

        public static void SerializeJSON(Megapack globalMegaFile, Stream stream)
        {
            var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            writer.Write(JsonConvert.SerializeObject(globalMegaFile));
        }
    }
}
