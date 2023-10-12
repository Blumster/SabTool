namespace SabTool.Data.Animations;

public sealed class AnimationsContainer
{
    public AnimationPack AnimationPack { get; set; }
    public AnimationHavokData HavokData { get; set; }
    public uint BlendBoneException { get; set; }
    public AnimationIntervalPack IntervalPack { get; set; }
    public AnimationSequencePack SequencePack { get; set; }
    public AnimationTransitionContainer TransitionContainer { get; set; }
}
