using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Data.Packs
{
    using Utils.Extensions;

    public class StreamBlock
    {
        public string FileName { get; set; }
        public uint[] Palettes { get; set; } = new uint[32];
        public uint TotalTextureSize { get; set; }
        public uint CountF4 { get; set; }
        public uint CountF8 { get; set; }
        public float FloatFC { get; set; }
        public float Float100 { get; set; }
        public uint Flags { get; set; }
        public byte[] Array104 { get; set; }
        public uint TextureCount { get; set; }
        public TextureInfo[] TextureInfoArray { get; set; }
        public uint CountFor128_124 { get; set; }
        public TextureInfo[] Array128 { get; set; }
        public uint[] EntryCounts { get; } = new uint[9];
        public Dictionary<uint, uint[]> FenceTree { get; } = new();
        public uint Count1ACFor1B0And1B4_1AC { get; set; }
        public uint[] Array1B0 { get; set; }
        public byte[] Array1B4 { get; set; }
        public ushort CountFor1B0_19C { get; set; }


        public void ReadHeaderData(BinaryReader reader)
        {
            Float100 = 1.0f;

            if ((Flags & 0x1C00) == 0x400)
            {
                if (!reader.CheckHeaderString("HEI1", reversed: true))
                    throw new Exception("Invalid magic found while expecting \"HEI1\"");

                CountF4 = reader.ReadUInt32();
                CountF8 = reader.ReadUInt32();

                var floatUnk = reader.ReadSingle();
                FloatFC = reader.ReadSingle();

                Float100 = (floatUnk - FloatFC) / 255.0f;

                Array104 = new byte[CountF4 * CountF8];
            }

            ReadTextureInfo(reader);
            ReadEntries(reader);
        }

        public void ReadTextureInfo(BinaryReader reader)
        {
            if (TextureInfoArray != null)
                TextureInfoArray = null;

            if (Array128 != null)
                Array128 = null;

            TextureCount = reader.ReadUInt32();
            if (TextureCount > 0)
            {
                TextureInfoArray = new TextureInfo[TextureCount];

                for (var i = 0; i < TextureCount; ++i)
                {
                    TextureInfoArray[i].Crc = reader.ReadUInt32();
                    TextureInfoArray[i].UncompressedSize = reader.ReadUInt32();

                    TotalTextureSize += TextureInfoArray[i].UncompressedSize;
                }
            }

            CountFor128_124 = reader.ReadUInt32();
            if (CountFor128_124 > 0)
            {
                Array128 = new TextureInfo[CountFor128_124];

                for (var i = 0; i < CountFor128_124; ++i)
                {
                    Array128[i].Crc = reader.ReadUInt32();
                    Array128[i].UncompressedSize = reader.ReadUInt32();

                    TotalTextureSize += Array128[i].UncompressedSize;
                }
            }
        }

        public void ReadEntries(BinaryReader br)
        {
            _ = br.ReadInt32();

            for (var i = 0; i < EntryCounts.Length; ++i)
                EntryCounts[i] = br.ReadUInt32();

            Count1ACFor1B0And1B4_1AC = br.ReadUInt32();
            if (Count1ACFor1B0And1B4_1AC > 0)
            {
                Array1B0 = new uint[4 * Count1ACFor1B0And1B4_1AC];
                Array1B4 = new byte[Count1ACFor1B0And1B4_1AC];

                for (var i = 0; i < Count1ACFor1B0And1B4_1AC; ++i)
                {
                    Array1B0[i] = br.ReadUInt32();
                    Array1B4[i] = 0;
                }
            }

            //ushort unkCnt = 0;

            CountFor1B0_19C = (ushort)br.ReadInt32();
            if (CountFor1B0_19C > 0)
            {
                //var field24Ind = 0;

                for (var i = 0; i < CountFor1B0_19C; ++i)
                {
                    var streamBlockId = br.ReadInt32();

                    //var v14 = StreamingManager.Instance.Sub9EE6B0(streamBlockId, (Flags & 0x1C00) == 0x800);

                    /*if (CountFor1B0_19C != 0)
                    {
                        var j = 0;
                        var field24Ind2 = 0;

                        for (; j < CountFor1B0_19C; ++j)
                        {
                            if (Palettes[field24Ind2++] == v14)
                                break;
                        }

                        if (j == CountFor1B0_19C)
                        {
                            ++unkCnt;
                            Palettes[field24Ind++] = v14;
                        }
                    }
                    else
                    {
                        ++unkCnt;
                        Palettes[field24Ind++] = v14;
                    }*/
                }
            }

            //CountFor1B0_19C = unkCnt;

            var fenceTreeCount = br.ReadInt32();
            if (fenceTreeCount > 0)
            {
                for (var i = 0; i < fenceTreeCount; ++i)
                {
                    var crc = br.ReadUInt32();
                    var subCnt = br.ReadUInt32();

                    var dataArr = new uint[subCnt + 1];
                    dataArr[0] = subCnt;

                    for (var j = 1; j <= subCnt; ++j)
                    {
                        dataArr[j] = br.ReadUInt32();
                    }

                    FenceTree.Add(crc, dataArr);
                }
            }
        }

        public class TextureInfo
        {
            public uint Crc { get; set; }
            public uint UncompressedSize { get; set; }
        }
    }
}
