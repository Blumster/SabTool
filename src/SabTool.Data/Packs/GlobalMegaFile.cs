using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SabTool.Data.Packs
{
    using Utils;
    using Utils.Extensions;

    public class GlobalMegaFile
    {
        public Dictionary<Crc, FileEntry> FileEntries { get; set; } = new();
        public uint FileCount { get; set; }
        public Tuple<Crc, Crc>[] BlockPathToNameCrcs { get; set; }
        public GlobalMap Map { get; }

        public GlobalMegaFile(GlobalMap map)
        {
            Map = map;
        }

        public bool Read(BinaryReader br)
        {
            if (!br.CheckHeaderString("MP00", reversed: true))
                return false;

            FileCount = br.ReadUInt32();

            for (var i = 0; i < FileCount; ++i)
            {
                var entry = new FileEntry(br);

                // Store the hashes
                if (!string.IsNullOrEmpty(entry.Crc2.GetString()))
                {
                    Hash.StringToHash($"global\\{entry.Crc2.GetString()}.dynpack");
                    Hash.StringToHash($"global\\{entry.Crc2.GetString()}.palettepack");
                }

                FileEntries.Add(entry.Crc, entry);
            }

            BlockPathToNameCrcs = new Tuple<Crc, Crc>[FileCount];

            for (var i = 0; i < FileCount; ++i)
            {
                BlockPathToNameCrcs[i] = new(new(br.ReadUInt32()), new(br.ReadUInt32()));

                if (!FileEntries.ContainsKey(BlockPathToNameCrcs[i].Item1))
                {
                    Console.WriteLine($"ERROR: {BlockPathToNameCrcs[i].Item1} => {BlockPathToNameCrcs[i].Item2} is not a valid fileentry!");
                    continue;
                }

                var entry = FileEntries[BlockPathToNameCrcs[i].Item1];
                if (entry.Crc == BlockPathToNameCrcs[i].Item1 && entry.Crc2 == BlockPathToNameCrcs[i].Item2)
                {
                    Console.WriteLine($"BlockPathToNameCrcs Crc mismatch! {entry.Crc} != {BlockPathToNameCrcs[i].Item1} || {entry.Crc2} != {BlockPathToNameCrcs[i].Item2}");
                    continue;
                }
            }

            return true;
        }

        public void Export(BinaryReader reader, string outputPath)
        {
            foreach (var entry in FileEntries)
            {
                // hack, use Streamblocks to determine
                var ext = "dynpack";

                if (outputPath.Contains("palette"))
                    ext = "palettepack";

                var fileName = string.IsNullOrWhiteSpace(entry.Key.GetString()) ? $"global\\0x{entry.Key.Value:X8}.{ext}" : entry.Key.GetString();
                var outputFilePath = Path.Combine(outputPath, fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

                var startOff = reader.BaseStream.Position;

                reader.BaseStream.Position = entry.Value.Offset;

                var data = reader.ReadBytes(entry.Value.Size);

                File.WriteAllBytes(outputFilePath, data);

                reader.BaseStream.Position = startOff;
            }
        }
    }

    public enum FileEntryType
    {
        Mesh = 0,
        Texture = 1,
        Physics = 2,
        PathGraph = 3,
        AIFence = 4,
        Unk5 = 5,
        SoundBank = 6,
        FlashMovie = 7,
        WSD = 8
    }

    public enum FileReadMethod : uint
    {
        Mesh = 0xFE5E3A56u,
        Texture = 0xA40D777Du,
        Physics = 0x4445EA18u,
        PathGraph = 0xE1087B27u,
        AIFence = 0xD3098461u,
        SoundBank = 0xDD62BA1Au,
        FlashMovie = 0xB5D2FE96u,
        WSD = 0x9AB5A351u,
        Unk5 = 0x00000001u,
        Typ2 = 0x00000002u, // SBLA (not yet finished)
        Typ3 = 0x00000003u  // SBLA (finished)
    }

    public class FileEntry
    {
        public Crc Crc { get; set; }
        public Crc Crc2 { get; set; }
        public int Size { get; set; }
        public long Offset { get; set; }

        public FileEntry(BinaryReader br)
        {
            Crc = new(br.ReadUInt32());
            Crc2 = new(br.ReadUInt32());
            Size = br.ReadInt32();
            Offset = br.ReadInt64();
        }
    }
}
