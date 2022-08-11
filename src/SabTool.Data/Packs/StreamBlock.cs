using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SabTool.Data.Packs
{
    using Assets;
    using Utils;
    using Utils.Extensions;

    public class StreamBlock
    {
        public static readonly uint[] OffIndices = new uint[9] { 6, 7, 0, 2, 8, 4, 3, 1, 5 };

        public string FileName { get; set; }
        public Crc[] Palettes { get; } = new Crc[32];
        public Vector3 Midpoint { get; set; }
        public float FieldC0 { get; set; }
        public Vector3[] Extents { get; } = new Vector3[2];
        public uint TotalTextureSize { get; set; }
        public uint CountF4 { get; set; }
        public uint CountF8 { get; set; }
        public float FloatFC { get; set; }
        public float Float100 { get; set; }
        public uint Flags { get; set; } // 0x8: dynpack? 0x10: no extension? 0x100: palettepack? no 0x8 and no 0x100: pack?
        public byte[] Array104 { get; set; }
        public uint TextureCount { get; set; }
        public TextureInfo[] TextureInfoArray { get; set; }
        public uint TextureCount2 { get; set; }
        public TextureInfo[] TextureInfoArray2 { get; set; }
        public uint[] EntryCounts { get; } = new uint[9];
        public Entry[][] Entries { get; } = new Entry[9][];
        public Dictionary<Crc, uint[]> FenceTree { get; } = new();
        public uint Count1ACFor1B0And1B4_1AC { get; set; }
        public Crc[] Array1B0 { get; set; }
        public byte[] Array1B4 { get; set; }
        public Crc Id { get; set; }
        public ushort PaletteCount { get; set; }
        public int FenceTreeCount { get; set; }
        public short UnkShort { get; set; }
        public short Index { get; set; }
        public uint HeaderEnd { get; set; }


        public void Read(BinaryReader reader)
        {
            if (!reader.CheckHeaderString("SBLA", reversed: true))
                throw new Exception("Invalid file fourcc found!");

            ReadHeader(reader);
        }

        public void Export(string outputPath)
        {
            var offInd = 0;

            do
            {
                var off = OffIndices[offInd];

                if (EntryCounts[off] == 0)
                {
                    ++offInd;
                    continue;
                }

                for (var i = 0; i < EntryCounts[off]; ++i)
                {
                    var entry = Entries[off][i];

                    var extension = "bin";
                    IStreamBlockAsset export = null;

                    switch (off)
                    {
                        case 0:
                            export = new MeshAsset(entry.Crc);
                            break;

                        case 1:
                            export = new TextureAsset(entry.Crc);
                            break;

                        case 2:
                            extension = "physics";
                            break;

                        case 3:
                            extension = "pathgraph";
                            Console.WriteLine("PATHGRAPH! {0}", string.IsNullOrWhiteSpace(entry.Crc.GetString()) ? $"0x{entry.Crc.Value:X8}.{extension}" : $"{entry.Crc.GetString()}.{extension}");
                            break;

                        case 4:
                            extension = "aifence";
                            Console.WriteLine("AIFENCE! {0}", string.IsNullOrWhiteSpace(entry.Crc.GetString()) ? $"0x{entry.Crc.Value:X8}.{extension}" : $"{entry.Crc.GetString()}.{extension}");
                            break;

                        case 5:
                            extension = "unknown";
                            Console.WriteLine("UNK! {0}", string.IsNullOrWhiteSpace(entry.Crc.GetString()) ? $"0x{entry.Crc.Value:X8}.{extension}" : $"{entry.Crc.GetString()}.{extension}");
                            break;

                        case 6:
                            extension = "soundbank";
                            Console.WriteLine("SOUNDBANK! {0}", string.IsNullOrWhiteSpace(entry.Crc.GetString()) ? $"0x{entry.Crc.Value:X8}.{extension}" : $"{entry.Crc.GetString()}.{extension}");
                            break;

                        case 7:
                            extension = "gfx";
                            break;

                        case 8:
                            extension = "wsd";
                            Console.WriteLine("WSD! {0}", string.IsNullOrWhiteSpace(entry.Crc.GetString()) ? $"0x{entry.Crc.Value:X8}.{extension}" : $"{entry.Crc.GetString()}.{extension}");
                            break;
                    }

                    if (export != null)
                    {
                        try
                        {
                            export.Read(new MemoryStream(entry.Payload, false));
                            export.Export(outputPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                        continue;
                    }

                    var outputFilePath = Path.Combine(outputPath, $"{entry.Crc.GetStringOrHexString()}.{extension}");

                    Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

                    File.WriteAllBytes(outputFilePath, entry.Payload);
                }

                ++offInd;
            }
            while (offInd < 9);
        }

        public void ReadHeader(BinaryReader reader)
        {
            HeaderEnd = reader.ReadUInt32();

            if (HeaderEnd > 0)
                ReadHeaderData(reader);

            HeaderEnd += 4;

            var offInd = 0;

            do
            {
                var off = OffIndices[offInd];

                if (EntryCounts[off] == 0)
                {
                    ++offInd;
                    continue;
                }

                var entries = new Entry[EntryCounts[off]];

                for (var i = 0; i < entries.Length; ++i)
                {
                    entries[i] = new Entry(reader);
                }

                Entries[off] = entries;

                HeaderEnd += 24 * EntryCounts[off];

                ++offInd;
            }
            while (offInd < 9);

            HeaderEnd += 4;

            offInd = 0;
            do
            {
                var off = OffIndices[offInd];

                if (EntryCounts[off] == 0)
                {
                    ++offInd;
                    continue;
                }

                for (var i = 0; i < EntryCounts[off]; ++i)
                {
                    var entry = Entries[off][i];

                    reader.BaseStream.Position = HeaderEnd + entry.Offset;

                    entry.Payload = reader.ReadBytes(entry.CompressedSize);
                }

                ++offInd;
            }
            while (offInd < 9);
        }

        public void ReadHeaderData(BinaryReader reader)
        {
            Float100 = 1.0f;

            if ((Flags & 0x1C00) == 0x400)
            {
                if (!reader.CheckHeaderString("HEI1", reversed: true))
                    throw new Exception("Invalid magic found while expecting \"HEI1\"");

                CountF4 = reader.ReadUInt32();
                CountF8 = reader.ReadUInt32();

                var floatUnk = reader.ReadSingle();
                FloatFC = reader.ReadSingle();

                Float100 = (floatUnk - FloatFC) / 255.0f;

                Array104 = new byte[CountF4 * CountF8];
            }

            ReadTextureInfo(reader);
            ReadEntries(reader);
        }

        public void ReadTextureInfo(BinaryReader reader)
        {
            if (TextureInfoArray != null)
                TextureInfoArray = null;

            if (TextureInfoArray2 != null)
                TextureInfoArray2 = null;

            TextureCount = reader.ReadUInt32();
            if (TextureCount > 0)
            {
                TextureInfoArray = new TextureInfo[TextureCount];

                for (var i = 0; i < TextureCount; ++i)
                {
                    TextureInfoArray[i] = new();
                    TextureInfoArray[i].Crc = new(reader.ReadUInt32());
                    TextureInfoArray[i].UncompressedSize = reader.ReadUInt32();

                    TotalTextureSize += TextureInfoArray[i].UncompressedSize;
                }
            }

            TextureCount2 = reader.ReadUInt32();
            if (TextureCount2 > 0)
            {
                TextureInfoArray2 = new TextureInfo[TextureCount2];

                for (var i = 0; i < TextureCount2; ++i)
                {
                    TextureInfoArray2[i] = new();
                    TextureInfoArray2[i].Crc = new(reader.ReadUInt32());
                    TextureInfoArray2[i].UncompressedSize = reader.ReadUInt32();

                    TotalTextureSize += TextureInfoArray2[i].UncompressedSize;
                }
            }
        }

        public void ReadEntries(BinaryReader br)
        {
            _ = br.ReadInt32();

            for (var i = 0; i < EntryCounts.Length; ++i)
                EntryCounts[i] = br.ReadUInt32();

            Count1ACFor1B0And1B4_1AC = br.ReadUInt32();
            if (Count1ACFor1B0And1B4_1AC > 0)
            {
                Array1B0 = new Crc[4 * Count1ACFor1B0And1B4_1AC];
                Array1B4 = new byte[Count1ACFor1B0And1B4_1AC];

                for (var i = 0; i < Count1ACFor1B0And1B4_1AC; ++i)
                {
                    Array1B0[i] = new(br.ReadUInt32());
                    Array1B4[i] = 0;
                }
            }

            PaletteCount = (ushort)br.ReadInt32();
            if (PaletteCount > 0)
            {
                for (var i = 0; i < PaletteCount; ++i)
                {
                    Palettes[i] = new Crc(br.ReadUInt32());
                }
            }

            var fenceTreeCount = br.ReadInt32();
            if (fenceTreeCount > 0)
            {
                for (var i = 0; i < fenceTreeCount; ++i)
                {
                    var crc = br.ReadUInt32();
                    var subCnt = br.ReadUInt32();

                    var dataArr = new uint[subCnt + 1];
                    dataArr[0] = subCnt;

                    for (var j = 1; j <= subCnt; ++j)
                    {
                        dataArr[j] = br.ReadUInt32();
                    }

                    FenceTree.Add(crc, dataArr);
                }
            }
        }

        public override string ToString()
        {
            return $"StreamBlock({Id})";
        }

        public class TextureInfo
        {
            public Crc Crc { get; set; }
            public uint UncompressedSize { get; set; }

            public override string ToString()
            {
                return $"TextureInfo({Crc}, {UncompressedSize})";
            }
        }

        public class Entry
        {
            public Crc Crc { get; set; }
            public int Offset { get; set; }
            public int CompressedSize { get; set; }
            public int UncompressedSize { get; set; }
            public byte[] Payload { get; set; }
            public Crc UnkCrc { get; set; }

            public Entry(BinaryReader reader)
            {
                Crc = new(reader.ReadUInt32());
                Offset = reader.ReadInt32();
                CompressedSize = reader.ReadInt32();
                UncompressedSize = reader.ReadInt32();
                _ = reader.ReadInt32();
                UnkCrc = new(reader.ReadUInt32());
            }
        }
    }

    public interface IStreamBlockAsset
    {
        bool Read(MemoryStream reader);
        bool Write(MemoryStream writer);
        void Import(string filePath);
        void Export(string outputPath);
    }
}
