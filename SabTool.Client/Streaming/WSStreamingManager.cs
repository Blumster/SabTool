using System;
using System.IO;
using System.Text;

namespace SabTool.Client.Streaming
{
    using Utils.Extensions;
    using Utils.Misc;

    public class WSStreamingManager : Singleton<WSStreamingManager>
    {
        public uint FieldA0 { get; set; }
        public WSStreamBlockNode[][] StreamBlockArray = new WSStreamBlockNode[2][];
        public uint Field1D4 { get; set; }

        public WSStreamingManager()
            : base()
        {
        }

        public bool ReadMap1(BinaryReader br, string mapFileName)
        {
            if (br.CheckHeaderString("MAP6", reversed: true))
                return false;

            var mapFileNameWithoutExtension = Path.GetFileNameWithoutExtension(mapFileName);

            FieldA0 = br.ReadUInt32();

            var streamBlockArrayIdx = 0;
            var itrCount = 0u;

            do
            {
                Field1D4 = br.ReadUInt32();

                StreamBlockArray[streamBlockArrayIdx] = new WSStreamBlockNode[Field1D4];

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

                    var node = StreamBlockArray[streamBlockArrayIdx][i] = new WSStreamBlockNode(crc);
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

                    node.StreamBlock.Flags = (node.StreamBlock.Flags & 0xFFFFFFF8) | 0x100;
                }
                while (++i < Field1D4);

                ++streamBlockArrayIdx;
                ++itrCount;
            }
            while (itrCount < 2);

            LoadBlocksFromMapFile(br, FieldA0, mapFileNameWithoutExtension, false);

            return true;
        }

        public void LoadBlocksFromMapFile(BinaryReader br, uint unkCount, string mapFileNameWithoutExtension, bool unk)
        {
            if (unkCount <= 0)
                return;

            // gibebrish?

            while (true)
            {
                var unkInt = br.ReadUInt32();

                var strLen = br.ReadUInt16();
                var str = br.ReadStringWithMaxLength(strLen);
            }
        }
    }
}
