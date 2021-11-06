using System;
using System.Collections.Generic;
using System.IO;

namespace SabTool.Data.Packs
{
    using Utils;
    using Utils.Extensions;

    public class GlobalMap
    {
        public uint NumTotalBlocks { get; set; }
        public uint NumStreamBlocks { get; set; }
        public StreamBlock[][] StreamBlockArray { get; set; } = new StreamBlock[2][];
        public Dictionary<Crc, StreamBlock> DynamicBlocks { get; } = new();

        public bool Read(BinaryReader br, string mapFileName)
        {
            if (!br.CheckHeaderString("MAP6", reversed: true))
                return false;

            var mapFileNameWithoutExtension = Path.GetFileNameWithoutExtension(mapFileName);

            NumTotalBlocks = br.ReadUInt32();

            var streamBlockArrayIdx = 0;
            var itrCount = 0u;

            do
            {
                NumStreamBlocks = br.ReadUInt32();

                StreamBlockArray[streamBlockArrayIdx] = new StreamBlock[NumStreamBlocks];

                if (NumStreamBlocks == 0)
                {
                    ++streamBlockArrayIdx;
                    ++itrCount;
                    continue;
                }

                var i = 0;
                uint unkFlagStuff = ((itrCount + 1) & 7) << 10;

                do
                {
                    var blockId = new Crc(br.ReadUInt32());
                    var strLen = br.ReadUInt16();
                    var name = br.ReadStringWithMaxLength(strLen);
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
                    br.BaseStream.Position += 12 * 2;

                    // SKipping two unused shorts
                    br.BaseStream.Position += 2 * 2;

                    streamBlock.Flags = (unkFlagStuff | streamBlock.Flags & 0xFFFFE3FF) & 0xFFFFFF3F;

                    streamBlock.ReadTextureInfo(br);
                    streamBlock.ReadEntries(br);

                    streamBlock.Flags = (streamBlock.Flags & 0xFFFFFFF8) | 0x100;

                    StreamBlockArray[streamBlockArrayIdx][i] = streamBlock;
                }
                while (++i < NumStreamBlocks);

                ++streamBlockArrayIdx;
                ++itrCount;
            }
            while (itrCount < 2);

            LoadBlocksFromMapFile(br, NumTotalBlocks, mapFileNameWithoutExtension, false);

            return true;
        }

        public void LoadBlocksFromMapFile(BinaryReader br, uint count, string mapFileNameWithoutExtension, bool removeExisting)
        {
            if (count <= 0)
                return;

            for (var i = 0; i < count; ++i)
            {
                var blockCrc = new Crc(br.ReadUInt32());

                var strLen = br.ReadUInt16();
                var blockName = "";

                if (strLen > 0)
                    blockName = br.ReadStringWithMaxLength(strLen);

                var fArr1 = new float[3];
                fArr1[0] = br.ReadSingle();
                fArr1[1] = br.ReadSingle();
                fArr1[2] = br.ReadSingle();

                var fArr2 = new float[3];
                fArr2[0] = br.ReadSingle();
                fArr2[1] = br.ReadSingle();
                fArr2[2] = br.ReadSingle();

                br.ReadInt16();

                var v22 = br.ReadInt16();

                if (removeExisting && DynamicBlocks.ContainsKey(blockCrc))
                    DynamicBlocks.Remove(blockCrc);

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
                if (!DynamicBlocks.ContainsKey(blockCrc))
                {
                    DynamicBlocks.Add(blockCrc, streamBlock);
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

                streamBlock.ReadTextureInfo(br);
                streamBlock.ReadEntries(br);
            }

            static float GetMiddlePoint(float f1, float f2) => f1 + ((f2 - f1) / 2.0f);
        }
    
        public StreamBlock GetDynamicBlock(Crc crc)
        {
            if (DynamicBlocks.TryGetValue(crc, out StreamBlock res))
                return res;

            return null;
        }

        public StreamBlock GetStaticBlock(Crc crc)
        {
            for (var i = 0; i < StreamBlockArray.Length; ++i)
            {
                for (var j = 0; j < StreamBlockArray[i].Length; ++j)
                {
                    if (StreamBlockArray[i][j].Id.Value == crc.Value)
                        return StreamBlockArray[i][j];
                }
            }

            return null;
        }
    }
}
