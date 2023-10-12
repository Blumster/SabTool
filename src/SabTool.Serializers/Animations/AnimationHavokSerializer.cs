using System.IO;
using System.Text;

namespace SabTool.Serializers.Animations;

using SabTool.Data.Animations;

public static class AnimationHavokSerializer
{
    public static AnimationHavokData DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        return new AnimationHavokData
        {
            UnkCount = reader.ReadUInt32(),
            UnkData = reader.ReadBytes(reader.ReadInt32())
        };
    }
}
