using System;
using System.IO;
using System.Text;

namespace SabTool.Containers.Megapacks.Global
{
    using Utils.Extensions;

    public class Map : StreamingManager
    {
        public void Read(Stream stream)
        {
            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (!br.CheckHeaderString("MAP6", reversed: true))
                    throw new Exception("Invalid FourCC header!");

                var mapFileNameWithoutExtension = Path.GetFileNameWithoutExtension("global.map");

                m_nTotalNumBlocks = br.ReadInt32();

                var streamBlockArrayIdx = 0;
                var itrCount = 0u;

                do
                {
                    m_nPalettes = br.ReadInt32();

                    m_pPalettes[streamBlockArrayIdx] = new StreamBlock[m_nPalettes];

                    if (m_nPalettes == 0)
                    {
                        ++streamBlockArrayIdx;
                        continue;
                    }

                    var i = 0;
                    uint unkFlagStuff = ((itrCount + 1) & 7) << 10;

                    do
                    {
                        var key = br.ReadInt32();
                        var strLen = br.ReadUInt16();
                        var tempName = br.ReadStringWithMaxLength(strLen);
                        var nameWithItr = string.Format("{0}{1}", tempName, itrCount);

                        var block = m_pPalettes[streamBlockArrayIdx][i] = new StreamBlock();
                        block.m_nId = key;

                        // Skipping two blocks on unused data
                        br.BaseStream.Position += 12 * 2;

                        block.SetFileNameFromFormat("{0}\\{1}", mapFileNameWithoutExtension, nameWithItr);

                        // SKipping two unused shorts
                        br.BaseStream.Position += 2 * 2;

                        block.Flags = (unkFlagStuff | block.Flags & 0xFFFFE3FF) & 0xFFFFFF3F;
                        block.FieldA4 = 0.0f;
                        block.FieldA8 = 0.0f;
                        block.FieldAC = 0.0f;
                        block.FieldCC = -10000.0f;
                        block.FieldD0 = -10000.0f;
                        block.FieldD4 = -10000.0f;
                        block.FieldDC = 10000.0f;
                        block.FieldE0 = 10000.0f;
                        block.FieldE4 = 10000.0f;
                        block.FieldC0 = BitConverter.ToSingle(BitConverter.GetBytes(0xFFFFFFFF), 0);

                        block.ReadTextureInfo(br);
                        block.ReadTOCs(br);

                        block.Flags = (block.Flags & 0xFFFFFFF8) | 0x100;
                    }
                    while (++i < m_nPalettes);

                    ++streamBlockArrayIdx;
                    ++itrCount;
                }
                while (itrCount < 2);

                LoadBlocksFromMapFile(br, m_nTotalNumBlocks, mapFileNameWithoutExtension, false);
            }
        }

        public void Write(Stream stream)
        {

        }
    }
}
