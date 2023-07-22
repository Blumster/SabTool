using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Graphics;
using SabTool.Serializers.Json.Converters;

namespace SabTool.Serializers.Graphics;
public static class MeshSerializer
{
    public static Mesh DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        Mesh mesh = new();

        long currentStart = stream.Position;

        stream.Position += 0xC;

        mesh.NumBones = reader.ReadInt32();
        mesh.NumSkeletonRemaps = reader.ReadInt32();
        mesh.Field14 = reader.ReadInt32();
        mesh.NumVertexHolder = reader.ReadInt16();
        mesh.NumPrimitives = reader.ReadInt16();
        mesh.NumShadows = reader.ReadInt16();
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
        mesh.SkeletonRemaps = new SkeletonRemap[mesh.NumSkeletonRemaps];
        mesh.Shadows = new Shadow[mesh.NumShadows];

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
            mesh.Skeleton = SkeletonSerializer.DeserializeRaw(reader.BaseStream, mesh.NumBones);

            if (mesh.Skeleton == null)
                throw new Exception("Unable to deserialize the Mesh's Skeleton!");
        }

        if (mesh.NumSkeletonRemaps > 0)
        {
            mesh.NumSkeletonRemapBones = reader.ReadInt32();

            stream.Position += 4;

            for (int i = 0; i < mesh.NumSkeletonRemaps; ++i)
                mesh.SkeletonRemaps[i] = SkeletonRemapSerializer.DeserializeRaw(stream);
        }

        for (int i = 0; i < mesh.NumShadows; ++i)
            mesh.Shadows[i] = ShadowSerializer.DeserializeRaw(stream);

        for (int i = 0; i < mesh.NumVertexHolder; ++i)
            mesh.VertexHolders[i] = VertexHolderSerializer.DeserializeRaw(stream);

        for (int i = 0; i < mesh.NumPrimitives; ++i)
            mesh.Primitives[i] = PrimitiveSerializer.DeserializeRaw(stream, mesh);

        for (int i = 0; i < mesh.NumSegments; ++i)
            mesh.Segments[i] = SegmentSerializer.DeserializeRaw(stream, mesh);

        return stream.Length != stream.Position
            ? throw new Exception($"Under or over read of the mesh asset! Pos: {stream.Position} | Expected: {stream.Length}")
            : mesh;
    }

    public static void DeserializeVerticesRaw(Mesh mesh, Stream stream)
    {
        for (int i = 0; i < mesh.NumVertexHolder; ++i)
            VertexHolderSerializer.DeserializeVerticesRaw(mesh.VertexHolders[i], stream);
    }

    public static void SerializeRaw(Mesh mesh, Stream stream)
    {

    }

    public static Mesh? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Mesh mesh, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(mesh, Formatting.Indented, new CrcConverter()));
    }
}
