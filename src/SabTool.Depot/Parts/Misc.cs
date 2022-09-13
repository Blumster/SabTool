namespace SabTool.Depot;

using SabTool.Data.Misc;
using SabTool.Serializers.Misc;

public sealed partial class ResourceDepot
{
    public Heightmap? Heightmap { get; private set; }
    public Waterflow? Waterflow { get; private set; }
    public Freeplay? Freeplay { get; private set; }
    public Watercontrol? Watercontrol { get; private set; }
    public List<WaterQuad>? WaterQuads { get; private set; }
    public List<RailwaySpline>? Railway { get; private set; }

    private bool LoadMisc()
    {
        Console.WriteLine("Loading Misc...");

        using var heightmapStream = new FileStream(GetGamePath("France.hei"), FileMode.Open, FileAccess.Read, FileShare.None);
        Heightmap = HeightmapSerializer.DeserializeRaw(heightmapStream);

        using var waterflowStream = new FileStream(GetGamePath("France.waterflow"), FileMode.Open, FileAccess.Read, FileShare.None);
        Waterflow = WaterflowSerializer.DeserializeRaw(waterflowStream);

        using var freeplayStream = new FileStream(GetGamePath("France.freeplay"), FileMode.Open, FileAccess.Read, FileShare.None);
        Freeplay = FreeplaySerializer.DeserializeRaw(freeplayStream);

        using var watercontrolStream = new FileStream(GetGamePath("France.waterctrl"), FileMode.Open, FileAccess.Read, FileShare.None);
        Watercontrol = WatercontrolSerializer.DeserializeRaw(watercontrolStream);
        
        WaterQuads = WaterplanesSerializer.DeserializeRaw(File.ReadAllLines(GetGamePath("water_planes.ini")).ToList());

        using var railwayStream = new FileStream(GetGamePath("France.railway"), FileMode.Open, FileAccess.Read, FileShare.None);
        Railway = RailwaySerializer.DeserializeRaw(railwayStream);

        Console.WriteLine("Misc loaded!");

        LoadedResources |= Resource.Misc;

        return true;
    }
}
