using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Packs;

using SabTool.Data.Packs;
using SabTool.Serializers.Json.Converters;
using SabTool.Serializers.Misc;
using SabTool.Serializers.Packs.Assets;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class StreamBlockSerializer
{
    [Flags]
    public enum SerializationFlags
    {
        None = 0,
        EntriesForOnlyIndex2 = 1
    }

    public static StreamBlock DeserializeFromMapRaw(Stream stream, SerializationFlags flags = SerializationFlags.None)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var streamBlock = DeserializeBaseBlock(stream);

        if (streamBlock.Index == 2 || (flags & SerializationFlags.EntriesForOnlyIndex2) == SerializationFlags.None)
        {
            ReadTextureInfo(streamBlock, reader);
            ReadEntries(streamBlock, reader);
        }

        return streamBlock;
    }

    public static StreamBlock DeserializeBaseBlock(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var streamBlock = new StreamBlock
        {
            Id = new(reader.ReadUInt32()),
            FileName = reader.ReadStringWithMaxLength(reader.ReadInt16())
        };

        streamBlock.Extents[0] = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        streamBlock.Extents[1] = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        streamBlock.UnkShort = reader.ReadInt16();
        streamBlock.Index = reader.ReadUInt16();

        return streamBlock;
    }

    public static void DeserializeHeader(StreamBlock streamBlock, Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        streamBlock.HeaderEnd = reader.ReadUInt32();
        if (streamBlock.HeaderEnd > 0)
            ReadHeaderData(streamBlock, reader);

        streamBlock.HeaderEnd += 4;

        var offInd = 0;

        do
        {
            var off = StreamBlock.OffIndices[offInd];

            if (streamBlock.EntryCounts[off] == 0)
            {
                ++offInd;
                continue;
            }

            streamBlock.Entries[off] = new StreamBlock.Entry[streamBlock.EntryCounts[off]];

            for (var i = 0; i < streamBlock.EntryCounts[off]; ++i)
                streamBlock.Entries[off][i] = new StreamBlock.Entry(reader);

            streamBlock.HeaderEnd += 24 * streamBlock.EntryCounts[off];

            ++offInd;
        }
        while (offInd < 9);

        streamBlock.HeaderEnd += 4;
    }

    public static void ReadPayloads(StreamBlock streamBlock, Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var offInd = 0;

        do
        {
            var off = StreamBlock.OffIndices[offInd];

            if (streamBlock.EntryCounts[off] == 0)
            {
                ++offInd;
                continue;
            }

            for (var i = 0; i < streamBlock.EntryCounts[off]; ++i)
            {
                var entry = streamBlock.Entries[off][i];

                // TODO: should be align up after each other, but who knows...
                //reader.BaseStream.Position = streamBlock.HeaderEnd + entry.Offset;

                entry.Payload = reader.ReadBytes(entry.CompressedSize);
            }

            ++offInd;
        }
        while (offInd < 9);
    }

    public static void ReadHeaderData(StreamBlock streamBlock, BinaryReader reader)
    {
        streamBlock.HeightRangeMax = 1.0f;

        if ((streamBlock.Flags & 0x1C00) == 0x400)
        {
            if (!reader.CheckHeaderString("HEI1", reversed: true))
                throw new Exception("Invalid StreamBlock header found!");

            streamBlock.PointCountX = reader.ReadInt32();
            streamBlock.PointCountY = reader.ReadInt32();
            streamBlock.HeightRangeMin = reader.ReadSingle();
            streamBlock.HeightRangeMax = reader.ReadSingle();
            streamBlock.HeightMapData = reader.ReadBytes(streamBlock.PointCountX * streamBlock.PointCountY);
        }

        ReadTextureInfo(streamBlock, reader);
        ReadEntries(streamBlock, reader);
    }

    public static void ReadTextureInfo(StreamBlock streamBlock, BinaryReader reader)
    {
        streamBlock.TextureCount = reader.ReadUInt32();
        if (streamBlock.TextureCount > 0)
        {
            streamBlock.TextureInfoArray = new StreamBlock.TextureInfo[streamBlock.TextureCount];

            for (var i = 0; i < streamBlock.TextureCount; ++i)
            {
                streamBlock.TextureInfoArray[i] = new StreamBlock.TextureInfo
                {
                    Crc = new(reader.ReadUInt32()),
                    UncompressedSize = reader.ReadUInt32()
                };

                streamBlock.TotalTextureSize += streamBlock.TextureInfoArray[i].UncompressedSize;
            }
        }

        streamBlock.TextureCount2 = reader.ReadUInt32();
        if (streamBlock.TextureCount2 > 0)
        {
            streamBlock.TextureInfoArray2 = new StreamBlock.TextureInfo[streamBlock.TextureCount2];

            for (var i = 0; i < streamBlock.TextureCount2; ++i)
            {
                streamBlock.TextureInfoArray2[i] = new StreamBlock.TextureInfo
                {
                    Crc = new(reader.ReadUInt32()),
                    UncompressedSize = reader.ReadUInt32()
                };

                streamBlock.TotalTextureSize += streamBlock.TextureInfoArray2[i].UncompressedSize;
            }
        }
    }

    public static void ReadEntries(StreamBlock streamBlock, BinaryReader reader)
    {
        _ = reader.ReadInt32();

        for (var i = 0; i < streamBlock.EntryCounts.Length; ++i)
            streamBlock.EntryCounts[i] = reader.ReadUInt32();

        streamBlock.Count1ACFor1B0And1B4_1AC = reader.ReadUInt32();
        if (streamBlock.Count1ACFor1B0And1B4_1AC > 0)
        {
            streamBlock.Array1B0 = new Crc[streamBlock.Count1ACFor1B0And1B4_1AC];
            streamBlock.Array1B4 = new byte[streamBlock.Count1ACFor1B0And1B4_1AC];

            for (var i = 0; i < streamBlock.Count1ACFor1B0And1B4_1AC; ++i)
            {
                streamBlock.Array1B0[i] = new(reader.ReadUInt32());
                streamBlock.Array1B4[i] = 0;
            }
        }

        streamBlock.PaletteCount = (ushort)reader.ReadInt32();
        for (var i = 0; i < streamBlock.PaletteCount; ++i)
            streamBlock.Palettes[i] = new Crc(reader.ReadUInt32());

        streamBlock.FenceTreeCount = reader.ReadInt32();
        for (var i = 0; i < streamBlock.FenceTreeCount; ++i)
        {
            var crc = new Crc(reader.ReadUInt32());
            var subCnt = reader.ReadInt32();

            try
            {
                streamBlock.FenceTree.Add(crc, reader.ReadConstArray(subCnt, () => new Crc(reader.ReadUInt32())));
            }
            catch
            {
                Console.WriteLine("Trying to add multiple fence tree by the same id!");
            }
        }
    }

    public static void SerializeRaw(StreamBlock streamBlock, Stream stream)
    {

    }

    public static StreamBlock? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(StreamBlock streamBlock, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(streamBlock, Formatting.Indented, new CrcConverter()));
    }

    public static void Export(StreamBlock streamBlock, string outputPath, IProgress<string> progress)
    {
        Directory.CreateDirectory(outputPath);

        var offInd = 0;

        do
        {
            var off = StreamBlock.OffIndices[offInd];

            if (streamBlock.EntryCounts[off] == 0)
            {
                ++offInd;
                continue;
            }

            for (var i = 0; i < streamBlock.EntryCounts[off]; ++i)
            {
                var entry = streamBlock.Entries[off][i];

                var extension = "bin";

                try
                {
                    switch (off)
                    {
                        case 0:
                            extension = "mesh-bin";

                            var meshAsset = MeshAssetSerializer.DeserializeRaw(new MemoryStream(entry.Payload, false));

                            meshAsset.Export(outputPath);

                            progress.Report(meshAsset.ModelName);
                            continue;

                        case 1:
                            extension = "texture-bin";

                            var textureAsset = TextureAssetSerializer.DeserializeRaw(new MemoryStream(entry.Payload, false), entry.Crc);
                            if (textureAsset is not null)
                            {
                                textureAsset.Export(outputPath);

                                progress.Report(textureAsset.Name ?? "");

                                continue;
                            }
                            
                            break;

                        case 2:
                            extension = "physics-bin";

                            var physicsAsset = PhysicsAssetSerializer.DeserializeRaw(entry);
                            if (physicsAsset is not null)
                            {
                                physicsAsset.Export(outputPath);

                                continue;
                            }
                            
                            break;

                        case 3:
                            extension = "pathgraph";

                            Console.Error.WriteLine("PATHGRAPH! {0}", string.IsNullOrWhiteSpace(entry.Crc.GetString()) ? $"0x{entry.Crc.Value:X8}.{extension}" : $"{entry.Crc.GetString()}.{extension}");
                            break;

                        case 4:
                            extension = "aifence";
                            Console.Error.WriteLine("AIFENCE! {0}", string.IsNullOrWhiteSpace(entry.Crc.GetString()) ? $"0x{entry.Crc.Value:X8}.{extension}" : $"{entry.Crc.GetString()}.{extension}");
                            break;

                        case 5:
                            extension = "unknown";
                            Console.Error.WriteLine("UNK! {0}", string.IsNullOrWhiteSpace(entry.Crc.GetString()) ? $"0x{entry.Crc.Value:X8}.{extension}" : $"{entry.Crc.GetString()}.{extension}");
                            break;

                        case 6:
                            extension = "soundbank";
                            Console.Error.WriteLine("SOUNDBANK! {0}", string.IsNullOrWhiteSpace(entry.Crc.GetString()) ? $"0x{entry.Crc.Value:X8}.{extension}" : $"{entry.Crc.GetString()}.{extension}");
                            break;

                        case 7:
                            extension = "gfx";
                            break;

                        case 8:
                            extension = "wsd-bin";

                            var dictOutPath = Path.Combine(outputPath, entry.SpawnTag.Value == 0 ? "objects.wsd.json" : $"{entry.SpawnTag.GetStringOrHexString()}.wsd.json");

                            var dict = DictionarySerializer.DeserializeRaw(new MemoryStream(entry.Payload, false));

                            using (var fs = new FileStream(dictOutPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                                DictionarySerializer.SerializeJSON(dict, fs);

                            continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unable to export {entry.Crc.GetStringOrHexString()}! Exporting as raw! Exception:");
                    Console.Error.WriteLine(ex.ToString());
                }

                var outputFileName = $"{entry.Crc.GetStringOrHexString()}.{extension}";
                var outputFilePath = Path.Combine(outputPath, outputFileName);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)!);

                File.WriteAllBytes(outputFilePath, entry.Payload);

                progress.Report(outputFileName);
            }

            ++offInd;
        }
        while (offInd < 9);
    }

}
