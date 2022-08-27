namespace SabTool.Depot;

using SabTool.Data.Misc;
using SabTool.Serializers.Misc;

public partial class ResourceDepot
{
    public Heightmap? Heightmap { get; private set; }
    public Waterflow? Waterflow { get; private set; }
    public Freeplay? Freeplay { get; private set; }
    public Watercontrol? Watercontrol { get; private set; }

    private bool LoadMisc()
    {
        Console.WriteLine("Loading Misc...");

        using FileStream heightmapStream = new(GetGamePath("France.hei"), FileMode.Open, FileAccess.Read, FileShare.None);
        Heightmap = HeightmapSerializer.DeserializeRaw(heightmapStream);
        using FileStream waterflowStream = new(GetGamePath("France.waterflow"), FileMode.Open, FileAccess.Read, FileShare.None);
        Waterflow = WaterflowSerializer.DeserializeRaw(waterflowStream);
        using FileStream freeplayStream = new(GetGamePath("France.freeplay"), FileMode.Open, FileAccess.Read, FileShare.None);
        Freeplay = FreeplaySerializer.DeserializeRaw(freeplayStream);
        using FileStream watercontrolStream = new(GetGamePath("France.waterctrl"), FileMode.Open, FileAccess.Read, FileShare.None);
        Watercontrol = WatercontrolSerializer.DeserializeRaw(watercontrolStream);
        Console.WriteLine("Misc loaded!");

        LoadedResources |= Resource.Misc;

        return true;
    }
}
