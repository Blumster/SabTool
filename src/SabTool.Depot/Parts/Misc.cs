
using SabTool.Data.Misc;
using SabTool.Serializers.Misc;

namespace SabTool.Depot;
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

        using FileStream heightmapStream = new(GetGamePath("France.hei"), FileMode.Open, FileAccess.Read, FileShare.None);
        Heightmap = HeightmapSerializer.DeserializeRaw(heightmapStream);

        using FileStream waterflowStream = new(GetGamePath("France.waterflow"), FileMode.Open, FileAccess.Read, FileShare.None);
        Waterflow = WaterflowSerializer.DeserializeRaw(waterflowStream);

        using FileStream freeplayStream = new(GetGamePath("France.freeplay"), FileMode.Open, FileAccess.Read, FileShare.None);
        Freeplay = FreeplaySerializer.DeserializeRaw(freeplayStream);

        using FileStream watercontrolStream = new(GetGamePath("France.waterctrl"), FileMode.Open, FileAccess.Read, FileShare.None);
        Watercontrol = WatercontrolSerializer.DeserializeRaw(watercontrolStream);

        WaterQuads = WaterplanesSerializer.DeserializeRaw(File.ReadAllLines(GetGamePath("water_planes.ini")).ToList());

        using FileStream railwayStream = new(GetGamePath("France.railway"), FileMode.Open, FileAccess.Read, FileShare.None);
        Railway = RailwaySerializer.DeserializeRaw(railwayStream);

        Console.WriteLine("Misc loaded!");

        LoadedResources |= Resource.Misc;

        return true;
    }
}
