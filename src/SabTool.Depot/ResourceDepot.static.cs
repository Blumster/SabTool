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

        private static bool LoadMegapacksStatic()
        {
            return Instance.LoadMegapacks();
        }

        private static bool LoadMaterialsStatic()
        {
            return Instance.LoadMaterials();
        }

        private static bool LoadShadersStatic()
        {
            return Instance.LoadShaders();
        }

        private static bool LoadLooseFilesStatic()
        {
            return Instance.LoadLooseFiles();
        }

        private static bool LoadMapsStatic()
        {
            return Instance.LoadMaps();
        }

        private static bool LoadBlueprintsStatic()
        {
            return Instance.LoadBlueprints();
        }
    }
}
