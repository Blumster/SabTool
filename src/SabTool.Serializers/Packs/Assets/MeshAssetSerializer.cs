using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Packs.Assets;

using SabTool.Data.Packs.Assets;
using SabTool.Serializers.Graphics;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class MeshAssetSerializer
{
    public static MeshAsset DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("MSHA", reversed: true))
            throw new Exception("Invalid mesh asset header found!");

        var meshAsset = new MeshAsset();

        var headerRealSize = reader.ReadInt32();
        var vertexRealSize = reader.ReadInt32();
        var headerCompressedSize = reader.ReadInt32();
        var vertexCompressedSize = reader.ReadInt32();
        meshAsset.ModelName = reader.ReadStringFromCharArray(256);

        // Save it to the lookup table
        Hash.StringToHash(meshAsset.ModelName);

        using var modelStream = headerRealSize == headerCompressedSize
            ? new MemoryStream(reader.ReadBytes(headerRealSize))
            : new MemoryStream(reader.ReadDecompressedBytes(headerCompressedSize), false);

        using var vertexStream = vertexRealSize == vertexCompressedSize
            ? new MemoryStream(reader.ReadBytes(vertexRealSize))
            : new MemoryStream(reader.ReadDecompressedBytes(vertexCompressedSize), false);

        meshAsset.Model = ModelSerializer.DeserializeRaw(modelStream);
        meshAsset.Model.Mesh = MeshSerializer.DeserializeRaw(modelStream);

        if (modelStream.Position != modelStream.Length)
            throw new Exception($"Under read of the header data of the mesh asset! Pos: {modelStream.Position} | Expected: {modelStream.Length}");

        MeshSerializer.DeserializeVerticesRaw(meshAsset.Model.Mesh, vertexStream);

        if (vertexStream.Position != vertexStream.Length)
            throw new Exception($"Under read of the vertex data of the mesh asset! Pos: {vertexStream.Position} | Expected: {vertexStream.Length}");

        return meshAsset;
    }

    public static void SerializeRaw(MeshAsset meshAsset, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
    }

    public static MeshAsset? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(MeshAsset meshAsset, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(meshAsset, Formatting.Indented));
    }
}
