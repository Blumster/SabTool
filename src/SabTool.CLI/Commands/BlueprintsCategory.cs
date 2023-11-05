namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Blueprints;

public sealed class BlueprintsCategory : BaseCategory
{
    public override string Key => "blueprints";
    public override string Shortcut => "b";
    public override string Usage => "<sub command>";

    public sealed class UnpackCommand : BaseCommand
    {
        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
        public override string Usage { get; } = "<game base path> <output directory> [input file path]";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            var outputDir = arguments.ElementAt(1);

            Directory.CreateDirectory(outputDir);

            using var outFileStream = new FileStream(Path.Combine(outputDir, "blueprints.json"), FileMode.Create, FileAccess.Write, FileShare.None);

            if (arguments.Count() >= 3)
            {
                using var inFileStream = new FileStream(arguments.ElementAt(2), FileMode.Open, FileAccess.Read, FileShare.Read);

                BlueprintSerializer.SerializeJSON(BlueprintSerializer.DeserializeRaw(inFileStream), outFileStream);

                return true;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Blueprints);

            BlueprintSerializer.SerializeJSON(ResourceDepot.Instance.GetAllBlueprints().ToList(), outFileStream);

            return true;
        }
    }

    public sealed class PackCommand : BaseCommand
    {
        public override string Key { get; } = "pack";
        public override string Shortcut { get; } = "p";
        public override string Usage { get; } = "<game base path> <input file path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            var inputFile = arguments.ElementAt(1);

            using var inFileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var outFileStream = new FileStream(Path.Combine(Path.GetDirectoryName(inputFile)!, "GameTemplates.wsd"), FileMode.Create, FileAccess.Write, FileShare.None);

            BlueprintSerializer.SerializeRaw(BlueprintSerializer.DeserializeJSON(inFileStream), outFileStream);

            return true;
        }
    }

    public sealed class DumpCommand : BaseCommand
    {
        public override string Key { get; } = "dump";
        public override string Shortcut { get; } = "d";
        public override string Usage { get; } = "<game base path> [output file path]";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (!arguments.Any())
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Blueprints);

            var outputFilePath = arguments.Count() > 1 ? arguments.ElementAt(1) : null;

            var writer = Console.Out;
            var outputToFile = false;

            if (outputFilePath != null)
            {
                writer = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None));
                outputToFile = true;
            }

            var blueprints = ResourceDepot.Instance.GetAllBlueprints();

            foreach (var blueprint in blueprints)
                writer.WriteLine(blueprint.Dump());

            if (writer != null && outputToFile)
            {
                writer.Flush();
                writer.Close();
            }

            return true;
        }
    }
}
