using System;
using System.IO;
using System.Text;

namespace SabTool.Serializers.Animations;

using SabTool.Data.Animations;
using SabTool.Utils.Extensions;

public static class AnimationIntervalPackSerializer
{
    public static AnimationIntervalPack DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("INTV", reversed: true))
            throw new Exception("Invalid INTV header found!");

        var animationPack = new AnimationIntervalPack();

        var intervalCount = reader.ReadUInt32();

        for (var i = 0u; i < intervalCount; ++i)
            animationPack.Intervals.Add(DeserializeIntervalRaw(stream));

        return animationPack;
    }

    public static AnimationInterval DeserializeIntervalRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var interval = new AnimationInterval
        {
            UnkType = reader.ReadUInt32(),
            BeginFrame = reader.ReadUInt16(),
            EndFrame = reader.ReadUInt16()
        };

        var animationCount = reader.ReadUInt32();

        for (var i = 0u; i < animationCount; ++i)
            interval.Animations.Add(reader.ReadUInt32());

        return interval;
    }
}
