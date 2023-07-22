using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Packs;
using SabTool.Utils;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Megapacks;
public static class MegapackSerializer
{
    public static Megapack DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("MP00", reversed: true))
            throw new Exception("Invalid MegaPack header found!");

        Megapack megapack = new()
        {
            FileCount = reader.ReadUInt32()
        };

        for (int i = 0; i < megapack.FileCount; ++i)
        {
            FileEntry entry = DeserializeFileEntryRaw(reader);

            // Store the hashes
            if (!string.IsNullOrEmpty(entry.Name.GetString()))
            {
                _ = Hash.StringToHash($"global\\{entry.Name.GetString()}.dynpack");
                _ = Hash.StringToHash($"global\\{entry.Name.GetString()}.palettepack");
            }

            megapack.FileEntries.Add(entry.Path, entry);
        }

        megapack.BlockPathToNameCrcs = new Tuple<Crc, Crc>[megapack.FileCount];

        for (int i = 0; i < megapack.FileCount; ++i)
        {
            megapack.BlockPathToNameCrcs[i] = new(new(reader.ReadUInt32()), new(reader.ReadUInt32()));

            if (!megapack.FileEntries.ContainsKey(megapack.BlockPathToNameCrcs[i].Item1))
            {
                Console.WriteLine($"ERROR: {megapack.BlockPathToNameCrcs[i].Item1} => {megapack.BlockPathToNameCrcs[i].Item2} is not a valid fileentry!");
                continue;
            }

            FileEntry entry = megapack.FileEntries[megapack.BlockPathToNameCrcs[i].Item1];
            if (entry.Path == megapack.BlockPathToNameCrcs[i].Item1 && entry.Name == megapack.BlockPathToNameCrcs[i].Item2)
            {
                continue;
            }

            Console.WriteLine($"BlockPathToNameCrcs Crc mismatch! {entry.Path} != {megapack.BlockPathToNameCrcs[i].Item1} || {entry.Name} != {megapack.BlockPathToNameCrcs[i].Item2}");
        }

        return megapack;
    }

    private static FileEntry DeserializeFileEntryRaw(BinaryReader reader)
    {
        return new FileEntry
        {
            Path = new(reader.ReadUInt32()),
            Name = new(reader.ReadUInt32()),
            Size = reader.ReadInt32(),
            Offset = reader.ReadInt64()
        };
    }

    public static void SerializeRaw(Megapack globalMegaFile, Stream stream)
    {

    }

    public static Megapack? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Megapack globalMegaFile, Stream stream)
    {
        StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(globalMegaFile));
    }
}
