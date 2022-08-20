namespace SabTool.Depot;

using SabTool.Data.Misc;
using SabTool.Serializers.Misc;

public partial class ResourceDepot
{
    public Hei5Container? Hei5Container { get; private set; }

    private bool LoadMisc()
    {
        Console.WriteLine("Loading Misc...");

        using var hei5Stream = new FileStream(GetGamePath("France.hei"), FileMode.Open, FileAccess.Read, FileShare.Read);

        Hei5Container = Hei5Serializer.DeserializeRaw(hei5Stream);

        Console.WriteLine("Misc loaded!");

        LoadedResources |= Resource.Misc;

        return true;
    }
}
