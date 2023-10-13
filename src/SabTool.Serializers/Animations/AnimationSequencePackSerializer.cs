using System;
using System.IO;
using System.Text;

namespace SabTool.Serializers.Animations;

using SabTool.Data.Animations;
using SabTool.Utils.Extensions;

public static class AnimationSequencePackSerializer
{
    public static AnimationSequencePack DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("SEQC", reversed: true))
            throw new Exception("Invalid SEQC header found!");

        var sequencePack = new AnimationSequencePack();

        var sequenceCount = reader.ReadUInt32();

        for (var i = 0u; i < sequenceCount; ++i)
            sequencePack.Sequences.Add(DeserializeSequenceRaw(stream));

        return sequencePack;
    }

    public static AnimationSequence DeserializeSequenceRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var sequence = new AnimationSequence
        {
            Name = new(reader.ReadUInt32())
        };

        var instructionCount = reader.ReadUInt32();
        for (var i = 0u; i < instructionCount; ++i)
            sequence.Instructions.Add(DeserializeSequenceInstructionRaw(stream));

        sequence.Data = DeserialzieSequenceDataRaw(stream);

        return sequence;
    }

    public static AnimationSequenceInstruction DeserializeSequenceInstructionRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var sequenceInstr = new AnimationSequenceInstruction
        {
            Type = (AnimationInstructionType)reader.ReadUInt32(),
            LoopIndex = reader.ReadInt32(),
        };

        var animationCount = reader.ReadUInt32();
        for (var i = 0u; i < animationCount; ++i)
            sequenceInstr.Animations.Add(new(reader.ReadUInt32()));

        var tagCount = reader.ReadUInt32();
        for (var i = 0u; i < tagCount; ++i)
            sequenceInstr.Tags.Add(new(reader.ReadUInt32()));

        return sequenceInstr;
    }

    public static AnimationSequenceData DeserialzieSequenceDataRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var sequenceData = new AnimationSequenceData
        {
            Length = reader.ReadSingle(),
            Movement = reader.ReadVector3(),
            Rotation = reader.ReadSingle(),
            Type = (AnimationInstructionType)reader.ReadUInt32(),
            AimMin = reader.ReadVector2(),
            AimMax = reader.ReadVector2()
        };

        for (var i = 0; i < 3; ++i)
            sequenceData.AimFrameCounts[i] = reader.ReadByte();

        for (var i = 0; i < 3; ++i)
            sequenceData.AimFrames[i] = reader.ReadConstArray(sequenceData.AimFrameCounts[i], reader.ReadVector2);

        sequenceData.TurnPositive = reader.ReadBoolean();
        sequenceData.Synchronized = reader.ReadBoolean();

        return sequenceData;
    }
}
