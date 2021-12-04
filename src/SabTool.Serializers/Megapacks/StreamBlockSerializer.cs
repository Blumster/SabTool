using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Megapacks
{
    using Data.Packs;
    using Json.Converters;
    using Utils;
    using Utils.Extensions;

    public static class StreamBlockSerializer
    {
        [Flags]
        public enum SerializationFlags
        {
            None = 0,
            EntriesForOnlyIndex2 = 1
        }

        public static StreamBlock DeserializeFromMapRaw(Stream stream, SerializationFlags flags = SerializationFlags.None)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            var streamBlock = new StreamBlock
            {
                Id = new(reader.ReadUInt32()),
                FileName = reader.ReadStringWithMaxLength(reader.ReadInt16())
            };

            streamBlock.Extents[0] = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            streamBlock.Extents[1] = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            streamBlock.UnkShort = reader.ReadInt16();
            streamBlock.Index = reader.ReadInt16();

            if (streamBlock.Index == 2 || (flags & SerializationFlags.EntriesForOnlyIndex2) == SerializationFlags.None)
            {
                ReadTextureInfo(streamBlock, reader);
                ReadEntries(streamBlock, reader);
            }

            return streamBlock;
        }

        private static void ReadTextureInfo(StreamBlock streamBlock, BinaryReader reader)
        {
            streamBlock.TextureCount = reader.ReadUInt32();
            if (streamBlock.TextureCount > 0)
            {
                streamBlock.TextureInfoArray = new StreamBlock.TextureInfo[streamBlock.TextureCount];

                for (var i = 0; i < streamBlock.TextureCount; ++i)
                {
                    streamBlock.TextureInfoArray[i] = new StreamBlock.TextureInfo
                    {
                        Crc = new(reader.ReadUInt32()),
                        UncompressedSize = reader.ReadUInt32()
                    };

                    streamBlock.TotalTextureSize += streamBlock.TextureInfoArray[i].UncompressedSize;
                }
            }

            streamBlock.TextureCount2 = reader.ReadUInt32();
            if (streamBlock.TextureCount2 > 0)
            {
                streamBlock.TextureInfoArray2 = new StreamBlock.TextureInfo[streamBlock.TextureCount2];

                for (var i = 0; i < streamBlock.TextureCount2; ++i)
                {
                    streamBlock.TextureInfoArray2[i] = new StreamBlock.TextureInfo
                    {
                        Crc = new(reader.ReadUInt32()),
                        UncompressedSize = reader.ReadUInt32()
                    };

                    streamBlock.TotalTextureSize += streamBlock.TextureInfoArray2[i].UncompressedSize;
                }
            }
        }

        private static void ReadEntries(StreamBlock streamBlock, BinaryReader reader)
        {
            _ = reader.ReadInt32();

            for (var i = 0; i < streamBlock.EntryCounts.Length; ++i)
                streamBlock.EntryCounts[i] = reader.ReadUInt32();

            streamBlock.Count1ACFor1B0And1B4_1AC = reader.ReadUInt32();
            if (streamBlock.Count1ACFor1B0And1B4_1AC > 0)
            {
                streamBlock.Array1B0 = new Crc[streamBlock.Count1ACFor1B0And1B4_1AC];
                streamBlock.Array1B4 = new byte[streamBlock.Count1ACFor1B0And1B4_1AC];

                for (var i = 0; i < streamBlock.Count1ACFor1B0And1B4_1AC; ++i)
                {
                    streamBlock.Array1B0[i] = new(reader.ReadUInt32());
                    streamBlock.Array1B4[i] = 0;
                }
            }

            streamBlock.PaletteCount = (ushort)reader.ReadInt32();
            for (var i = 0; i < streamBlock.PaletteCount; ++i)
            {
                streamBlock.Palettes[i] = new Crc(reader.ReadUInt32());
            }

            streamBlock.FenceTreeCount = reader.ReadInt32();
            for (var i = 0; i < streamBlock.FenceTreeCount; ++i)
            {
                var crc = new Crc(reader.ReadUInt32());
                var subCnt = reader.ReadInt32();

                streamBlock.FenceTree.Add(crc, reader.ReadConstArray(subCnt, reader.ReadUInt32));
            }
        }

        public static void SerialzieRaw(StreamBlock streamBlock, Stream stream)
        {

        }

        public static StreamBlock DeserializeJSON(Stream stream)
        {
            return null;
        }

        public static void SerializeJSON(StreamBlock streamBlock, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            writer.Write(JsonConvert.SerializeObject(streamBlock, Formatting.Indented, new CrcConverter()));
        }
    }
}
