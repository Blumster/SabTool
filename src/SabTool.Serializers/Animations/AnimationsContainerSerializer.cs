using System;
using System.IO;
using System.Text;

namespace SabTool.Serializers.Animations;

using SabTool.Data.Animations;
using SabTool.Utils.Extensions;

public static class AnimationsContainerSerializer
{
    public static AnimationsContainer DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("AP0L", reversed: true))
            throw new Exception("Invalid AP0L header found!");

        var container = new AnimationsContainer();

        container.AnimationPacks.Add(AnimationPackSerializer.DeserializeRaw(stream));

        return container;
    }
}
