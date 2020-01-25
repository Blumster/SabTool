using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabTool.Containers.Map
{
    using Utils.Extensions;

    public class GlobalMap : BaseMap
    {
        public int FieldA0 { get; set; }
        public int Field1D4 { get; set; }
        public int Field1D8 { get; set; }
        public StreamBlock[][] StreamBlockArray = new StreamBlock[2][];

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

    public class StreamBlock
    {
        public string FileName { get; set; }
        public float FieldA4 { get; set; }
        public float FieldA8 { get; set; }
        public float FieldAC { get; set; }
        public float FieldC0 { get; set; }
        public float FieldCC { get; set; }
        public float FieldD0 { get; set; }
        public float FieldD4 { get; set; }
        public float FieldDC { get; set; }
        public float FieldE0 { get; set; }
        public float FieldE4 { get; set; }
        public int SomeOffset { get; set; }
        public uint Flags { get; set; }
        public int CountFor288 { get; set; }
        public int[] Array288 { get; set; }
        public int CountFor296 { get; set; }
        public int[] Array296 { get; set; }
        public int[] EntryCounts => new int[9];
        public int Field198_CRC { get; set; }
        public int CountFor432 { get; set; }
        public int CountFor432And436 { get; set; }
        public int[] Array432 { get; set; }
        public int[] Array436 { get; set; }
        public Dictionary<int, int[]> PblTreeULongP => new Dictionary<int, int[]>();

        public StreamBlock()
        {
            Flags = 0xAAAAAAAA;
            Flags &= 0xFFCFFEE7;
            Field198_CRC = -1;

            // ...

            Flags = (Flags & 0xFFF10000) | 0x10000;
        }

        public void SetFileNameFromFormat(string format, params object[] args)
        {
            FileName = string.Format(format, args);
        }

        public void ReadSomeArrays(BinaryReader br)
        {
            if (Array288 != null)
                Array288 = null;

            if (Array296 != null)
                Array296 = null;

            CountFor288 = br.ReadInt32();
            if (CountFor288 > 0)
            {
                Array288 = new int[2 * CountFor288];

                for (var i = 0; i < CountFor288; ++i)
                {
                    Array288[2 * i] = br.ReadInt32();
                    Array288[2 * i + 1] = br.ReadInt32();

                    SomeOffset += Array288[2 * i + 1];
                }
            }

            CountFor296 = br.ReadInt32();
            if (CountFor296 > 0)
            {
                Array296 = new int[2 * CountFor296];

                for (var i = 0; i < CountFor296; ++i)
                {
                    Array296[2 * i] = br.ReadInt32();
                    Array296[2 * i + 1] = br.ReadInt32();

                    SomeOffset += Array296[2 * i + 1];
                }
            }
        }

        public void Sub659F20(BinaryReader br)
        {
            br.ReadInt32();

            for (var i = 0; i < EntryCounts.Length; ++i)
                EntryCounts[i] = br.ReadInt32();

            CountFor432And436 = br.ReadInt32();
            if (CountFor432And436 > 0)
            {
                Array432 = new int[CountFor432And436];
                Array436 = new int[CountFor432And436];

                for (var i = 0; i < CountFor296; ++i)
                {
                    Array432[i] = br.ReadInt32();
                    Array436[i] = 0;
                }
            }

            var unkCnt = 0;

            CountFor432 = br.ReadInt32(); // Only the low 2 bytes are used?
            if (CountFor432 > 0)
            {
                for (var i = 0; i < CountFor432; ++i)
                {
                    var unk = br.ReadInt32();

                    // todo: some shit
                    // ++unkCnt;
                }
            }

            CountFor432 = unkCnt;

            var cnt = br.ReadInt32();
            if (cnt > 0)
            {
                for (var i = 0; i < cnt; ++i)
                {
                    var crc = br.ReadInt32();
                    var subCnt = br.ReadInt32();

                    var dataArr = new int[subCnt + 1];
                    dataArr[0] = subCnt;

                    for (var j = 1; j <= subCnt; ++j)
                    {
                        dataArr[j] = br.ReadInt32();
                    }

                    PblTreeULongP.Add(crc, dataArr);
                }
            }
        }
    }
}
