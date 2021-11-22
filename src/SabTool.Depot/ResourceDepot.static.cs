namespace SabTool.Depot
{
    public partial class ResourceDepot
    {
        private static readonly Lazy<ResourceDepot> _resourceDepotInstance = new(() => new ResourceDepot());

        public static ResourceDepot Instance
        {
            get
            {
                return _resourceDepotInstance.Value;
            }
        }

        public static bool IsInitialized
        {
            get
            {
                return _resourceDepotInstance.IsValueCreated && !string.IsNullOrWhiteSpace(_resourceDepotInstance.Value.GamePath);
            }
        }

        private record ResourceLoadingInfo(Func<bool> LoaderFunction, HashSet<Resource> Dependencies);
        private static Dictionary<Resource, ResourceLoadingInfo> LoadingInfos { get; } = new()
        {
            { Resource.Megapacks,  new(() => Instance.LoadMegapacks(),  new() { Resource.Maps }) },
            { Resource.Materials,  new(() => Instance.LoadMaterials(),  new() { Resource.None }) },
            { Resource.Shaders,    new(() => Instance.LoadShaders(),    new() { Resource.LooseFiles }) },
            { Resource.LooseFiles, new(() => Instance.LoadLooseFiles(), new() { Resource.None }) },
            { Resource.Maps,       new(() => Instance.LoadMaps(),       new() { Resource.LooseFiles }) },
            { Resource.Blueprints, new(() => Instance.LoadBlueprints(), new() { Resource.LooseFiles }) },
        };

        private static HashSet<Resource> CollectAllDependencies(Resource resource, HashSet<Resource>? container = null)
        {
            if (container == null)
                container = new HashSet<Resource>();

            if (resource == Resource.None)
                return container;

            if (!LoadingInfos.TryGetValue(resource, out var info))
                throw new Exception($"Invalid or non-mapped resource found: {resource}!");

            foreach (var dependency in info.Dependencies)
            {
                if (dependency != Resource.None && !container.Contains(dependency))
                {
                    container.Add(dependency);

                    CollectAllDependencies(dependency, container);
                }
            }

            return container;
        }
    }
}
