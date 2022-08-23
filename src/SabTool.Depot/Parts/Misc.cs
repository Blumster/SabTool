namespace SabTool.Depot;

using SabTool.Data.Misc;
using SabTool.Serializers.Misc;

public partial class ResourceDepot
{
    public Heightmap? Heightmap { get; private set; }

    private bool LoadMisc()
    {
        Console.WriteLine("Loading Misc...");

        using var heightmapStream = new FileStream(GetGamePath("France.hei"), FileMode.Open, FileAccess.Read, FileShare.Read);

        Heightmap = HeightmapSerializer.DeserializeRaw(heightmapStream);

        Console.WriteLine("Misc loaded!");

        LoadedResources |= Resource.Misc;

        return true;
    }
}
