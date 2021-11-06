using System;
using System.IO;

namespace SabTool.Data.Packs.Assets
{
    using Packs;
    using Utils;
    using Utils.Extensions;

    public class TextureAsset : IStreamBlockAsset
    {
        public Crc NameCrc { get; }
        public string Name { get; set; }

        public byte[][] DDSFiles { get; private set; }

        public TextureAsset(Crc nameCrc)
        {
            NameCrc = nameCrc;
        }

        public bool Read(MemoryStream data)
        {
            using var reader = new BinaryReader(data);

            Name = reader.ReadStringFromCharArray(reader.ReadInt32());

            Hash.StringToHash(Name); // save to the lookup table

            if (Name.Contains('~'))
            {
                Name = Name.Substring(0, Name.IndexOf('~'));
            }

            var fmt = reader.ReadUInt32();
            if (fmt != 0x31545844 && fmt != 0x33545844 && fmt != 0x35545844) // DXT1, DXT3, DXT5
            {
                Console.WriteLine($"Texture ({Name}) has unsupported format: 0x{fmt:X8}");
                return false;
            }

            var flags = reader.ReadInt32();
            var width = reader.ReadInt16();
            var height = reader.ReadInt16();
            var numMipmaps = reader.ReadInt16();
            var dataSize = reader.ReadInt32();
            var numChunks = reader.ReadInt32();
            if (numChunks == 0)
            {
                numChunks = 1;
            }

            DDSFiles = new byte[numChunks][];

            for (var i = 0; i < numChunks; ++i)
            {
                try
                {
                    var ddsData = reader.ReadDecompressedBytes(reader.ReadInt32());

                    DDSFiles[i] = new byte[128 + ddsData.Length - numMipmaps * 24];

                    using var ddsStream = new MemoryStream(DDSFiles[i], true);
                    using var ddsWriter = new BinaryWriter(ddsStream);

                    var ddsFlags = 0x1007;
                    if (numMipmaps > 0)
                    {
                        ddsFlags |= 0x20000;
                    }

                    // DDS_HEADER start

                    ddsWriter.Write(0x20534444); // dwMagic
                    ddsWriter.Write(124); // dwSize
                    ddsWriter.Write(ddsFlags); // dwFlags
                    ddsWriter.Write((int)height); // dwHeight
                    ddsWriter.Write((int)width); // dwWidth
                    ddsWriter.Write(0); // dwPitchOrLinearSize
                    ddsWriter.Write(0); // dwDepth
                    ddsWriter.Write((int)numMipmaps); // dwMipMapCount

                    for (var k = 0; k < 11; ++k)
                    {
                        ddsWriter.Write(0); // dwReserved[11]
                    }

                    // DDS_PIXELFORMAT start
                    ddsWriter.Write(0x20); // dwSize
                    ddsWriter.Write(0x4); // dwFlags
                    ddsWriter.Write(fmt); // dwFourCC
                    ddsWriter.Write(0); // dwRGBBitCount
                    ddsWriter.Write(0); // dwRBitMask
                    ddsWriter.Write(0); // dwGBitMask
                    ddsWriter.Write(0); // dwBBitMask
                    ddsWriter.Write(0); // dwABitMask
                    // DDS_PIXELFORMAT end

                    ddsWriter.Write(0x1000); // dwCaps
                    ddsWriter.Write(0); // dwCaps2
                    ddsWriter.Write(0); // dwCaps3
                    ddsWriter.Write(0); // dwCaps4
                    ddsWriter.Write(0); // dwReserved2

                    // DDS_HEADER end

                    var ddsDataOff = 0;

                    for (var j = 0; j < numMipmaps; ++j)
                    {
                        var ddsDataLength = BitConverter.ToInt32(ddsData, ddsDataOff + 20);

                        ddsWriter.Write(ddsData, ddsDataOff + 24, ddsDataLength);

                        ddsDataOff += ddsDataLength + 24;
                    }
                }
                catch
                {
                    DDSFiles[i] = null;
                }
            }

            return true;
        }

        public bool Write(MemoryStream writer)
        {
            throw new NotImplementedException();
        }

        public void Import(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Export(string outputPath)
        {
            if (DDSFiles == null)
            {
                return;
            }

            if (DDSFiles.Length == 1)
            {
                var outputFilePath = Path.Combine(outputPath, $"{Name}.dds");

                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

                File.WriteAllBytes(outputFilePath, DDSFiles[0]);

                return;
            }

            for (var i = 0; i < DDSFiles.Length; ++i)
            {
                if (DDSFiles[i] == null)
                {
                    continue;
                }

                var localName = $"{Name}{i}.dds";
                var outputFilePath = Path.Combine(outputPath, localName);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

                File.WriteAllBytes(outputFilePath, DDSFiles[i]);
            }
        }
    }
}
