namespace SabTool.Depot;

using SabTool.Utils.Extensions;

[Flags]
public enum Resource
{
    None       = 0x0000,
    Megapacks  = 0x0001,
    Materials  = 0x0002,
    Shaders    = 0x0004,
    LooseFiles = 0x0008,
    Maps       = 0x0010,
    Blueprints = 0x0020,
    Cinematics = 0x0040,
    Sounds     = 0x0080,
    Lua        = 0x0100,
    Misc       = 0x0200,
    Animations = 0x0400,

    All        = Megapacks | Materials | Shaders | LooseFiles | Maps | Blueprints | Cinematics | Sounds | Lua | Misc | Animations,
}

public sealed partial class ResourceDepot
{
    private const string GameExecutableName = "Saboteur.exe";

    public string GamePath { get; private set; }
    public Resource LoadedResources { get; private set; }

    private ResourceDepot()
    {
        GamePath = "";
        LoadedResources = Resource.None;
    }

    public void Initialize(string gamePath)
    {
        if (IsInitialized)
            return;

        GamePath = gamePath;
    }

    public Resource GetLoadedResources(Resource resources = Resource.None)
    {
        if (resources == Resource.None)
            return LoadedResources;

        return LoadedResources & resources;
    }

    public Resource GetNotLoadedResources(Resource resources = Resource.None)
    {
        if (resources == Resource.None)
            resources = Resource.All;

        return ~LoadedResources & resources;
    }

    public bool IsResourceLoaded(Resource resource)
    {
        return (LoadedResources & resource) != Resource.None;
    }

    public bool Load(Resource resources)
    {
        if (string.IsNullOrEmpty(GamePath))
        {
            Console.WriteLine("ResourceDepot: No GamePath is specified!");
            return false;
        }

        if (!Directory.Exists(GamePath))
        {
            Console.WriteLine("ResourceDepot: The specified GamePath doesn't exist!");
            return false;
        }

        if (!File.Exists(GetGamePath(GameExecutableName)))
        {
            Console.WriteLine("ResourceDepot: The specified GamePath doesn't exist!");
            return false;
        }

        return LoadResources(resources);
    }

    private bool LoadResources(Resource resources)
    {
        var resourceOrder = new HashSet<Resource>();

        foreach (var info in LoadingInfos)
        {
            if ((resources & info.Key) == info.Key)
                resourceOrder.Add(info.Key);
        }

        var toAdd = new HashSet<Resource>();
        foreach (var resource in resourceOrder)
            CollectAllDependencies(resource, toAdd);

        foreach (var resource in toAdd)
            resourceOrder.Add(resource);

        // TODO: later: async tasks to load them parallel

        var loadOrder = resourceOrder.TopologicalSort(resource => CollectAllDependencies(resource), Resource.None);
        foreach (var load in loadOrder)
            LoadResource(load);

        return true;
    }

    private bool LoadResource(Resource resource)
    {
        if (IsResourceLoaded(resource))
            return true;

        if (!LoadingInfos.TryGetValue(resource, out var info))
            return false;

        if (!info.LoaderFunction())
            return false;

        LoadedResources |= resource;

        return true;
    }

    private string GetGamePath(string path)
    {
        return Path.Combine(GamePath, path);
    }
}
