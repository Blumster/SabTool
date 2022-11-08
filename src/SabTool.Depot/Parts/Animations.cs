namespace SabTool.Depot;

public sealed partial class ResourceDepot
{
    private bool LoadAnimations()
    {
        Console.WriteLine("Loading Animations...");

        Console.WriteLine("Animations loaded!");

        LoadedResources |= Resource.Animations;

        return true;
    }
}
