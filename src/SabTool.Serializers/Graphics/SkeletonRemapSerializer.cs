using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Graphics;

using SabTool.Data.Graphics;
using SabTool.Utils.Extensions;

public static class SkeletonRemapSerializer
{
    public static SkeletonRemap DeserializeRaw(Stream stream)
    {
        var currentStart = stream.Position;

        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var skeletonRemap = new SkeletonRemap
        {
            BasePose = reader.ReadMatrix4x4(),
            Bone = reader.ReadInt32()
        };

        if (currentStart + 0x44 != stream.Position)
            throw new Exception($"Under or orver read of the SkeletonRemap part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x44}");

        return skeletonRemap;
    }

    public static void SerializeRaw(SkeletonRemap skeletonRemap, Stream stream)
    {

    }

    public static SkeletonRemap? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(SkeletonRemap skeletonRemap, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(skeletonRemap, Formatting.Indented));
    }
}
