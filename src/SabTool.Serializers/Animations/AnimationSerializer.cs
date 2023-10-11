using System.IO;
using System.Text;

namespace SabTool.Serializers.Animations;

using SabTool.Data.Animations;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class AnimationSerializer
{
    public static Animation DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var animation = new Animation
        {
            Crc = new(reader.ReadUInt32()),
            Unk1 = reader.ReadByte(),
            Unk2 = reader.ReadByte(),
            Name = reader.ReadUTF8StringOn(reader.ReadInt32()),
            Unk3 = reader.ReadSingle()
        };

        // Force hashes, maybe some missing ones can be found
        Hash.FNV32string(animation.Name);
        Hash.StringToHash(animation.Name);

        if (animation.Unk2 == 0)
            animation.BoneIndices = DeserializeBoneIndices(reader, animation.Unk1);

        animation.MotionLength = reader.ReadSingle();
        animation.MovementX = reader.ReadSingle();
        animation.MovementY = reader.ReadSingle();
        animation.MovementZ = reader.ReadSingle();
        animation.Rotation = reader.ReadSingle();
        animation.StartAimAngle = reader.ReadSingle();
        animation.EndAimAngle = reader.ReadSingle();
        animation.AimPitch = reader.ReadSingle();
        animation.Unk4 = reader.ReadByte();

        var eventCount = reader.ReadUInt32();
        for (var i = 0u; i < eventCount; ++i)
            animation.Events.Add(DeserializeEvent(reader));

        var offsetCount = reader.ReadUInt32();
        for (var i = 0u; i < offsetCount; ++i)
            animation.Offsets.Add(DeserializeOffset(reader));

        return animation;
    }

    private static AnimationBoneIndices DeserializeBoneIndices(BinaryReader reader, byte unk1)
    {
        var indices = new AnimationBoneIndices();

        var count = reader.ReadInt32();

        if (unk1 == 0)
            indices.BoneIndex.AddRange(reader.ReadConstArray(count, reader.ReadUInt32));
        else
            indices.TrackBone.AddRange(reader.ReadConstArray(count, reader.ReadUInt32));

        return indices;
    }

    private static AnimationEvent DeserializeEvent(BinaryReader reader)
    {
        var animEvent = new AnimationEvent
        {
            Frame = reader.ReadUInt32(),
            Callback = new(reader.ReadUInt32())
        };

        for (var i = 0; i < 4; ++i)
            animEvent.Params.Add(DeserializeEventParam(reader));

        return animEvent;
    }

    private static AnimationEventParam DeserializeEventParam(BinaryReader reader)
    {
        return new AnimationEventParam
        {
            CrcParam = new(reader.ReadUInt32()),
            FloatParam = reader.ReadSingle()
        };
    }

    private static AnimationOffset DeserializeOffset(BinaryReader reader)
    {
        return new AnimationOffset
        {
            Unk1 = reader.ReadBoolean(),
            Offset = reader.ReadVector3(),
            DeltaTime = reader.ReadSingle()
        };
    }
}
