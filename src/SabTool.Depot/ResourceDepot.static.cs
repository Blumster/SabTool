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

        private record ResourceLoadingInfo(Func<bool, bool> LoaderFunction, HashSet<Resource> Dependencies);
        private static Dictionary<Resource, ResourceLoadingInfo> LoadingInfos { get; } = new()
        {
            { Resource.Megapacks,  new(LoadMegapacksStatic,  new() { Resource.Maps }) },
            { Resource.Materials,  new(LoadMaterialsStatic,  new() { Resource.None }) },
            { Resource.Shaders,    new(LoadShadersStatic,    new() { Resource.LooseFiles }) },
            { Resource.LooseFiles, new(LoadLooseFilesStatic, new() { Resource.None }) },
            { Resource.Maps,       new(LoadMapsStatic,       new() { Resource.LooseFiles }) },
            { Resource.Blueprints, new(LoadBlueprintsStatic, new() { Resource.LooseFiles }) },
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

        private static bool LoadMegapacksStatic(bool reload)
        {
            return Instance.LoadMegapacks(reload);
        }

        private static bool LoadMaterialsStatic(bool reload)
        {
            return Instance.LoadMaterials(reload);
        }

        private static bool LoadShadersStatic(bool reload)
        {
            return Instance.LoadShaders(reload);
        }

        private static bool LoadLooseFilesStatic(bool reload)
        {
            return Instance.LoadLooseFiles(reload);
        }

        private static bool LoadMapsStatic(bool reload)
        {
            return Instance.LoadMaps(reload);
        }

        private static bool LoadBlueprintsStatic(bool reload)
        {
            return Instance.LoadBlueprints(reload);
        }
    }
}
