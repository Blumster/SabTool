using System.Numerics;
using System.Text;

using Ionic.Zlib;

using SharpGLTF.Transforms;

namespace SabTool.Utils.Extensions;

public static class BinaryReaderExtensions
{
    public static string ReadHeaderString(this BinaryReader reader, int length, bool reversed = false)
    {
        byte[] bytes = new byte[length];

        if (reversed)
        {
            for (int i = length - 1; i >= 0; --i)
                bytes[i] = reader.ReadByte();
        }
        else
        {
            for (int i = 0; i < length; ++i)
                bytes[i] = reader.ReadByte();
        }

        return Encoding.UTF8.GetString(bytes);
    }

    public static bool CheckHeaderString(this BinaryReader reader, string required, bool reversed = false)
    {
        return !string.IsNullOrEmpty(required) && reader.ReadHeaderString(required.Length, reversed: reversed) == required;
    }

    public static string ReadStringWithMaxLength(this BinaryReader reader, int maxLength, bool breakOnNewLine = false)
    {
        if (maxLength == 0)
            return string.Empty;

        StringBuilder sb = new();
        byte b = 0;
        int length = maxLength;

        if (reader.BaseStream.Position < reader.BaseStream.Length && (b = reader.ReadByte()) != 0)
        {
            do
            {
                if (length == 0 || (breakOnNewLine && b == 13))
                    break;

                _ = sb.Append((char)b);
                --length;

                if (reader.BaseStream.Position == reader.BaseStream.Length)
                    break;

                b = reader.ReadByte();
            }
            while (b != 0);
        }

        if (breakOnNewLine && b == 13 && reader.BaseStream.Position < reader.BaseStream.Length)
            _ = reader.ReadByte();

        return sb.ToString();
    }

    public static string ReadStringFromCharArray(this BinaryReader reader, int length)
    {
        StringBuilder sb = new();

        byte[] charArr = reader.ReadBytes(length);
        for (int i = 0; i < charArr.Length; ++i)
        {
            if (charArr[i] == 0)
                break;

            _ = sb.Append((char)charArr[i]);
        }

        return sb.ToString();
    }

    public static string ReadPrefixedString(this BinaryReader reader)
    {
        // Didn't find a type yet that has less than a 4 byte prefix
        // String is always null terminated with ASCII 7-bit encoding
        uint length = reader.ReadUInt32();
        if (length == 0)
        {
            throw new Exception("String has to be at least one character long (null terminator)");
        }
        // ReadBytes(...) only takes int and no string in the game should be longer than 256 characters, so it just gets checked and cast down to int
        if (length >= int.MaxValue)
        {
            throw new Exception("Exceeding expected string length");
        }
        int correctedLength = (int)length;
        string result = Encoding.UTF8.GetString(reader.ReadBytes(correctedLength - 1));
        // Check termination
        return reader.ReadByte() != 0 ? throw new Exception("String is not correctly terminated") : result;
    }

    public static string ReadUTF8StringOn(this BinaryReader reader, int length)
    {
        byte[] bytes = reader.ReadBytes(length);
        int validBytes = 0;

        for (; validBytes < length; ++validBytes)
        {
            if (bytes[validBytes] == 0)
            {
                break;
            }
        }

        return Encoding.UTF8.GetString(bytes, 0, validBytes);
    }

    public static string ReadUTF16StringOn(this BinaryReader reader, int length)
    {
        int byteLength = length * 2;
        byte[] bytes = reader.ReadBytes(byteLength);
        int validBytes = 0;

        for (; validBytes < byteLength; validBytes += 2)
        {
            if (bytes[validBytes] == 0 && bytes[validBytes + 1] == 0)
            {
                break;
            }
        }

        return Encoding.Unicode.GetString(bytes, 0, validBytes);
    }

    public static byte[] ReadDecompressedBytes(this BinaryReader reader, int compressedLength)
    {
        byte[] sourceBuff = reader.ReadBytes(compressedLength);

        using MemoryStream mStream = new(sourceBuff);
        using ZlibStream unzip = new(mStream, CompressionMode.Decompress);
        using MemoryStream outStream = new();

        unzip.CopyTo(outStream);

        return outStream.ToArray();
    }

    public static Matrix4x4 ReadMatrix4x4(this BinaryReader reader)
    {
        return new Matrix4x4(
            reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
            reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
            reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
            reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()
            );
    }

    public static Vector3 ReadVector3(this BinaryReader reader)
    {
        return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public static Vector4 ReadVector4(this BinaryReader reader)
    {
        return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public static Quaternion ReadQuaternion(this BinaryReader reader)
    {
        return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public static AffineTransform ReadAffineTransform(this BinaryReader reader)
    {
        Vector3 translation = reader.ReadVector3();

        reader.BaseStream.Position += 4;

        Quaternion rotation = reader.ReadQuaternion();
        Vector3 scale = reader.ReadVector3();

        reader.BaseStream.Position += 4;

        return new AffineTransform(scale, rotation, translation);
    }

    public static T[] ReadConstArray<T>(this BinaryReader _, int count, Func<T> readerFunction)
    {
        T[] result = new T[count];

        for (int i = 0; i < count; ++i)
            result[i] = readerFunction();

        return result;
    }

    public static T[] ReadConstArray<T>(this BinaryReader reader, int count, Func<BinaryReader, T> readerFunction)
    {
        T[] result = new T[count];

        for (int i = 0; i < count; ++i)
            result[i] = readerFunction(reader);

        return result;
    }

    public static T ReadWithPosition<T>(this BinaryReader reader, long position, Func<T> readerFunction)
    {
        long currentPosition = reader.BaseStream.Position;

        try
        {
            reader.BaseStream.Position = position;

            return readerFunction();
        }
        finally
        {
            reader.BaseStream.Position = currentPosition;
        }
    }
}
