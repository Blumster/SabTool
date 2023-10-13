namespace SabTool.Depot;

using SabTool.Data.Animations;
using SabTool.Serializers.Animations;

public sealed partial class ResourceDepot
{
    public AnimationsContainer AnimationsContainer { get; private set; }
    public AnimationsContainer DLCAnimationsContainer { get; private set; }

    private bool LoadAnimations()
    {
        Console.WriteLine("Loading Animations...");

        using var animationStream = new FileStream(GetGamePath("Animations.pack"), FileMode.Open, FileAccess.Read, FileShare.None);
        AnimationsContainer = AnimationsContainerSerializer.DeserializeRaw(animationStream, false);

        using var dlcAnimationStream = new FileStream(GetGamePath(@"DLC\01\Animations.pack"), FileMode.Open, FileAccess.Read, FileShare.None);
        DLCAnimationsContainer = AnimationsContainerSerializer.DeserializeRaw(dlcAnimationStream, true);

        Console.WriteLine("Animations loaded!");

        LoadedResources |= Resource.Animations;

        return true;
    }
}
