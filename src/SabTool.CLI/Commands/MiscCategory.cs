namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Misc;
using SabTool.Utils;

public sealed class MiscCategory : BaseCategory
{
    public override string Key => "misc";
    public override string Shortcut => "mi";
    public override string Usage => "<sub command name>";

    public sealed class UnpackHeightmapCommand : BaseCommand
    {
        public override string Key { get; } = "unpack-heightmap";
        public override string Shortcut { get; } = "uheightmap";
        public override string Usage { get; } = "<game base path> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Misc);

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            using var outFile = new FileStream(Path.Combine(outputFolder, "heightmap.json"), FileMode.Create, FileAccess.Write, FileShare.None);

            HeightmapSerializer.SerializeJSON(ResourceDepot.Instance.Heightmap!, outFile);

            return true;
        }
    }

    public sealed class ExportGltfFromWaterflowCommand : BaseCommand
    {
        public override string Key => "export-waterflow-to-gltf";
        public override string Shortcut => "ewaterflowgltf";
        public override string Usage => "<game base path> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Misc);

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            WaterflowSerializer.ExportGltf(ResourceDepot.Instance.Waterflow!, outputFolder);

            return true;
        }
    }

    public sealed class ExportGltfFromWatercontrolCommand : BaseCommand
    {
        public override string Key => "export-watercontrol-to-gltf";
        public override string Shortcut => "ewatercontrolgltf";
        public override string Usage => "<game base path> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Misc);

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            WatercontrolSerializer.ExportGltf(ResourceDepot.Instance.Watercontrol!, outputFolder);

            return true;
        }
    }

    public sealed class ExportGltfFromFreeplayCommand : BaseCommand
    {
        public override string Key => "export-freeplay-to-gltf";
        public override string Shortcut => "efreeplaygltf";
        public override string Usage => "<game base path> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Misc);

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            FreeplaySerializer.ExportGltf(ResourceDepot.Instance.Freeplay!, outputFolder);

            return true;
        }
    }

    public sealed class ExportPlyFromHeiCommand : BaseCommand
    {
        public override string Key => "export-heightmap-to-ply";
        public override string Shortcut => "eheightmapply";
        public override string Usage => "<game base path> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Misc);

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            using var outFile = new FileStream(Path.Combine(outputFolder, "heightmap.ply"), FileMode.Create, FileAccess.Write, FileShare.None);

            HeightmapSerializer.ExportPly(ResourceDepot.Instance.Heightmap!, outFile);

            return true;
        }
    }

    public sealed class ExportGltfFromWaterplanesCommand : BaseCommand
    {
        public override string Key => "export-waterplanes-to-gltf";
        public override string Shortcut => "ewaterplanesgltf";
        public override string Usage => "<game base path> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Misc);

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            WaterplanesSerializer.ExportGltf(ResourceDepot.Instance.WaterQuads!, outputFolder);
            return true;
        }
    }

    public sealed class ExportGltfMeshFromHeightmapCommand : BaseCommand
    {
        public override string Key => "export-heightmap-to-gltfl";
        public override string Shortcut => "eheightmapgltf";
        public override string Usage => "<game base path> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Misc);

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            HeightmapSerializer.ExportGltf(ResourceDepot.Instance.Heightmap!, outputFolder, false);
            return true;
        }
    }

    public sealed class ExportGltfMeshMergedFromHeightmapCommand : BaseCommand
    {
        public override string Key => "export-heightmap-merged-to-gltfl";
        public override string Shortcut => "eheightmapmergedgltf";
        public override string Usage => "<game base path> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Misc);

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            HeightmapSerializer.ExportGltf(ResourceDepot.Instance.Heightmap!, outputFolder, true);
            return true;
        }
    }

    public sealed class ExportGltfMeshFromPacksCommand : BaseCommand
    {
        public override string Key => "export-heightmap-packs-to-gltfl";
        public override string Shortcut => "eheightmappacksgltf";
        public override string Usage => "<path to extracted megapack1 and 2> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            HeightmapSerializer.ExportAllCellsInPacks(arguments.ElementAt(0), outputFolder);
            return true;
        }
    }

    public sealed class ExportGltfSplinePointsFromRailways : BaseCommand
    {
        public override string Key => "export-railway-splinepoints-to-gltf";
        public override string Shortcut => "erailwaysplinepointsgltf";
        public override string Usage => "<game base path> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Misc);

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            RailwaySerializer.ExportGltfSplinePoints(ResourceDepot.Instance.Railway!, outputFolder);
            return true;
        }
    }   
}
