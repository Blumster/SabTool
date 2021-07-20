using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Data.Packs
{
    using Utils.Extensions;

    public class GlobalMap
    {
        public uint NumTotalBlocks { get; set; }
        public uint NumPalettes { get; set; }

        public bool ReadMap1(BinaryReader br, string mapFileName)
        {
            if (br.CheckHeaderString("MAP6", reversed: true))
                return false;

            var mapFileNameWithoutExtension = Path.GetFileNameWithoutExtension(mapFileName);

            NumTotalBlocks = br.ReadUInt32();

            var streamBlockArrayIdx = 0;
            var itrCount = 0u;

            do
            {
                NumPalettes = br.ReadUInt32();

                //StreamBlockArray[streamBlockArrayIdx] = new WSStreamBlockNode[NumPalettes];

                if (NumPalettes == 0)
                {
                    ++streamBlockArrayIdx;
                    continue;
                }

                var i = 0;
                uint unkFlagStuff = ((itrCount + 1) & 7) << 10;

                do
                {
                    var crc = br.ReadInt32();
                    var strLen = br.ReadUInt16();
                    var tempName = br.ReadStringWithMaxLength(strLen);
                    var nameWithItr = string.Format("{0}{1}", tempName, itrCount);

                    /*var node = StreamBlockArray[streamBlockArrayIdx][i] = new WSStreamBlockNode(crc);
                    node.StreamBlock.Field198_CRC = crc;

                    // Skipping two blocks on unused data
                    br.BaseStream.Position += 12 * 2;

                    node.StreamBlock.SetFileNameFromFormat("{0}\\{1}", mapFileNameWithoutExtension, nameWithItr);

                    // SKipping two unused shorts
                    br.BaseStream.Position += 2 * 2;

                    node.StreamBlock.Flags = (unkFlagStuff | node.StreamBlock.Flags & 0xFFFFE3FF) & 0xFFFFFF3F;
                    node.StreamBlock.FieldA4 = 0.0f;
                    node.StreamBlock.FieldA8 = 0.0f;
                    node.StreamBlock.FieldAC = 0.0f;
                    node.StreamBlock.FieldCC = -10000.0f;
                    node.StreamBlock.FieldD0 = -10000.0f;
                    node.StreamBlock.FieldD4 = -10000.0f;
                    node.StreamBlock.FieldDC = 10000.0f;
                    node.StreamBlock.FieldE0 = 10000.0f;
                    node.StreamBlock.FieldE4 = 10000.0f;
                    node.StreamBlock.FieldC0 = BitConverter.ToSingle(BitConverter.GetBytes(0xFFFFFFFF), 0);

                    node.StreamBlock.ReadSomeArrays(br);
                    node.StreamBlock.Sub659F20(br);

                    node.StreamBlock.Flags = (node.StreamBlock.Flags & 0xFFFFFFF8) | 0x100;*/
                }
                while (++i < NumPalettes);

                ++streamBlockArrayIdx;
                ++itrCount;
            }
            while (itrCount < 2);

            LoadBlocksFromMapFile(br, NumTotalBlocks, mapFileNameWithoutExtension, false);

            return true;
        }

        public void LoadBlocksFromMapFile(BinaryReader br, uint unkCount, string mapFileNameWithoutExtension, bool unk)
        {
            if (unkCount <= 0)
                return;

            // gibberish?

            while (true)
            {
                var unkInt = br.ReadUInt32();

                var strLen = br.ReadUInt16();
                var str = br.ReadStringWithMaxLength(strLen);
            }
        }
    }
}
