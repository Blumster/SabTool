using System;
using System.IO;
using System.Text;

namespace SabTool.Client.Pebble
{
    public abstract class PblFile
    {
        public int F4 { get; set; }
        public int F8 { get; set; }

        public PblFile() // 0xDBA400
        {
            F4 = 0;
        }

        #region Virtual Table 0x107BD90
        public abstract long GetFileSize(); // 0x04

        public virtual bool IsEndOfFile() // 0x08, 0x64AF70
        {
            return false;
        }

        public abstract bool IsOpen(); // 0x0C

        public virtual bool Func16() // 0x10, 0xDBAFC0
        {
            return false;
        }

        public abstract void Read(byte[] destination, int bytesToRead); // 0x14

        public abstract bool Write(byte[] source, int bytesToWrite); // 0x18

        public abstract void Seek(long destOffset, SeekOrigin type); // 0x1C

        public virtual void Read2(byte[] dest, int bytesToRead) // 0x20, 0x64AF80
        {

        }

        public abstract long GetOffset(); // 0x24

        public virtual void Close() // 0x28, 0xDBAFD0
        {
        }
        #endregion

        private byte[] ReadHelper(int count)
        {
            byte[] temp = new byte[count];

            Read(temp, count);

            return temp;
        }

        private byte ReadByteHelper()
        {
            return ReadHelper(1)[0];
        }

        public string ReadString(int length)
        {
            byte[] temp = ReadHelper(length);

            return Encoding.UTF8.GetString(temp, 0, length);
        }

        public string ReadStringWithMaxLength(int maxLength, bool breakOnNewLine) // 0xDBB000
        {
            byte b = 0;
            int length = maxLength;
            var sb = new StringBuilder();

            if (!IsEndOfFile() && (b = ReadByteHelper()) != 0)
            {
                do
                {
                    if (length == 0 || (breakOnNewLine && b == 0xD))
                        break;

                    sb.Append((char)b);

                    --length;

                    if (IsEndOfFile())
                        break;

                    b = ReadByteHelper();
                }
                while (b != 0);
            }

            if (breakOnNewLine && b == 0xD && !IsEndOfFile())
            {
                breakOnNewLine = false;
                ReadByteHelper();
            }

            if (length <= 0)
                return string.Empty;

            return sb.ToString();
        }

        public void WriteString(string str) // 0xDBA430
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            Write(bytes, bytes.Length);
        }

        public string ReadFourCC()
        {
            return ReadString(4);
        }

        public float ReadFloat() // 0x430040
        {
            byte[] temp = ReadHelper(4);

            if ((F8 & 1) != 0)
                Array.Reverse(temp);

            return BitConverter.ToSingle(temp, 0);
        }

        public double ReadDouble()
        {
            byte[] temp = ReadHelper(8);

            if ((F8 & 1) != 0)
                Array.Reverse(temp);

            return BitConverter.ToDouble(temp, 0);
        }

        public short ReadShort() // 0x4BB090
        {
            byte[] temp = ReadHelper(2);

            if ((F8 & 1) != 0)
                Array.Reverse(temp);

            return BitConverter.ToInt16(temp, 0);
        }

        public short ReadShort2() // 0x782FA0
        {
            byte[] temp = ReadHelper(2);

            if ((F8 & 1) != 0)
                Array.Reverse(temp);

            return BitConverter.ToInt16(temp, 0);
        }

        public int ReadInt() // 0x49EE70
        {
            byte[] temp = ReadHelper(4);

            if ((F8 & 1) != 0)
                Array.Reverse(temp);

            return BitConverter.ToInt32(temp, 0);
        }

        public uint ReadUInt32() // 0x427CB0
        {
            byte[] temp = ReadHelper(4);

            if ((F8 & 1) != 0)
                Array.Reverse(temp);

            return BitConverter.ToUInt32(temp, 0);
        }

        public ulong ReadUInt64() // 0x6CA430
        {
            byte[] temp = ReadHelper(8);

            if ((F8 & 1) != 0)
                Array.Reverse(temp);

            return BitConverter.ToUInt64(temp, 0);
        }
    }
}
