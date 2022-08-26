namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Misc;

public class MiscCategory : BaseCategory
{
    public override string Key => "misc";
    public override string Shortcut => "mi";
    public override string Usage => "<sub command name>";

    public class UnpackHeightmapCommand : BaseCommand
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

    public class ExportGltfFromWaterflowCommand : BaseCommand
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

    public class ExportGltfFromFreeplayCommand : BaseCommand
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

    public class ExportPlyFromHeiCommand : BaseCommand
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

            using (FileStream outFile = new(Path.Combine(outputFolder, "heightmap.ply"), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                HeightmapSerializer.ExportPly(ResourceDepot.Instance.Heightmap!, outFile);
            }
            return true;
        }
    }
}
