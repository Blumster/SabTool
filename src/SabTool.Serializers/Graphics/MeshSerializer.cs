using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Graphics
{
    using Data.Graphics;
    using Json.Converters;
    using Utils.Extensions;

    public static class MeshSerializer
    {
        public static Mesh DeserializeRaw(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            var mesh = new Mesh();

            var currentStart = stream.Position;

            stream.Position += 0xC;

            mesh.NumBones = reader.ReadInt32();
            mesh.NumUnk1 = reader.ReadInt32();
            mesh.Field14 = reader.ReadInt32();
            mesh.NumVertexHolder = reader.ReadInt16();
            mesh.NumPrimitives = reader.ReadInt16();
            mesh.NumUnk3 = reader.ReadInt16();
            mesh.Field1E = reader.ReadInt16();
            mesh.Field20 = reader.ReadInt32();
            mesh.Field24 = reader.ReadInt32();
            mesh.NumSegments = reader.ReadInt16();
            mesh.Field2A = reader.ReadByte();
            mesh.Field2B = reader.ReadByte();

            if (currentStart + 0x2C != stream.Position)
                throw new Exception($"Under or orver read of the base Mesh part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x2C}");

            mesh.Primitives = new Primitive[mesh.NumPrimitives];
            mesh.Segments = new Segment[mesh.NumSegments];
            mesh.VertexHolders = new VertexHolder[mesh.NumVertexHolder];
            mesh.Unk1s = new Unk1[mesh.NumUnk1];
            mesh.Unk3s = new Unk3[mesh.NumUnk3];

            currentStart = stream.Position;

            mesh.Field2C = reader.ReadInt32();
            mesh.Field30 = reader.ReadByte();
            mesh.Field31 = reader.ReadByte();
            mesh.Field32 = reader.ReadByte();
            mesh.Field33 = reader.ReadByte();

            if (currentStart + 0x8 != stream.Position)
                throw new Exception($"Under or orver read of the Mesh part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x8}");

            if (mesh.NumBones <= 1)
            {
                mesh.Skeleton = Skeleton.SingleBoneInstance;
            }
            else
            {
                mesh.Skeleton = SkeletonSerializer.DeserializeRaw(reader.BaseStream);

                if (mesh.Skeleton == null)
                    throw new Exception("Unable to deserialize the Mesh's Skeleton!");
            }

            if (mesh.NumUnk1 > 0)
            {
                // TODO
                var vals = reader.ReadConstArray(2, reader.ReadInt32);

                for (var i = 0; i < mesh.NumUnk1; ++i)
                {
                    mesh.Unk1s[i] = Unk1Serializer.DeserializeRaw(stream);
                }
            }

            for (var i = 0; i < mesh.NumUnk3; ++i)
            {
                mesh.Unk3s[i] = Unk3Serializer.DeserializeRaw(stream);
            }

            for (var i = 0; i < mesh.NumVertexHolder; ++i)
            {
                mesh.VertexHolders[i] = VertexHolderSerializer.DeserializeRaw(stream);
            }

            for (var i = 0; i < mesh.NumPrimitives; ++i)
            {
                mesh.Primitives[i] = PrimitiveSerializer.DeserializeRaw(stream, mesh);
            }

            for (var i = 0; i < mesh.NumSegments; ++i)
            {
                mesh.Segments[i] = SegmentSerializer.DeserializeRaw(stream, mesh);
            }

            return mesh;
        }

        public static void DeserializeVerticesRaw(Mesh mesh, Stream stream)
        {
            for (var i = 0; i < mesh.NumVertexHolder; ++i)
                VertexHolderSerializer.DeserializeVerticesRaw(mesh.VertexHolders[i], stream);
        }

        public static void SerializeRaw(Mesh mesh, Stream stream)
        {

        }

        public static Mesh DeserializeJSON(Stream stream)
        {
            return null;
        }

        public static void SerializeJSON(Mesh mesh, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            writer.Write(JsonConvert.SerializeObject(mesh, Formatting.Indented, new CrcConverter()));
        }
    }
}
