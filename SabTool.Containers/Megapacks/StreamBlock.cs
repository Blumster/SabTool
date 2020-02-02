using System;
using System.Collections.Generic;
using System.IO;

namespace SabTool.Containers.Megapacks
{
    public class StreamBlock
    {
        public string FileName { get; set; }
        public int[] Field24 { get; set; } = new int[32];
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
        public int[] EntryCounts { get; } = new int[9];
        public int[] Field18C { get; set; }
        public int Entry7Count { get; set; }
        public int Field194 { get; set; }
        public int Field198_CRC { get; set; }
        public short CountFor432 { get; set; }
        public Dictionary<int, int[]> PblTreeULongP => new Dictionary<int, int[]>();
        public int CountFor432And436 { get; set; }
        public int[] Array432 { get; set; }
        public byte[] Array436 { get; set; }
        public long Field1D0 { get; set; }

        public StreamBlock()
        {
            Flags = 0xAAAAAAAA;
            Flags &= 0xFFCFFEE7;
            Field198_CRC = -1;

            // ...

            Flags = (Flags & 0xFFF10000) | 0x10000;

            for (var i = 0; i < 32; ++i)
                Field24[i] = -1;
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

        public void Sub658580(int crc)
        {
            Sub162DC70(crc);
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
                Array436 = new byte[CountFor432And436];

                for (var i = 0; i < CountFor296; ++i)
                {
                    Array432[i] = br.ReadInt32();
                    Array436[i] = 0;
                }
            }

            short unkCnt = 0;

            CountFor432 = (short)br.ReadInt32();
            if (CountFor432 > 0)
            {
                var field24Ind = 0;

                for (var i = 0; i < CountFor432; ++i)
                {
                    var crc = br.ReadInt32();

                    var v14 = StreamingManager.Instance.Sub9EE6B0(crc, (Flags & 0x1C00) == 0x800);

                    if (CountFor432 != 0)
                    {
                        var j = 0;
                        var field24Ind2 = 0;

                        for (; j < CountFor432; ++j)
                        {
                            if (Field24[field24Ind2++] == v14)
                                break;
                        }

                        if (j == CountFor432)
                        {
                            ++unkCnt;
                            Field24[field24Ind++] = v14;
                        }
                    }
                    else
                    {
                        ++unkCnt;
                        Field24[field24Ind++] = v14;
                    }
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

        public void Sub162DC70(int crc)
        {
            if (Field18C == null)
            {
                Field194 = 0;
                Field18C = new int[Entry7Count];
            }

            Field18C[Field194++] = crc;
        }
    }
}
