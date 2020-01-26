using System;
using System.Collections.Generic;
using System.IO;

namespace SabTool.Containers.Megapacks
{
    using Utils.Extensions;

    public class StreamingManager
    {
        public static StreamingManager Instance { get; private set; }
        public int FieldA0 { get; set; }
        public int Field1D4 { get; set; }
        public int Field1D8 { get; set; }
        public StreamBlock[][] StreamBlockArray { get; } = new StreamBlock[2][];
        public StreamBlock[][] Field1CC { get; } = new StreamBlock[2][];
        public Dictionary<int, StreamBlock> FieldDA30 { get; } = new Dictionary<int, StreamBlock>();
        public long PerformanceCounter { get; set; }

        public StreamingManager()
        {
            Instance = this;
        }

        ~StreamingManager()
        {
            Instance = null;
        }

        public void LoadBlocksFromMapFile(BinaryReader br, int unkCount, string mapFileNameWithoutExtension, bool removeExisting)
        {
            if (unkCount <= 0)
                return;

            for (var i = 0; i < unkCount; ++i)
            {
                var blockCrc = br.ReadInt32();

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

                if (removeExisting && FieldDA30.ContainsKey(blockCrc))
                    FieldDA30.Remove(blockCrc);

                var block = new StreamBlock();
                block.Flags |= 8;
                block.Field198_CRC = blockCrc;
                block.SetFileNameFromFormat(@"{0}\{1}", mapFileNameWithoutExtension, blockName);

                // Do not override it. If removeExisting is true, existing ones will be removed
                if (!FieldDA30.ContainsKey(blockCrc))
                    FieldDA30.Add(blockCrc, block);
                else
                    Console.WriteLine($"DEBUG: Trying to add {blockCrc} (${block.FileName}) to dictionary, but it already exists!");

                block.FieldCC = fArr1[0];
                block.FieldD0 = 0.0f;
                block.FieldD4 = fArr1[2];
                block.FieldDC = fArr2[0];
                block.FieldE0 = 0.0f;
                block.FieldE4 = fArr2[2];

                block.Flags = (block.Flags & 0xFFFFE33F) | (uint)((v22 & 7) << 10);

                block.FieldA4 = UnkCalc(fArr1[0], fArr2[0]);
                block.FieldA8 = UnkCalc(0.0f, 0.0f);
                block.FieldAC = UnkCalc(fArr1[2], fArr2[2]);
                block.Flags &= 0xFFFFFEF8;

                block.ReadSomeArrays(br);
                block.Sub659F20(br);
            }
        }

        public int Sub9EE6B0(int crc, bool arrOff)
        {
            if (Field1D8 > 0)
                for (var i = 0; i < Field1D8; ++i)
                    if (Field1CC[arrOff ? 1 : 0][i].Field198_CRC == crc)
                        return Field1D4 + i;

            if (Field1D4 <= 0)
                return -1;

            for (var i = 0; i < Field1D4; ++i)
                if (StreamBlockArray[arrOff ? 1 : 0][i].Field198_CRC == crc)
                    return i;

            return -1;
        }

        public static float UnkCalc(float f1, float f2)
        {
            return f1 + ((f2 - f1) / 2.0f);
        }
    }

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
            if (Field18C == null)
            {
                Field194 = 0;
                Field18C = new int[Entry7Count];
            }

            Field18C[Field194++] = crc;
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

            CountFor432 = br.ReadInt16();
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
    }
}
