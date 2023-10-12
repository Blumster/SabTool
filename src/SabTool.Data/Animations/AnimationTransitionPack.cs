namespace SabTool.Data.Animations;

public sealed class AnimationTransitionContainer
{
    public List<AnimationTransitionPack> TransitionPacks { get; } = new();
}
public sealed class AnimationTransitionPack
{
    public Crc Name { get; set; }
    public List<AnimationTransition> Transitions { get; } = new();
}

public sealed class AnimationTransition
{
    public Crc FromName { get; set; }
    public Crc FromTag { get; set; }
    public Crc ToSequenceName { get; set; }
    public Crc ToSequenceTag { get; set; }
    public float Threshold { get; set; }
    public TransitionType Type { get; set; }
    public float Value { get; set; }
    public Crc SequenceCrc { get; set; }
    public AnimationTransitionSequence Sequence { get; set; }
    public TransitionType NextType { get; set; }
    public float NextValue { get; set; }
    public uint NextSequenceIndex { get; set; }
    public string DebugName { get; set; }
}

public sealed class AnimationTransitionSequence
{
    public Crc Animation { get; set; }
    public Crc Tag { get; set; }
    public AnimationSequenceData Data { get; set; }
}

public enum TransitionType
{
    Set       = 0,
    Fade      = 1,
    Queue     = 2,
    Sync      = 3,
    QueueFade = 4,
    SyncWorld = 5
}
