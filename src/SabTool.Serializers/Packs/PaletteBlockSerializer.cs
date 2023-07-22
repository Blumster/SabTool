using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Packs;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;

namespace SabTool.Serializers.Packs;
public static class PaletteBlockSerializer
{
    public static PaletteBlock DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        long startOff = stream.Position;

        StreamBlock streamBlock = StreamBlockSerializer.DeserializeFromMapRaw(stream, StreamBlockSerializer.SerializationFlags.EntriesForOnlyIndex2);

        PaletteBlock paletteBlock = new()
        {
            Crc = streamBlock.Id,
            X = streamBlock.Extents[0].X,
            Z = streamBlock.Extents[0].Z,
            Index = streamBlock.Index
        };

        HashSet<Crc> paletteSet = new();

        for (int i = 0; i < streamBlock.PaletteCount; ++i)
        {
            _ = paletteSet.Add(streamBlock.Palettes[i]);
        }

        paletteBlock.Palettes = paletteSet.ToList();

        return paletteBlock;
    }

    public static void SerialzieRaw(PaletteBlock paletteBlock, Stream stream)
    {

    }

    public static PaletteBlock? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(PaletteBlock paletteBlock, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(paletteBlock, Formatting.Indented, new CrcConverter()));
    }
}
