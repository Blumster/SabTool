namespace SabTool.Data.Animations;

public sealed class AnimationSequencePack
{
    public List<AnimationSequence> Sequences { get; } = new();
}

public sealed class AnimationSequence
{
    public Crc Name { get; set; }
    public List<AnimationSequenceInstruction> Instructions { get; } = new();
    public AnimationSequenceData Data { get; set; }
}

public sealed class AnimationSequenceInstruction
{
    public AnimationInstructionType Type { get; set; }
    public int LoopIndex { get; set; }
    public List<Crc> Animations { get; } = new();
    public List<Crc> Tags { get; } = new();
}

public sealed class AnimationSequenceData
{
    public float Length { get; set; }
    public Vector3 Movement { get; set; }
    public float Rotation { get; set; }
    public AnimationInstructionType Type { get; set; }
    public Vector2 AimMin { get; set; }
    public Vector2 AimMax { get; set; }
    public byte[] AimFrameCounts { get; } = new byte[3];
    public Vector2[][] AimFrames { get; set; } = new Vector2[3][];
    public bool TurnPositive { get; set; }
    public bool Synchronized { get; set; }
}

[Flags]
public enum AnimationInstructionType
{
    Animation        = 0x001,
    Loop             = 0x002,
    Stop             = 0x004,
    Animation1D      = 0x008,
    Animation2D      = 0x010,
    StartPose        = 0x020,
    EndPose          = 0x040,
    LoopSynchronized = 0x080,
    SteeringAnim     = 0x100
}
