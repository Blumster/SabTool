using System.Text;

using SabTool.Data.Misc;
using SabTool.Utils;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers;
public static class LooseFileSerializer
{
    public static ICollection<LooseFile> DeserializeRaw(Stream stream)
    {
        List<LooseFile> looseFiles = new();

        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        while (stream.Position < stream.Length)
        {
            Crc crc = new(reader.ReadUInt32());
            int size = reader.ReadInt32();
            string name = reader.ReadStringFromCharArray(120);

            long dataOffset = stream.Position;

            stream.Position += size;

            looseFiles.Add(new LooseFile(crc, name, dataOffset, size));

            if ((stream.Position % 16) != 0)
                stream.Position += 16 - (stream.Position % 16);
        }

        return looseFiles;
    }

    public static void SerializeRaw(ICollection<LooseFile> looseFiles, Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        foreach (LooseFile file in looseFiles)
        {
            if (file.Name.Length > 119)
                throw new Exception("The file path can not be longer than 119 characters!");

            if (string.IsNullOrEmpty(file.FilePath))
                throw new Exception("The file path is not set!");

            byte[] nameBytes = Encoding.UTF8.GetBytes(file.Name);

            byte[] dataBytes = File.ReadAllBytes(file.FilePath);

            writer.Write(file.Crc.Value);
            writer.Write(dataBytes.Length);
            writer.Write(nameBytes);

            for (int i = 0; i < 119 - nameBytes.Length; ++i)
                writer.Write((byte)0);

            writer.Write((byte)0);
            writer.Write(dataBytes);

            long remBytes = stream.Position % 16;

            for (int i = 0; remBytes != 0 && i < 16 - remBytes; ++i)
                writer.Write((byte)0);
        }
    }
}
