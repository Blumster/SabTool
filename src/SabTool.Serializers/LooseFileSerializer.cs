﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Serializers
{
    using Data;
    using Utils.Extensions;

    public static class LooseFileSerializer
    {
        public static ICollection<LooseFile> DeserializeRaw(Stream stream)
        {
            var looseFiles = new List<LooseFile>();

            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            while (stream.Position < stream.Length)
            {
                var crc = reader.ReadUInt32();
                var size = reader.ReadInt32();
                var name = reader.ReadStringFromCharArray(120);

                var dataOffset = stream.Position;

                stream.Position += size;

                looseFiles.Add(new LooseFile(crc, name, dataOffset, size));

                if ((stream.Position % 16) != 0)
                    stream.Position += 16 - (stream.Position % 16);
            }

            return looseFiles;
        }

        public static void SerializeRaw(ICollection<LooseFile> looseFiles, Stream stream)
        {
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

            foreach (var file in looseFiles)
            {
                if (file.Name.Length > 119)
                    throw new Exception("The file path can not be longer than 119 characters!");

                if (string.IsNullOrEmpty(file.FilePath))
                    throw new Exception("The file path is not set!");

                var nameBytes = Encoding.UTF8.GetBytes(file.Name);

                var dataBytes = File.ReadAllBytes(file.FilePath);

                writer.Write(file.Crc);
                writer.Write(dataBytes.Length);
                writer.Write(nameBytes);

                for (var i = 0; i < 119 - nameBytes.Length; ++i)
                    writer.Write((byte)0);

                writer.Write((byte)0);
                writer.Write(dataBytes);

                var remBytes = stream.Position % 16;

                for (var i = 0; remBytes != 0 && i < 16 - remBytes; ++i)
                    writer.Write((byte)0);
            }
        }
    }
}
