using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Packs.Assets;

using SabTool.Data.Packs.Assets;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class TextureAssetSerializer
{
    public static TextureAsset? DeserializeRaw(Stream stream, Crc name)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var textureAsset = new TextureAsset(name)
        {
            Name = reader.ReadStringFromCharArray(reader.ReadInt32())
        };

        Hash.StringToHash(textureAsset.Name); // save to the lookup table

        if (textureAsset.Name.Contains('~'))
            textureAsset.Name = textureAsset.Name[..textureAsset.Name.IndexOf('~')];

        var fmt = reader.ReadUInt32();
        if (fmt != 0x31545844 && fmt != 0x33545844 && fmt != 0x35545844) // DXT1, DXT3, DXT5
        {
            Console.WriteLine($"Texture ({textureAsset.Name}) has unsupported format: 0x{fmt:X8}");
            return null;
        }

        var flags = reader.ReadInt32();
        var width = reader.ReadInt16();
        var height = reader.ReadInt16();
        var numMipmaps = reader.ReadInt16();
        var dataSize = reader.ReadInt32();
        var numChunks = reader.ReadInt32();

        if (numChunks == 0)
            numChunks = 1;

        textureAsset.DDSFiles = new byte[numChunks][];

        for (var i = 0; i < numChunks; ++i)
        {
            try
            {
                var ddsData = reader.ReadDecompressedBytes(reader.ReadInt32());

                textureAsset.DDSFiles[i] = new byte[128 + ddsData.Length - numMipmaps * 24];

                using var ddsStream = new MemoryStream(textureAsset.DDSFiles[i], true);
                using var ddsWriter = new BinaryWriter(ddsStream);

                var ddsFlags = 0x1007;
                if (numMipmaps > 0)
                    ddsFlags |= 0x20000;

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
                    ddsWriter.Write(0); // dwReserved[11]

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
                Console.WriteLine($"Unknown error while processing DDS in {textureAsset.Name}!");

                textureAsset.DDSFiles[i] = null;
            }
        }

        return textureAsset;
    }

    public static void SerializeRaw(TextureAsset meshAsset, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
    }

    public static TextureAsset? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(TextureAsset textureAsset, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(textureAsset, Formatting.Indented));
    }
}
