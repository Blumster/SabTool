namespace SabTool.Data.Animations;

public sealed class Animation
{
    public Crc Crc { get; set; }
    public byte Unk1 { get; set; }
    public byte Unk2 { get; set; }
    public string Name { get; set; }
    public float Unk3 { get; set; }
    public object BoneIndices { get; set; }
    public float MotionLength { get; set; }
    public float MovementX { get; set; }
    public float MovementY { get; set; }
    public float MovementZ { get; set; }
    public float Rotation {  get; set; }
    public float StartAimAngle { get; set; }
    public float EndAimAngle { get; set; }
    public float AimPitch { get; set; }
    public byte Unk4 { get; set; }
    public List<AnimationEvent> Events { get; } = new();
    public List<AnimationOffset> Offsets { get; } = new();
}

public sealed class AnimationBoneIndices
{
    public List<uint> TrackBone { get; } = new();
    public List<uint> BoneIndex { get; } = new();
}

public sealed class AnimationEvent
{
    public uint Frame { get; set; }
    public Crc Callback { get; set; }
    public List<AnimationEventParam> Params { get; } = new();
}

public sealed class AnimationEventParam
{
    public Crc CrcParam { get; set; }
    public float FloatParam { get; set; }
}

public sealed class AnimationOffset
{
    public bool Unk1 { get; set; }
    public Vector3 Offset { get; set; }
    public float DeltaTime { get; set; }
}
