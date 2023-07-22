using System.Text;

using SabTool.Data.Packs.Assets;
using SabTool.Utils;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Packs.Assets;
public static class TextureAssetSerializer
{
    public const int DDSMagic = 0x20534444;
    public const int DDSMagicSize = 4;
    public const int DDSHeaderSize = 128;
    public const int DDSPixelFormatSize = 32;

    public const int MipMapHeaderLengthOffset = 20;
    public const int MipMapHeaderSize = 24;

    public static TextureAsset? DeserializeRaw(Stream stream, Crc name)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        int nameLength = reader.ReadInt32();
        if (nameLength > 1024)
            return null;

        TextureAsset textureAsset = new(name)
        {
            Name = reader.ReadStringFromCharArray(nameLength)
        };

        if (textureAsset.Name.Contains('~'))
            textureAsset.Name = textureAsset.Name[..textureAsset.Name.IndexOf('~')];

        _ = Hash.StringToHash(textureAsset.Name); // save to the lookup table

        //Hash.StringToHash($"{textureAsset.Name}_High"); // save to the lookup table the high version

        uint fmt = reader.ReadUInt32();
        if (fmt is not 0x31545844 and not 0x33545844 and not 0x35545844 and not 0x15 and not 0x1A) // DXT1, DXT3, DXT5, RGBA32, RGBA16
            throw new Exception($"Texture ({textureAsset.Name}) has unsupported format: 0x{fmt:X8}");

        int flags = reader.ReadInt32();
        int width = reader.ReadInt16();
        int height = reader.ReadInt16();
        int numMipmaps = reader.ReadInt16();
        int dataSize = reader.ReadInt32();
        int numChunks = reader.ReadInt32();

        if (numChunks == 0)
            numChunks = 1;

        byte[] ddsMipMapData = numChunks == 1
            ? reader.ReadDecompressedBytes(reader.ReadInt32())
            : new byte[dataSize];

        if (numChunks > 1)
        {
            int off = 0;

            for (int i = 0; i < numChunks; ++i)
            {
                byte[] chunk = reader.ReadDecompressedBytes(reader.ReadInt32());

                Array.Copy(chunk, 0, ddsMipMapData, off, chunk.Length);

                off += chunk.Length;
            }
        }

        textureAsset.DDSFile = new byte[DDSHeaderSize + ddsMipMapData.Length - (numMipmaps * MipMapHeaderSize)];

        using MemoryStream ddsStream = new(textureAsset.DDSFile, true);
        using BinaryWriter ddsWriter = new(ddsStream);

        uint ddsFlags = 0x1007u; // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT
        uint ddsSurfaceFlags = 0x1000u; // DDSCAPS_TEXTURE
        if (numMipmaps > 0)
        {
            ddsFlags |= 0x20000u; // DDSD_MIPMAPCOUNT
            ddsSurfaceFlags |= 0x400008u; // DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
        }

        uint ddspfFlags = 0x4u;
        uint ddspfFourCC = fmt;
        uint ddspfRGBBitCount = 0u;
        uint ddspfRBitMask = 0u;
        uint ddspfGBitMask = 0u;
        uint ddspfBBitMask = 0u;
        uint ddspfABitMask = 0u;

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
        else if (fmt == 0x1A)
        {
            ddspfFlags = 0x41u;
            ddspfFourCC = 0u;
            ddspfRGBBitCount = 16u;
            ddspfRBitMask = 0x00000f00u;
            ddspfGBitMask = 0x000000f0u;
            ddspfBBitMask = 0x0000000fu;
            ddspfABitMask = 0x0000f000u;
        }

        // DDS_HEADER start

        ddsWriter.Write(DDSMagic); // dwMagic
        ddsWriter.Write(DDSHeaderSize - DDSMagicSize); // dwSize
        ddsWriter.Write(ddsFlags); // dwFlags
        ddsWriter.Write(height); // dwHeight
        ddsWriter.Write(width); // dwWidth
        ddsWriter.Write(0); // dwPitchOrLinearSize
        ddsWriter.Write(0); // dwDepth
        ddsWriter.Write(numMipmaps); // dwMipMapCount

        for (int k = 0; k < 11; ++k)
            ddsWriter.Write(0); // dwReserved[11]

        // DDS_PIXELFORMAT start
        ddsWriter.Write(DDSPixelFormatSize); // dwSize
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

        int ddsDataOff = 0;

        for (int j = 0; j < numMipmaps; ++j)
        {
            int ddsDataLength = BitConverter.ToInt32(ddsMipMapData, ddsDataOff + MipMapHeaderLengthOffset);

            ddsWriter.Write(ddsMipMapData, ddsDataOff + MipMapHeaderSize, ddsDataLength);

            ddsDataOff += ddsDataLength + MipMapHeaderSize;
        }

        return textureAsset;
    }

    public static void SerializeRaw(TextureAsset meshAsset, Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        int nameLength = Math.Min(meshAsset.Name.Length, 1024);

        writer.Write(nameLength);
        writer.WriteUtf8StringOn(meshAsset.Name, nameLength);
    }
}
