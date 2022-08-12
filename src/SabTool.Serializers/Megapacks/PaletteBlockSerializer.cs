using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Megapacks;

using SabTool.Data.Packs;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;

public static class PaletteBlockSerializer
{
    public static PaletteBlock DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var startOff = stream.Position;

        var streamBlock = StreamBlockSerializer.DeserializeFromMapRaw(stream, StreamBlockSerializer.SerializationFlags.EntriesForOnlyIndex2);

        var paletteBlock = new PaletteBlock
        {
            Crc = streamBlock.Id,
            X = streamBlock.Extents[0].X,
            Z = streamBlock.Extents[0].Z,
            Index = streamBlock.Index
        };

        var paletteSet = new HashSet<Crc>();

        for (var i = 0; i < streamBlock.PaletteCount; ++i)
        {
            paletteSet.Add(streamBlock.Palettes[i]);
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
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(paletteBlock, Formatting.Indented, new CrcConverter()));
    }
}
