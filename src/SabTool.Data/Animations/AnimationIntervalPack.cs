namespace SabTool.Data.Animations;

public sealed class AnimationIntervalPack
{
    public List<AnimationInterval> Intervals { get; } = new();
}

public sealed class AnimationInterval
{
    public Crc UnkType { get; set; }
    public ushort BeginFrame { get; set; }
    public ushort EndFrame { get; set; }
    public List<Crc> Animations { get; } = new();
}
