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

        var nameLength = reader.ReadInt32();
        if (nameLength > 1024)
            return null;

        var textureAsset = new TextureAsset(name)
        {
            Name = reader.ReadStringFromCharArray(nameLength)
        };

        if (textureAsset.Name.Contains('~'))
            textureAsset.Name = textureAsset.Name[..textureAsset.Name.IndexOf('~')];

        Hash.StringToHash(textureAsset.Name); // save to the lookup table

        //Hash.StringToHash($"{textureAsset.Name}_High"); // save to the lookup table the high version

        var fmt = reader.ReadUInt32();
        if (fmt != 0x31545844 && fmt != 0x33545844 && fmt != 0x35545844 && fmt != 0x15) // DXT1, DXT3, DXT5, RGBA32
            throw new Exception($"Texture ({textureAsset.Name}) has unsupported format: 0x{fmt:X8}");

        var flags = reader.ReadInt32();
        var width = reader.ReadInt16();
        var height = reader.ReadInt16();
        var numMipmaps = reader.ReadInt16();
        var dataSize = reader.ReadInt32();
        var numChunks = reader.ReadInt32();

        if (numChunks == 0)
            numChunks = 1;

        byte[] ddsData;

        if (numChunks == 1)
        {
            ddsData = reader.ReadDecompressedBytes(reader.ReadInt32());
        }
        else
        {
            ddsData = new byte[dataSize];
            var off = 0;

            for (var i = 0; i < numChunks; ++i)
            {
                var chunk = reader.ReadDecompressedBytes(reader.ReadInt32());

                Array.Copy(chunk, 0, ddsData, off, chunk.Length);

                off += chunk.Length;
            }
        }

        textureAsset.DDSFile = new byte[128 + ddsData.Length - numMipmaps * 24];

        using var ddsStream = new MemoryStream(textureAsset.DDSFile, true);
        using var ddsWriter = new BinaryWriter(ddsStream);

        var ddsFlags = 0x1007u; // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT
        var ddsSurfaceFlags = 0x1000u; // DDSCAPS_TEXTURE
        if (numMipmaps > 0)
        {
            ddsFlags |= 0x20000u; // DDSD_MIPMAPCOUNT
            ddsSurfaceFlags |= 0x400008u; // DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
        }

        var ddspfFlags = 0x4u;
        var ddspfFourCC = fmt;
        var ddspfRGBBitCount = 0u;
        var ddspfRBitMask = 0u;
        var ddspfGBitMask = 0u;
        var ddspfBBitMask = 0u;
        var ddspfABitMask = 0u;

        if (fmt == 0x15)
        {
            ddspfFlags = 0x41u;
            ddspfFourCC = 0u;
            ddspfRGBBitCount = 32u;
            ddspfRBitMask = 0x00ff0000u;
            ddspfGBitMask = 0x0000ff00u;
            ddspfBBitMask = 0x000000ffu;
            ddspfABitMask = 0xff000000u;
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
            ddsWriter.Write(0); // dwReserved[11]

        // DDS_PIXELFORMAT start
        ddsWriter.Write(0x20); // dwSize
        ddsWriter.Write(ddspfFlags); // dwFlags
        ddsWriter.Write(ddspfFourCC); // dwFourCC
        ddsWriter.Write(ddspfRGBBitCount); // dwRGBBitCount
        ddsWriter.Write(ddspfRBitMask); // dwRBitMask
        ddsWriter.Write(ddspfGBitMask); // dwGBitMask
        ddsWriter.Write(ddspfBBitMask); // dwBBitMask
        ddsWriter.Write(ddspfABitMask); // dwABitMask
                                        // DDS_PIXELFORMAT end

        ddsWriter.Write(ddsSurfaceFlags); // dwSurfaceFlags
        ddsWriter.Write(0); // dwCubemapFlags
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
