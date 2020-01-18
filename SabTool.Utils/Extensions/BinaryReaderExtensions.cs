using System;
using System.IO;
using System.Text;

namespace SabTool.Utils.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static string ReadHeaderString(this BinaryReader reader, int length, bool reversed = false)
        {
            var bytes = new byte[length];

            if (reversed)
            {
                for (var i = length - 1; i >= 0; --i)
                    bytes[i] = reader.ReadByte();
            }
            else
            {
                for (var i = 0; i < length; ++i)
                    bytes[i] = reader.ReadByte();
            }

            return Encoding.UTF8.GetString(bytes);
        }

        public static bool CheckHeaderString(this BinaryReader reader, string required, bool reversed = false)
        {
            if (string.IsNullOrEmpty(required))
                return false;

            return reader.ReadHeaderString(required.Length) == required;
        }

        public static string ReadStringWithMaxLength(this BinaryReader reader, int maxLength, bool breakOnNewLine = false)
        {
            var sb = new StringBuilder();
            byte b = 0;
            var length = maxLength;

            if (reader.BaseStream.Position < reader.BaseStream.Length && (b = reader.ReadByte()) != 0)
            {
                do
                {
                    if (length == 0 || (breakOnNewLine && b == 13))
                        break;

                    sb.Append((char)b);
                    --length;

                    if (reader.BaseStream.Position == reader.BaseStream.Length)
                        break;

                    b = reader.ReadByte();
                }
                while (b != 0);
            }

            if (breakOnNewLine && b == 13 && reader.BaseStream.Position < reader.BaseStream.Length)
                reader.ReadByte();

            return sb.ToString();
        }

        public static string ReadStringFromCharArray(this BinaryReader reader, int length)
        {
            var sb = new StringBuilder();

            var charArr = reader.ReadBytes(length);
            for (var i = 0; i < charArr.Length; ++i)
            {
                if (charArr[i] == 0)
                    break;

                sb.Append((char)charArr[i]);
            }

            return sb.ToString();
        }
    }
}
