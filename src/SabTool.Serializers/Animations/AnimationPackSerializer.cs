using System;
using System.IO;
using System.Text;

namespace SabTool.Serializers.Animations;

using SabTool.Data.Animations;
using SabTool.Utils.Extensions;

public static class AnimationPackSerializer
{
    public static AnimationPack DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("ANIM", reversed: true))
            throw new Exception("Invalid ANIM header found!");

        var animationPack = new AnimationPack();

        var animationCount = reader.ReadUInt32();

        for (var i = 0u; i < animationCount; ++i)
            animationPack.Animations.Add(AnimationSerializer.DeserializeRaw(stream));

        return animationPack;
    }
}
