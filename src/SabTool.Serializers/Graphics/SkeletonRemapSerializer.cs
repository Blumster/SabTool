using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Graphics;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Graphics;
public static class SkeletonRemapSerializer
{
    public static SkeletonRemap DeserializeRaw(Stream stream)
    {
        long currentStart = stream.Position;

        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        SkeletonRemap skeletonRemap = new()
        {
            BasePose = reader.ReadMatrix4x4(),
            Bone = reader.ReadInt32()
        };

        return currentStart + 0x44 != stream.Position
            ? throw new Exception($"Under or orver read of the SkeletonRemap part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x44}")
            : skeletonRemap;
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
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(skeletonRemap, Formatting.Indented));
    }
}
