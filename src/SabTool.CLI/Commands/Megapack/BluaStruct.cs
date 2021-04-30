using System;
using System.Text;

namespace SabTool.Client
{
    public class BluaStruct
    {
        public int BaseOff { get; set; }
        public byte[] Data { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
        public int Count { get; set; }

        public BluaStruct()
        {
            BaseOff = 0;
            Data = null;
            Offset = 0;
            Size = 0;
            Count = 0;
        }

        public int ReadInt()
        {
            var res = BitConverter.ToInt32(Data, BaseOff + Offset);

            Offset += 4;

            return res;
        }

        public int ReadInt(int offset)
        {
            return BitConverter.ToInt32(Data, BaseOff + offset);
        }

        public uint ReadUInt()
        {
            var res = BitConverter.ToUInt32(Data, BaseOff + Offset);

            Offset += 4;

            return res;
        }

        public uint ReadUInt(int offset)
        {
            return BitConverter.ToUInt32(Data, BaseOff + offset);
        }

        public byte ReadByte()
        {
            var res = Data[BaseOff + Offset];

            Offset += 1;

            return res;
        }

        public string ReadString(int maxLen)
        {
            var sb = new StringBuilder();
            var i = 0;

            for (; i < maxLen; ++i)
            {
                if (Data[BaseOff + Offset + i] == 0)
                {
                    break;
                }

                sb.Append((char)Data[BaseOff + Offset + i]);
            }

            Offset += i + 1;

            return sb.ToString();
        }
    }
}
