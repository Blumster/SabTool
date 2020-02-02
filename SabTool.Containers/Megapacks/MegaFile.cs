using System;
using System.Collections.Generic;
using System.IO;

namespace SabTool.Containers.Megapacks
{
    using Utils.Extensions;

    public class MegaFile
    {
        public Dictionary<int, FileEntry> FileEntries { get; set; } = new Dictionary<int, FileEntry>();
        public int EntryCount { get; set; }
        public int[] Array3D8 { get; set; }

        public bool ReadHeader(BinaryReader br)
        {
            if (!br.CheckHeaderString("MP00", reversed: true))
                return false;

            EntryCount = br.ReadInt32();

            for (var i = 0; i < EntryCount; ++i)
            {
                var entry = new FileEntry(br);

                FileEntries.Add(entry.Crc, entry);

                var crc = entry.Crc;
                var crc2 = entry.Crc2;

                string source;
                var block = StreamingManager.GetStreamBlockByCRC(crc, out source);
                if (block != null)
                    Console.WriteLine($"Adding entry: 0x{entry.Crc:X8} => {block.FileName,-45} through {source}");

                block = StreamingManager.GetStreamBlockByCRC(crc2, out source);
                if (block != null)
                {
                    Console.WriteLine($"Adding entry: 0x{crc:X8} => 0x{crc2:X8} => {block.FileName,-45} through {source}");
                }
                else
                    Console.WriteLine($"Adding unknown entry: 0x{crc:X8} => 0x{crc2:X8}");
            }

            Array3D8 = new int[2 * EntryCount];

            for (var i = 0; i < EntryCount; ++i)
            {
                Array3D8[2 * i] = br.ReadInt32();
                Array3D8[2 * i + 1] = br.ReadInt32();

                var crc = Array3D8[2 * i];
                var crc2 = Array3D8[2 * i + 1];

                if (!FileEntries.ContainsKey(crc))
                {
                    Console.WriteLine($"ERROR: 0x{crc:X8} => 0x{crc2:X8} is not a valid fileentry!");
                    continue;
                }

                var entry = FileEntries[crc];
                if (entry.Crc == crc && entry.Crc2 == crc2)
                    continue;

                string source;
                var block = StreamingManager.GetStreamBlockByCRC(crc, out source);
                if (block != null)
                    Console.WriteLine($"Map entry: 0x{crc:X8} => {block.FileName,-45} through {source}");

                block = StreamingManager.GetStreamBlockByCRC(crc2, out source);
                if (block != null)
                    Console.WriteLine($"Map entry: 0x{crc:X8} 0x{crc2:X8} => {block.FileName,-45} through {source}");
                else
                    Console.WriteLine($"Map unknown entry: 0x{crc:X8} => 0x{crc2:X8}");
            }

            return true;
        }
    }

    public class FileEntry
    {
        public int Crc { get; set; }
        public int Crc2 { get; set; }
        public uint Size { get; set; }
        public long Offset { get; set; }

        public FileEntry(BinaryReader br)
        {
            Crc = br.ReadInt32();
            Crc2 = br.ReadInt32();
            Size = br.ReadUInt32();
            Offset = br.ReadInt64();
        }
    }
}
