
using SabTool.Utils.Extensions;

namespace SabTool.Depot;
[Flags]
public enum Resource
{
    None = 0x0000,
    Megapacks = 0x0001,
    Materials = 0x0002,
    Shaders = 0x0004,
    LooseFiles = 0x0008,
    Maps = 0x0010,
    Blueprints = 0x0020,
    Cinematics = 0x0040,
    Sounds = 0x0080,
    Lua = 0x0100,
    Misc = 0x0200,
    Animations = 0x0400,

    All = Megapacks | Materials | Shaders | LooseFiles | Maps | Blueprints | Cinematics | Sounds | Lua | Misc | Animations,
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
        return resources == Resource.None ? LoadedResources : LoadedResources & resources;
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
        HashSet<Resource> resourceOrder = new();

        foreach (KeyValuePair<Resource, ResourceLoadingInfo> info in LoadingInfos)
        {
            if ((resources & info.Key) == info.Key)
                _ = resourceOrder.Add(info.Key);
        }

        HashSet<Resource> toAdd = new();
        foreach (Resource resource in resourceOrder)
            _ = CollectAllDependencies(resource, toAdd);

        foreach (Resource resource in toAdd)
            _ = resourceOrder.Add(resource);

        // TODO: later: async tasks to load them parallel

        IEnumerable<Resource> loadOrder = resourceOrder.TopologicalSort(resource => CollectAllDependencies(resource), Resource.None);
        foreach (Resource load in loadOrder)
            _ = LoadResource(load);

        return true;
    }

    private bool LoadResource(Resource resource)
    {
        if (IsResourceLoaded(resource))
            return true;

        if (!LoadingInfos.TryGetValue(resource, out ResourceLoadingInfo? info))
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
