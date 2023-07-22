using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Packs;
using SabTool.Serializers.Json.Converters;
using SabTool.Serializers.Packs;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Megapacks;
public static class GlobalMapSerializer
{
    public static GlobalMap DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        GlobalMap globalMap = new();

        if (!reader.CheckHeaderString("MAP6", reversed: true))
            throw new Exception("Invalid global Map header found!");

        string mapFileNameWithoutExtension = Path.GetFileNameWithoutExtension("global.map");

        globalMap.NumTotalBlocks = reader.ReadUInt32();

        int streamBlockArrayIdx = 0;
        uint itrCount = 0u;

        do
        {
            globalMap.NumStreamBlocks = reader.ReadUInt32();

            globalMap.StreamBlockArray[streamBlockArrayIdx] = new StreamBlock[globalMap.NumStreamBlocks];

            if (globalMap.NumStreamBlocks == 0)
            {
                ++streamBlockArrayIdx;
                ++itrCount;
                continue;
            }

            int i = 0;
            uint unkFlagStuff = ((itrCount + 1) & 7) << 10;

            do
            {
                StreamBlock streamBlock = StreamBlockSerializer.DeserializeBaseBlock(reader.BaseStream);

                streamBlock.Midpoint = new(0.0f, 0.0f, 0.0f);
                streamBlock.Extents[0] = new(-10000.0f, -10000.0f, -10000.0f);
                streamBlock.Extents[1] = new(10000.0f, 10000.0f, 10000.0f);
                streamBlock.FieldC0 = BitConverter.ToSingle(BitConverter.GetBytes(0xFFFFFFFF), 0);
                streamBlock.FileName = $"{mapFileNameWithoutExtension}\\{streamBlock.FileName}{itrCount}";
                streamBlock.Flags = (unkFlagStuff | (streamBlock.Flags & 0xFFFFE3FF)) & 0xFFFFFF3F;

                StreamBlockSerializer.ReadTextureInfo(streamBlock, reader);
                StreamBlockSerializer.ReadEntries(streamBlock, reader);

                streamBlock.Flags = (streamBlock.Flags & 0xFFFFFFF8) | 0x100;

                globalMap.StreamBlockArray[streamBlockArrayIdx][i] = streamBlock;

                globalMap.StaticBlocks.Add(streamBlock.Id, streamBlock);
            }
            while (++i < globalMap.NumStreamBlocks);

            ++streamBlockArrayIdx;
            ++itrCount;
        }
        while (itrCount < 2);

        LoadBlocksFromMapFile(globalMap, reader, globalMap.NumTotalBlocks, mapFileNameWithoutExtension, false);

        return globalMap;
    }

    private static void LoadBlocksFromMapFile(GlobalMap globalMap, BinaryReader reader, uint count, string mapFileNameWithoutExtension, bool removeExisting)
    {
        static float GetMiddlePoint(float f1, float f2) => f1 + ((f2 - f1) / 2.0f);

        if (count <= 0)
            return;

        for (int i = 0; i < count; ++i)
        {
            StreamBlock streamBlock = StreamBlockSerializer.DeserializeBaseBlock(reader.BaseStream);

            if (removeExisting && globalMap.DynamicBlocks.ContainsKey(streamBlock.Id))
                _ = globalMap.DynamicBlocks.Remove(streamBlock.Id);

            streamBlock.Flags |= 8;
            streamBlock.FileName = $"{mapFileNameWithoutExtension}\\{streamBlock.FileName}";

            // Do not override it. If removeExisting is true, existing ones will be removed
            if (!globalMap.DynamicBlocks.ContainsKey(streamBlock.Id))
                globalMap.DynamicBlocks.Add(streamBlock.Id, streamBlock);
            else
                Console.WriteLine($"DEBUG: Trying to add {streamBlock.Id} (${streamBlock.FileName}) to dictionary, but it already exists!");

            streamBlock.Extents[0].Y = 0.0f;
            streamBlock.Extents[1].Y = 0.0f;
            streamBlock.Midpoint = new(
                GetMiddlePoint(streamBlock.Extents[0].X, streamBlock.Extents[1].X),
                GetMiddlePoint(streamBlock.Extents[0].Y, streamBlock.Extents[1].Y),
                GetMiddlePoint(streamBlock.Extents[0].Z, streamBlock.Extents[1].Z));
            streamBlock.Flags = (streamBlock.Flags & 0xFFFFE33F) | ((uint)(streamBlock.Index & 7) << 10);
            streamBlock.Flags &= 0xFFFFFEF8;

            StreamBlockSerializer.ReadTextureInfo(streamBlock, reader);
            StreamBlockSerializer.ReadEntries(streamBlock, reader);
        }
    }

    public static GlobalMap? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(GlobalMap globalMap, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(globalMap, Formatting.Indented, new CrcConverter()));
    }
}
