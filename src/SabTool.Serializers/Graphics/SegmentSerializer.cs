using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Graphics
{
    using Data.Graphics;
    using Json.Converters;
    using Utils;

    public static class SegmentSerializer
    {
        public static Segment DeserializeRaw(Stream stream, Mesh mesh)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            var segment = new Segment()
            {
                Mesh = mesh
            };

            var currentStart = stream.Position;

            segment.PrimitiveIndex = reader.ReadInt32();

            segment.Primitive = segment.Mesh.Primitives[segment.PrimitiveIndex];
            segment.MaterialCrc = new Crc(reader.ReadUInt32());

            stream.Position += 0x4;

            segment.BoneIndex = reader.ReadInt16();
            segment.Flags = reader.ReadInt16();

            if (currentStart + 0x10 != stream.Position)
                throw new Exception($"Under or orver read of the Segment part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x10}");

            return null;
        }

        public static void SerializeRaw(Segment segment, Stream stream)
        {

        }

        public static Segment DeserializeJSON(Stream stream)
        {
            return null;
        }

        public static void SerializeJSON(Segment segment, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            writer.Write(JsonConvert.SerializeObject(segment, Formatting.Indented, new CrcConverter()));
        }
    }
}
