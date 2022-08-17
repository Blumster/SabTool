namespace SabTool.Depot
{
    using Data.Packs;
    using SabTool.Serializers.Packs;
    using Serializers.Megapacks;
    using Utils;

    public partial class ResourceDepot
    {
        public GlobalMap? GlobalMap { get; set; }
        public GlobalMap? DLCGlobalMap { get; set; }
        public FranceMap? FranceMap { get; set; }

        public bool LoadMaps()
        {
            Console.WriteLine("Loading Maps...");

            LoadGlobalMap();
            LoadDLCGlobalMap();
            LoadFranceMap();
            LoadDLCFranceMap();

            LoadedResources |= Resource.Maps;

            Console.WriteLine("Maps loaded!");

            return true;
        }

        private void LoadGlobalMap()
        {
            Console.WriteLine("  Loading Global Map...");

            using var globalMapStream = GetLooseFile("global.map") ?? throw new Exception($"global.map is missing from {LooseFilesFileName}!");

            GlobalMap = GlobalMapSerializer.DeserializeRaw(globalMapStream);

            Console.WriteLine("  Global Map loaded!");
        }

        private void LoadDLCGlobalMap()
        {
            Console.WriteLine("  Loading DLC Global Map...");

            var DLCGlobalMapPath = GetGamePath(@"DLC\01\Global.map");

            using var DLCGlobalMapStream = new FileStream(DLCGlobalMapPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            DLCGlobalMap = GlobalMapSerializer.DeserializeRaw(DLCGlobalMapStream);

            Console.WriteLine("  DLC Global Map loaded!");
        }

        private void LoadFranceMap()
        {
            Console.WriteLine("  Loading France Map...");

            using var franceMapStream = GetLooseFile("France.map") ?? throw new Exception($"france.map is missing from {LooseFilesFileName}!");

            FranceMap = FranceMapSerializer.DeserializeRaw(franceMapStream);

            Console.WriteLine("  France Map loaded!");
        }

        private void LoadDLCFranceMap()
        {
            Console.WriteLine("Loading DLC France Map...");

            var DLCFranceMapPath = GetGamePath(@"DLC\01\FRANCE.map");

            using var DLCFranceMapStream = new FileStream(DLCFranceMapPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            FranceMapSerializer.DeserializeRaw(DLCFranceMapStream, FranceMap);

            Console.WriteLine("  DLC France Map loaded!");
        }

        public StreamBlock? GetDynamicBlock(Crc crc, bool canCheckDLC = true)
        {
            if (!IsResourceLoaded(Resource.Maps))
                Load(Resource.Maps);

            var block = GlobalMap!.GetDynamicBlock(crc);
            if (block == null && canCheckDLC)
                block = DLCGlobalMap!.GetDynamicBlock(crc);

            return block;
        }

        public StreamBlock? GetStreamBlock(Crc crc)
        {
            if (!IsResourceLoaded(Resource.Maps))
                Load(Resource.Maps);

            var dynBlock = GlobalMap!.GetDynamicBlock(crc);
            if (dynBlock != null)
                return dynBlock;

            dynBlock = DLCGlobalMap!.GetDynamicBlock(crc);
            if (dynBlock != null)
                return dynBlock;

            var staticBlock = GlobalMap.GetStaticBlock(crc);
            if (staticBlock != null)
                return staticBlock;

            if (FranceMap!.Interiors.TryGetValue(crc, out var interiorBlock))
                return interiorBlock;

            if (FranceMap!.CinematicBlocks.TryGetValue(crc, out var cinematicBlock))
                return cinematicBlock;

            return null;
        }
    }
}
