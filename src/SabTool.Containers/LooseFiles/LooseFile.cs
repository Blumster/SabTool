using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.Containers.LooseFiles
{
    using System.IO;
    using Utils.Extensions;

    public class LooseFile
    {
        public ICollection<LooseFileEntry> Files { get; } = new List<LooseFileEntry>();

        public void AddFile(LooseFileEntry entry)
        {
            Files.Add(entry);
        }

        public void Read(Stream stream)
        {
            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                while (stream.Position < stream.Length)
                {
                    var crc = br.ReadUInt32();
                    var size = br.ReadInt32();
                    var name = br.ReadStringFromCharArray(120);
                    var data = br.ReadBytes(size);

                    Files.Add(new LooseFileEntry(crc, name, data));

                    if ((stream.Position % 16) != 0)
                        stream.Position += 16 - (stream.Position % 16);
                }
            }
        }

        public void Write(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                foreach (var file in Files)
                {
                    if (file.Name.Length > 119)
                        throw new Exception("The file path can not be longer than 119 characters!");

                    var nameBytes = Encoding.UTF8.GetBytes(file.Name);

                    bw.Write(file.Crc);
                    bw.Write(file.Data.Length);
                    bw.Write(nameBytes);

                    for (var i = 0; i < 119 - nameBytes.Length; ++i)
                        bw.Write((byte)0);

                    bw.Write((byte)0);
                    bw.Write(file.Data);

                    var remBytes = stream.Position % 16;

                    for (var i = 0; remBytes != 0 && i < 16 - remBytes; ++i)
                        bw.Write((byte)0);
                }
            }
        }
    }
}
