namespace SabTool.Depot;

public partial class ResourceDepot
{
    private static readonly Lazy<ResourceDepot> _resourceDepotInstance = new(() => new ResourceDepot());

    public static ResourceDepot Instance => _resourceDepotInstance.Value;
    public static bool IsInitialized => _resourceDepotInstance.IsValueCreated && !string.IsNullOrWhiteSpace(_resourceDepotInstance.Value.GamePath);

    private record ResourceLoadingInfo(Func<bool> LoaderFunction, HashSet<Resource> Dependencies);
    private static Dictionary<Resource, ResourceLoadingInfo> LoadingInfos { get; } = new()
    {
        { Resource.Megapacks,  new(() => Instance.LoadMegapacks(),  new() { Resource.None }) },
        { Resource.Materials,  new(() => Instance.LoadMaterials(),  new() { Resource.None }) },
        { Resource.Shaders,    new(() => Instance.LoadShaders(),    new() { Resource.LooseFiles }) },
        { Resource.LooseFiles, new(() => Instance.LoadLooseFiles(), new() { Resource.None }) },
        { Resource.Maps,       new(() => Instance.LoadMaps(),       new() { Resource.LooseFiles }) },
        { Resource.Blueprints, new(() => Instance.LoadBlueprints(), new() { Resource.LooseFiles }) },
        { Resource.Cinematics, new(() => Instance.LoadCinematics(), new() { Resource.LooseFiles }) },
        { Resource.Sounds,     new(() => Instance.LoadSounds(),     new() { Resource.None }) },
        { Resource.Lua,        new(() => Instance.LoadLua(),        new() { Resource.None }) },
        { Resource.Misc,       new(() => Instance.LoadMisc(),       new() { Resource.None }) },
        { Resource.Animations, new(() => Instance.LoadAnimations(), new() { Resource.None }) },
    };

    private static HashSet<Resource> CollectAllDependencies(Resource resource, HashSet<Resource>? container = null)
    {
        container ??= new HashSet<Resource>();

        if (resource == Resource.None)
            return container;

        if (!LoadingInfos.TryGetValue(resource, out ResourceLoadingInfo? info))
            throw new Exception($"Invalid or non-mapped resource found: {resource}!");

        foreach (Resource dependency in info.Dependencies)
        {
            if (dependency != Resource.None && !container.Contains(dependency))
            {
                _ = container.Add(dependency);

                _ = CollectAllDependencies(dependency, container);
            }
        }

        return container;
    }
}
