using System;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Packs.Assets;

using SabTool.Data.Packs.Assets;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class TextureAssetSerializer
{
    public const int DDSMagic = 0x20534444;
    public const int DDSMagicSize = 4;
    public const int DDSHeaderSize = 128;
    public const int DDSPixelFormatSize = 32;
    public const int DDSPixelFormatOffset = 76;

    public const int MipMapHeaderLengthOffset = 20;
    public const int MipMapHeaderSize = 24;

    public const uint FormatDXT1 = 0x31545844;
    public const uint FormatDXT3 = 0x33545844;
    public const uint FormatDXT5 = 0x35545844;
    public const uint FormatRGBA32 = 0x15;
    public const uint FormatRGBA16 = 0x1A;

    private const uint DDSD_PITCH = 0x8u;
    private const uint DDSD_LINEARSIZE = 0x80000u;
    private const uint DDPF_FOURCC = 0x4u;

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

        var originalName = textureAsset.Name;

        if (textureAsset.Name.Contains('~'))
            textureAsset.Name = textureAsset.Name[..textureAsset.Name.IndexOf('~')];

        Hash.StringToHash(textureAsset.Name); // save to the lookup table

        //Hash.StringToHash($"{textureAsset.Name}_High"); // save to the lookup table the high version

        var fmt = reader.ReadUInt32();
        if (!IsSupportedFormat(fmt))
            throw new Exception($"Texture ({textureAsset.Name}) has unsupported format: 0x{fmt:X8}");

        var flags = reader.ReadInt32();
        var width = (int)reader.ReadInt16();
        var height = (int)reader.ReadInt16();
        var numMipmaps = (int)reader.ReadInt16();
        var dataSize = reader.ReadInt32();
        var rawNumChunks = reader.ReadInt32();

        var numChunks = rawNumChunks == 0 ? 1 : rawNumChunks;
        var chunkSizes = new int[numChunks];

        var ddsMipMapData = numChunks == 1
            ? reader.ReadDecompressedBytes(reader.ReadInt32())
            : new byte[dataSize];

        if (numChunks == 1)
        {
            chunkSizes[0] = ddsMipMapData.Length;
        }
        else
        {
            var off = 0;

            for (var i = 0; i < numChunks; ++i)
            {
                var chunk = reader.ReadDecompressedBytes(reader.ReadInt32());

                Array.Copy(chunk, 0, ddsMipMapData, off, chunk.Length);

                chunkSizes[i] = chunk.Length;
                off += chunk.Length;
            }
        }

        var mipHeaders = new byte[numMipmaps][];
        var mipSizes = new int[numMipmaps];
        var mipDataOff = 0;

        for (var j = 0; j < numMipmaps; ++j)
        {
            mipHeaders[j] = ddsMipMapData[mipDataOff..(mipDataOff + MipMapHeaderSize)];
            mipSizes[j] = BitConverter.ToInt32(ddsMipMapData, mipDataOff + MipMapHeaderLengthOffset);

            mipDataOff += mipSizes[j] + MipMapHeaderSize;
        }

        textureAsset.Metadata = new TextureMetadata
        {
            OriginalName = originalName,
            NameLength = nameLength,
            Format = fmt,
            Flags = flags,
            Width = width,
            Height = height,
            MipCount = numMipmaps,
            NumChunks = rawNumChunks,
            ChunkSizes = chunkSizes,
            MipHeaders = mipHeaders
        };

        textureAsset.DDSFile = new byte[DDSHeaderSize + ddsMipMapData.Length - numMipmaps * MipMapHeaderSize];

        using var ddsStream = new MemoryStream(textureAsset.DDSFile, true);
        using var ddsWriter = new BinaryWriter(ddsStream);

        WriteDDSHeader(ddsWriter, fmt, width, height, numMipmaps);

        var ddsDataOff = 0;

        for (var j = 0; j < numMipmaps; ++j)
        {
            ddsWriter.Write(ddsMipMapData, ddsDataOff + MipMapHeaderSize, mipSizes[j]);

            ddsDataOff += mipSizes[j] + MipMapHeaderSize;
        }

        return textureAsset;
    }

    private static void WriteDDSHeader(BinaryWriter ddsWriter, uint fmt, int width, int height, int numMipmaps)
    {
        var ddsFlags = 0x1007u; // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT
        var ddsSurfaceFlags = 0x1000u; // DDSCAPS_TEXTURE
        if (numMipmaps > 0)
        {
            ddsFlags |= 0x20000u; // DDSD_MIPMAPCOUNT
            ddsSurfaceFlags |= 0x400008u; // DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
        }

        var ddspfFlags = DDPF_FOURCC;
        var ddspfFourCC = fmt;
        var ddspfRGBBitCount = 0u;
        var ddspfRBitMask = 0u;
        var ddspfGBitMask = 0u;
        var ddspfBBitMask = 0u;
        var ddspfABitMask = 0u;

        if (fmt == FormatRGBA32)
        {
            ddspfFlags = 0x41u;
            ddspfFourCC = 0u;
            ddspfRGBBitCount = 32u;
            ddspfRBitMask = 0x00ff0000u;
            ddspfGBitMask = 0x0000ff00u;
            ddspfBBitMask = 0x000000ffu;
            ddspfABitMask = 0xff000000u;
        }
        else if (fmt == FormatRGBA16)
        {
            ddspfFlags = 0x41u;
            ddspfFourCC = 0u;
            ddspfRGBBitCount = 16u;
            ddspfRBitMask = 0x00000f00u;
            ddspfGBitMask = 0x000000f0u;
            ddspfBBitMask = 0x0000000fu;
            ddspfABitMask = 0x0000f000u;
        }

        uint pitchOrLinearSize;
        if (IsBlockCompressed(fmt))
        {
            ddsFlags |= DDSD_LINEARSIZE;
            pitchOrLinearSize = (uint)GetMipSize(fmt, width, height);
        }
        else
        {
            ddsFlags |= DDSD_PITCH;
            pitchOrLinearSize = (uint)((width * GetBitsPerPixel(fmt) + 7) / 8);
        }

        // DDS_HEADER start

        ddsWriter.Write(DDSMagic); // dwMagic
        ddsWriter.Write(DDSHeaderSize - DDSMagicSize); // dwSize
        ddsWriter.Write(ddsFlags); // dwFlags
        ddsWriter.Write(height); // dwHeight
        ddsWriter.Write(width); // dwWidth
        ddsWriter.Write(pitchOrLinearSize); // dwPitchOrLinearSize
        ddsWriter.Write(0); // dwDepth
        ddsWriter.Write(numMipmaps); // dwMipMapCount

        for (var k = 0; k < 11; ++k)
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
    }

    public static void SerializeRaw(TextureAsset textureAsset, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        var meta = textureAsset.Metadata;

        var dds = ParseDDS(textureAsset.DDSFile, meta?.MipHeaders?.Select(h => BitConverter.ToInt32(h, MipMapHeaderLengthOffset)).ToArray());

        if (meta != null && (meta.Format != dds.Format || meta.Width != dds.Width || meta.Height != dds.Height || meta.MipCount != dds.MipSizes.Length))
        {
            Console.WriteLine($"Texture ({textureAsset.Name}) differs from its metadata, using the DDS values and default mip headers!");
            meta = new TextureMetadata
            {
                OriginalName = meta.OriginalName,
                NameLength = meta.NameLength,
                Flags = meta.Flags
            };
        }

        // Build the uncompressed mip data blob: [mip header][mip data] for each mip
        var dataSize = dds.MipSizes.Sum() + dds.MipSizes.Length * MipMapHeaderSize;
        var mipMapData = new byte[dataSize];

        var srcOff = DDSHeaderSize;
        var dstOff = 0;

        for (var i = 0; i < dds.MipSizes.Length; ++i)
        {
            if (meta?.MipHeaders != null)
                Array.Copy(meta.MipHeaders[i], 0, mipMapData, dstOff, MipMapHeaderSize);

            BitConverter.TryWriteBytes(mipMapData.AsSpan(dstOff + MipMapHeaderLengthOffset), dds.MipSizes[i]);

            Array.Copy(textureAsset.DDSFile, srcOff, mipMapData, dstOff + MipMapHeaderSize, dds.MipSizes[i]);

            srcOff += dds.MipSizes[i];
            dstOff += dds.MipSizes[i] + MipMapHeaderSize;
        }

        var useMetaChunks = meta?.ChunkSizes != null && meta.ChunkSizes.Sum() == dataSize;
        var chunkSizes = useMetaChunks ? meta!.ChunkSizes : new[] { dataSize };
        var numChunks = useMetaChunks ? meta!.NumChunks : 1;

        var name = meta?.OriginalName ?? textureAsset.Name;
        var nameLength = meta?.NameLength ?? Encoding.UTF8.GetByteCount(name);
        nameLength = Math.Min(nameLength, 1024);

        writer.Write(nameLength);
        writer.WriteUtf8StringOn(name, nameLength);

        writer.Write(dds.Format);
        writer.Write(meta?.Flags ?? 0);
        writer.Write((short)dds.Width);
        writer.Write((short)dds.Height);
        writer.Write((short)dds.MipSizes.Length);
        writer.Write(dataSize);
        writer.Write(numChunks);

        var chunkOff = 0;

        foreach (var chunkSize in chunkSizes)
        {
            writer.WriteCompressedBytes(mipMapData[chunkOff..(chunkOff + chunkSize)]);

            chunkOff += chunkSize;
        }
    }

    public sealed record DDSInfo(uint Format, int Width, int Height, int[] MipSizes);

    public static DDSInfo ParseDDS(byte[] ddsFile, int[]? expectedMipSizes = null)
    {
        if (ddsFile.Length < DDSHeaderSize || BitConverter.ToInt32(ddsFile, 0) != DDSMagic)
            throw new Exception("Invalid DDS file!");

        var height = BitConverter.ToInt32(ddsFile, 12);
        var width = BitConverter.ToInt32(ddsFile, 16);
        var mipCount = Math.Max(1, BitConverter.ToInt32(ddsFile, 28));

        var pfFlags = BitConverter.ToUInt32(ddsFile, DDSPixelFormatOffset + 4);
        var fourCC = BitConverter.ToUInt32(ddsFile, DDSPixelFormatOffset + 8);
        var bitCount = BitConverter.ToUInt32(ddsFile, DDSPixelFormatOffset + 12);

        uint fmt;
        if ((pfFlags & DDPF_FOURCC) != 0)
            fmt = fourCC;
        else if (bitCount == 32)
            fmt = FormatRGBA32;
        else if (bitCount == 16)
            fmt = FormatRGBA16;
        else
            throw new Exception($"Unsupported uncompressed DDS bit count: {bitCount}");

        if (!IsSupportedFormat(fmt))
            throw new Exception($"Unsupported DDS format: 0x{fmt:X8}");

        var dataLength = ddsFile.Length - DDSHeaderSize;

        if (expectedMipSizes != null && expectedMipSizes.Length == mipCount && expectedMipSizes.Sum() == dataLength)
            return new DDSInfo(fmt, width, height, expectedMipSizes);

        var mipSizes = new int[mipCount];
        for (int i = 0, w = width, h = height; i < mipCount; ++i, w = Math.Max(1, w / 2), h = Math.Max(1, h / 2))
            mipSizes[i] = GetMipSize(fmt, w, h);

        if (mipSizes.Sum() != dataLength)
            throw new Exception($"DDS data length ({dataLength}) does not match the expected mip chain size ({mipSizes.Sum()})!");

        return new DDSInfo(fmt, width, height, mipSizes);
    }

    public static bool IsSupportedFormat(uint fmt) => fmt is FormatDXT1 or FormatDXT3 or FormatDXT5 or FormatRGBA32 or FormatRGBA16;

    private static bool IsBlockCompressed(uint fmt) => fmt is FormatDXT1 or FormatDXT3 or FormatDXT5;

    private static int GetBitsPerPixel(uint fmt) => fmt == FormatRGBA32 ? 32 : 16;

    private static int GetMipSize(uint fmt, int width, int height)
    {
        if (IsBlockCompressed(fmt))
            return Math.Max(1, (width + 3) / 4) * Math.Max(1, (height + 3) / 4) * (fmt == FormatDXT1 ? 8 : 16);

        return width * height * GetBitsPerPixel(fmt) / 8;
    }

    public static TextureMetadata? DeserializeMetadataJSON(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);

        return JsonConvert.DeserializeObject<TextureMetadata>(reader.ReadToEnd());
    }

    public static void SerializeMetadataJSON(TextureMetadata metadata, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(metadata, Formatting.Indented));
    }
}
