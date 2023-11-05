using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace SabTool.Utils.Extensions
{
    public static class BinaryWriterExtensions
    {
        public static void WriteHeaderString(this BinaryWriter writer, string text, bool reversed = false)
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            if (reversed)
                Array.Reverse(bytes);

            writer.Write(bytes);
        }
        public static void WriteUtf8String(this BinaryWriter writer, string text)
        {
            writer.WriteUtf8StringOn(text, text.Length);
        }

        public static void WriteUtf8StringOn(this BinaryWriter writer, string value, int len)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(bytes, 0, Math.Min(bytes.Length, len));

            for (var i = 0; i < len - bytes.Length; ++i)
                writer.Write((byte)0);
        }

        public static void WriteUTF8LengthedString(this BinaryWriter writer, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                writer.Write(0);
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(value);

            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        public static void WriteConstArray<T>(this BinaryWriter _, T[] data, int count, Action<T> writerFunction) where T : new()
        {
            for (var i = 0; i < count; ++i)
                writerFunction(data[i]);
        }

        public static void WriteConstArray<T>(this BinaryWriter writer, T[] data, int count, Action<BinaryWriter, T> writerFunction) where T : new()
        {
            for (var i = 0; i < count; ++i)
                writerFunction(writer, data[i]);
        }

        public static void DoAtPosition(this BinaryWriter writer, long position, Action<BinaryWriter> writerFunction)
        {
            var currentPos = writer.BaseStream.Position;

            writer.BaseStream.Position = position;

            writerFunction(writer);

            writer.BaseStream.Position = currentPos;
        }
    }
}
