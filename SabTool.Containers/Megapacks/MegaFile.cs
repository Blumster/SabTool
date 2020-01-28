using System;
using System.Collections.Generic;
using System.IO;

namespace SabTool.Containers.Megapacks
{
    using Utils.Extensions;

    public class MegaFile
    {
        public FileEntry[] FileEntries { get; set; }
        public int EntryCount { get; set; }
        public uint[] Array3D8 { get; set; }

        public bool ReadHeader(BinaryReader br)
        {
            if (!br.CheckHeaderString("MP00", reversed: true))
                return false;

            EntryCount = br.ReadInt32();

            FileEntries = new FileEntry[EntryCount];
            Array3D8 = new uint[2 * EntryCount];

            for (var i = 0; i < EntryCount; ++i)
                FileEntries[i] = new FileEntry(br);

            for (var i = 0; i < EntryCount; ++i)
            {
                Array3D8[2 * i] = br.ReadUInt32();
                Array3D8[2 * i + 1] = br.ReadUInt32();
            }

            return true;
        }
    }

    public class FileEntry
    {
        public uint Crc { get; set; }
        public uint Crc2 { get; set; }
        public uint Size { get; set; }
        public long Offset { get; set; }

        public FileEntry(BinaryReader br)
        {
            Crc = br.ReadUInt32();
            Crc2 = br.ReadUInt32();
            Size = br.ReadUInt32();
            Offset = br.ReadInt64();
        }
    }
}
