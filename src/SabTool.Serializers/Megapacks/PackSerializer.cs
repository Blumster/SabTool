using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Packs;
using SabTool.Serializers.Packs;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Megapacks;
public static class PackSerializer
{
    public static void DeserializeRaw(Stream stream, StreamBlock streamBlock)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("SBLA", reversed: true))
            throw new Exception("Invalid pack header found!");

        StreamBlockSerializer.DeserializeHeader(streamBlock, stream);
    }

    public static void SerializeRaw(object value, Stream stream)
    {

    }

    public static void DeserializeJSON(Stream stream)
    {
    }

    public static void SerializeJSON(object value, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(value, Formatting.Indented));
    }
}
