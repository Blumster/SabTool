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

                FieldA0 = br.ReadInt32();

                var streamBlockArrayIdx = 0;
                var itrCount = 0u;

                do
                {
                    Field1D4 = br.ReadInt32();

                    StreamBlockArray[streamBlockArrayIdx] = new StreamBlock[Field1D4];

                    if (Field1D4 == 0)
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

                        var block = StreamBlockArray[streamBlockArrayIdx][i] = new StreamBlock();
                        block.Field198_CRC = crc;

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

                        block.ReadSomeArrays(br);
                        block.Sub659F20(br);

                        block.Flags = (block.Flags & 0xFFFFFFF8) | 0x100;
                    }
                    while (++i < Field1D4);

                    ++streamBlockArrayIdx;
                    ++itrCount;
                }
                while (itrCount < 2);

                LoadBlocksFromMapFile(br, FieldA0, mapFileNameWithoutExtension, false);
            }
        }

        public void Write(Stream stream)
        {

        }
    }
}
