using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Megapacks;

using SabTool.Data.Packs;
using SabTool.Serializers.Packs;
using SabTool.Utils.Extensions;

public static class PackSerializer
{
    public static void DeserializeRaw(Stream stream, StreamBlock streamBlock, bool loadData = false)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("SBLA", reversed: true))
            throw new Exception("Invalid pack header found!");

        StreamBlockSerializer.DeserializeHeader(streamBlock, stream);

        if (loadData)
            StreamBlockSerializer.ReadPayloads(streamBlock, stream); // TODO: is this good here? it loads everything into memory
    }

    public static void SerializeRaw(object value, Stream stream)
    {

    }

    public static void DeserializeJSON(Stream stream)
    {
    }

    public static void SerializeJSON(object value, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(value, Formatting.Indented));
    }
}
