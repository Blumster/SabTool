using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Animations;

using SabTool.Data.Animations;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils.Extensions;

public static class AnimationsContainerSerializer
{
    public static AnimationsContainer DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("AP0L", reversed: true))
            throw new Exception("Invalid AP0L header found!");

        var container = new AnimationsContainer
        {
            AnimationPack = AnimationPackSerializer.DeserializeRaw(stream),
            HavokData = AnimationHavokSerializer.DeserializeRaw(stream),
            BlendBoneException = reader.ReadUInt32(),
            IntervalPack = AnimationIntervalPackSerializer.DeserializeRaw(stream),
            SequencePack = AnimationSequencePackSerializer.DeserializeRaw(stream),
            TransitionContainer = AnimationTransitionPackSerializer.DeserializeContainerRaw(stream),
        };

        return container;
    }

    public static void SerializeJSON(AnimationsContainer container, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(container, Formatting.Indented, new CrcConverter()));
    }
}
