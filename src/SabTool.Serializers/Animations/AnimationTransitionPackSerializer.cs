using System;
using System.IO;
using System.Text;

namespace SabTool.Serializers.Animations;

using SabTool.Data.Animations;
using SabTool.Utils;
using SabTool.Utils.Extensions;
using SharpGLTF.Schema2;

public static class AnimationTransitionPackSerializer
{
    public static AnimationTransitionContainer DeserializeContainerRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("TRAN", reversed: true))
            throw new Exception("Invalid TRAN header found!");

        var transitionContainer = new AnimationTransitionContainer();

        var transitionPackCount = reader.ReadUInt32();

        for (var i = 0u; i < transitionPackCount; ++i)
            transitionContainer.TransitionPacks.Add(DeserializeTransitionPackRaw(stream));

        return transitionContainer;
    }

    public static AnimationTransitionPack DeserializeTransitionPackRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var transitionPack = new AnimationTransitionPack
        {
            Name = new(reader.ReadUInt32())
        };

        var transitionCount = reader.ReadUInt32();

        for (var i = 0u; i < transitionCount; ++i)
            transitionPack.Transitions.Add(DeserializeTransitionRaw(stream));

        return transitionPack;
    }

    public static AnimationTransition DeserializeTransitionRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var transition = new AnimationTransition
        {
            FromName = new(reader.ReadUInt32()),
            FromTag = new(reader.ReadUInt32()),
            ToSequenceName = new(reader.ReadUInt32()),
            ToSequenceTag = new(reader.ReadUInt32()),
            Threshold = reader.ReadSingle(),
            Type = new(reader.ReadUInt32()),
            Value = reader.ReadSingle(),
            SequenceCrc = new(reader.ReadUInt32())
        };

        if (transition.SequenceCrc.Value != 0)
            transition.Sequence = DeserializeTransitionSequenceRaw(stream);

        transition.NextType = (TransitionType)reader.ReadUInt32();
        transition.NextValue = reader.ReadSingle();
        transition.NextSequenceIndex = reader.ReadUInt32();
        transition.DebugName = reader.ReadUTF8StringOn(reader.ReadInt16());

        // Force hashes, maybe some missing ones can be found
        Hash.FNV32string(transition.DebugName);
        Hash.StringToHash(transition.DebugName);

        return transition;
    }

    public static AnimationTransitionSequence DeserializeTransitionSequenceRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        return new AnimationTransitionSequence
        {
            Animation = new(reader.ReadUInt32()),
            Tag = new(reader.ReadUInt32()),
            Data = AnimationSequencePackSerializer.DeserialzieSequenceDataRaw(stream)
        };
    }
}
