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

    public static class GlobalMapSerializer
    {
        public static GlobalMap DeserializeRaw(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            var globalMap = new GlobalMap();

            if (!reader.CheckHeaderString("MAP6", reversed: true))
                throw new Exception("Invalid global Map header found!");

            var mapFileNameWithoutExtension = Path.GetFileNameWithoutExtension("global.map");

            globalMap.NumTotalBlocks = reader.ReadUInt32();

            var streamBlockArrayIdx = 0;
            var itrCount = 0u;

            do
            {
                globalMap.NumStreamBlocks = reader.ReadUInt32();

                globalMap.StreamBlockArray[streamBlockArrayIdx] = new StreamBlock[globalMap.NumStreamBlocks];

                if (globalMap.NumStreamBlocks == 0)
                {
                    ++streamBlockArrayIdx;
                    ++itrCount;
                    continue;
                }

                var i = 0;
                uint unkFlagStuff = ((itrCount + 1) & 7) << 10;

                do
                {
                    var blockId = new Crc(reader.ReadUInt32());
                    var strLen = reader.ReadUInt16();
                    var name = reader.ReadStringWithMaxLength(strLen);
                    var nameWithItr = string.Format("{0}{1}", name, itrCount);

                    var streamBlock = new StreamBlock
                    {
                        Id = blockId,
                        Midpoint = new(0.0f, 0.0f, 0.0f),
                        FieldC0 = BitConverter.ToSingle(BitConverter.GetBytes(0xFFFFFFFF), 0),
                        FileName = $"{mapFileNameWithoutExtension}\\{nameWithItr}"
                    };

                    Hash.StringToHash(name);
                    Hash.StringToHash(nameWithItr);
                    Hash.StringToHash(streamBlock.FileName);

                    streamBlock.Extents[0] = new(-10000.0f, -10000.0f, -10000.0f);
                    streamBlock.Extents[1] = new(10000.0f, 10000.0f, 10000.0f);

                    // Skipping two blocks on unused data
                    reader.BaseStream.Position += 12 * 2;

                    // SKipping two unused shorts
                    reader.BaseStream.Position += 2 * 2;

                    streamBlock.Flags = (unkFlagStuff | streamBlock.Flags & 0xFFFFE3FF) & 0xFFFFFF3F;

                    streamBlock.ReadTextureInfo(reader);
                    streamBlock.ReadEntries(reader);

                    streamBlock.Flags = (streamBlock.Flags & 0xFFFFFFF8) | 0x100;

                    globalMap.StreamBlockArray[streamBlockArrayIdx][i] = streamBlock;
                }
                while (++i < globalMap.NumStreamBlocks);

                ++streamBlockArrayIdx;
                ++itrCount;
            }
            while (itrCount < 2);

            LoadBlocksFromMapFile(globalMap, reader, globalMap.NumTotalBlocks, mapFileNameWithoutExtension, false);

            return globalMap;
        }

        private static void LoadBlocksFromMapFile(GlobalMap globalMap, BinaryReader reader, uint count, string mapFileNameWithoutExtension, bool removeExisting)
        {
            if (count <= 0)
                return;

            for (var i = 0; i < count; ++i)
            {
                var blockCrc = new Crc(reader.ReadUInt32());

                var strLen = reader.ReadUInt16();
                var blockName = "";

                if (strLen > 0)
                    blockName = reader.ReadStringWithMaxLength(strLen);

                var fArr1 = new float[3];
                fArr1[0] = reader.ReadSingle();
                fArr1[1] = reader.ReadSingle();
                fArr1[2] = reader.ReadSingle();

                var fArr2 = new float[3];
                fArr2[0] = reader.ReadSingle();
                fArr2[1] = reader.ReadSingle();
                fArr2[2] = reader.ReadSingle();

                reader.ReadInt16();

                var v22 = reader.ReadInt16();

                if (removeExisting && globalMap.DynamicBlocks.ContainsKey(blockCrc))
                    globalMap.DynamicBlocks.Remove(blockCrc);

                var streamBlock = new StreamBlock
                {
                    Id = blockCrc,
                    FileName = $"{mapFileNameWithoutExtension}\\{blockName}"
                };

                streamBlock.Flags |= 8;

                // Save the string into the lookup table for later use
                Hash.StringToHash(blockName);
                Hash.StringToHash($"{mapFileNameWithoutExtension}\\{blockName}.dynpack");

                // Do not override it. If removeExisting is true, existing ones will be removed
                if (!globalMap.DynamicBlocks.ContainsKey(blockCrc))
                {
                    globalMap.DynamicBlocks.Add(blockCrc, streamBlock);
                }
                else
                {
                    Console.WriteLine($"DEBUG: Trying to add {blockCrc} (${streamBlock.FileName}) to dictionary, but it already exists!");
                }

                streamBlock.Extents[0] = new(fArr1[0], 0.0f, fArr1[2]);
                streamBlock.Extents[1] = new(fArr2[0], 0.0f, fArr2[2]);
                streamBlock.Midpoint = new(GetMiddlePoint(fArr1[0], fArr2[0]), GetMiddlePoint(0.0f, 0.0f), GetMiddlePoint(fArr1[2], fArr2[2]));
                streamBlock.Flags = (streamBlock.Flags & 0xFFFFE33F) | (uint)((v22 & 7) << 10);
                streamBlock.Flags &= 0xFFFFFEF8;

                streamBlock.ReadTextureInfo(reader);
                streamBlock.ReadEntries(reader);
            }

            static float GetMiddlePoint(float f1, float f2) => f1 + ((f2 - f1) / 2.0f);
        }

        public static GlobalMap DeserializeJSON(Stream stream)
        {
            return null;
        }

        public static void SerializeJSON(GlobalMap globalMap, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            writer.Write(JsonConvert.SerializeObject(globalMap, Formatting.Indented, new CrcConverter()));
        }
    }
}
