using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Megapacks
{
    public static class PackSerializer
    {
        public static object DeserializeRaw(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            return null;
        }

        public static void SerializeRaw(object value, Stream stream)
        {

        }

        public static object DeserializeJSON(Stream stream)
        {
            return null;
        }

        public static void SerializeJSON(object unk1, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            writer.Write(JsonConvert.SerializeObject(unk1, Formatting.Indented));
        }
    }
}
