﻿using System;
using System.Collections.Generic;
using System.IO;

namespace SabTool.Containers.Megapacks
{
    using Utils.Extensions;

    public class StreamingManager
    {
        public static StreamingManager Instance { get; private set; }
        public int Field90 { get; set; }
        public int FieldA0 { get; set; }
        public StreamBlock[][] Field1CC { get; } = new StreamBlock[2][];
        public int Field1D4 { get; set; }
        public int Field1D8 { get; set; }
        public StreamBlock[][] StreamBlockArray { get; } = new StreamBlock[2][];
        public Dictionary<int, StreamBlock> Field9F7C { get; } = new Dictionary<int, StreamBlock>();
        public Dictionary<int, StreamBlock> Field9FA4 { get; } = new Dictionary<int, StreamBlock>();
        public Dictionary<int, StreamBlock> FieldDA30 { get; } = new Dictionary<int, StreamBlock>();
        public int FieldDA84 { get; set; }
        public long PerformanceCounter { get; set; }

        public StreamingManager()
        {
            Instance = this;
        }

        ~StreamingManager()
        {
            Instance = null;
        }

        public void LoadBlocksFromMapFile(BinaryReader br, int count, string mapFileNameWithoutExtension, bool removeExisting)
        {
            if (count <= 0)
                return;

            for (var i = 0; i < count; ++i)
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

        public void Sub9F3900(BinaryReader br, int count, string mapFileNameWithoutExtension, bool removeExisting)
        {
            if (count <= 0)
                return;

            for (var i = 0; i < count; ++i)
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

                if (v22 >= 2)
                {
                    var block = new StreamBlock();
                    block.ReadSomeArrays(br);
                    block.Sub659F20(br);
                }
            }
        }

        public void Sub9F3BF0(BinaryReader br, int count, string mapFileNameWithoutExtension, bool removeExisting)
        {
            if (count <= 0)
                return;

            for (var i = 0; i < count; ++i)
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

                if (removeExisting && Field9F7C.ContainsKey(blockCrc))
                    Field9F7C.Remove(blockCrc);

                var block = new StreamBlock();
                block.Field198_CRC = blockCrc;
                block.SetFileNameFromFormat(@"{0}\{1}", mapFileNameWithoutExtension, blockName);

                // Do not override it. If removeExisting is true, existing ones will be removed
                if (!Field9F7C.ContainsKey(blockCrc))
                    Field9F7C.Add(blockCrc, block);
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
                block.Flags = (block.Flags & 0xFFFFFFF9) | 1;

                // todo: some check
                if (false)
                    block.Flags |= 4;

                block.Flags &= 0xFFFFFEFF;

                block.ReadSomeArrays(br);
                block.Sub659F20(br);
            }
        }

        public void Sub9F3FA0(BinaryReader br, int count, string mapFileNameWithoutExtension, bool removeExisting)
        {
            if (count <= 0)
                return;

            for (var i = 0; i < count; ++i)
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

                if (removeExisting && Field9FA4.ContainsKey(blockCrc))
                    Field9FA4.Remove(blockCrc);

                var block = new StreamBlock();
                block.Field198_CRC = blockCrc;
                block.SetFileNameFromFormat(@"{0}\{1}", mapFileNameWithoutExtension, blockName);

                // Do not override it. If removeExisting is true, existing ones will be removed
                if (!Field9FA4.ContainsKey(blockCrc))
                    Field9FA4.Add(blockCrc, block);
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
                block.Flags = (block.Flags & 0xFFFFFEFA) | 2;

                block.ReadSomeArrays(br);
                block.Sub659F20(br);
            }
        }

        public static float UnkCalc(float f1, float f2)
        {
            return f1 + ((f2 - f1) / 2.0f);
        }

        public static StreamBlock GetStreamBlockByCRC(int crc, out string source)
        {
            for (var off = 0; off <= 1; ++off)
            {
                if (Instance.StreamBlockArray[off] == null)
                    continue;

                var indLen = Instance.StreamBlockArray[off].Length.ToString().Length;

                for (var i = 0; i < Instance.StreamBlockArray[off].Length; ++i)
                {
                    if (Instance.StreamBlockArray[off][i].Field198_CRC == crc)
                    {
                        source = $"StreamBlockArray[{off}][{string.Format($"{{0,{indLen}}}", i)}]";
                        return Instance.StreamBlockArray[off][i];
                    }
                }
            }

            if (Instance.Field9F7C.ContainsKey(crc))
            {
                source = $"Field9F7C[0x{crc:X8}]";
                return Instance.FieldDA30[crc];
            }

            if (Instance.Field9FA4.ContainsKey(crc))
            {
                source = $"Field9FA4[0x{crc:X8}]";
                return Instance.FieldDA30[crc];
            }

            if (Instance.FieldDA30.ContainsKey(crc))
            {
                source = $"FieldDA30[0x{crc:X8}]";
                return Instance.FieldDA30[crc];
            }

            source = null;
            return null;
        }
    }
}