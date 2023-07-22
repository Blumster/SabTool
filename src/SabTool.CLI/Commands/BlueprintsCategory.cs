
using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Blueprints;

namespace SabTool.CLI.Commands;
public sealed class BlueprintsCategory : BaseCategory
{
    public override string Key => "blueprints";
    public override string Shortcut => "b";
    public override string Usage => "<sub command>";

    public sealed class UnpackCommand : BaseCommand
    {
        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
        public override string Usage { get; } = "<game base path> <output directory>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            _ = ResourceDepot.Instance.Load(Resource.Blueprints);

            string outputDir = arguments.ElementAt(1);

            _ = Directory.CreateDirectory(outputDir);

            List<Data.Blueprints.Blueprint> blueprints = ResourceDepot.Instance.GetAllBlueprints().ToList();

            using FileStream outFileStream = new(Path.Combine(outputDir, "blueprints.json"), FileMode.Create, FileAccess.Write, FileShare.None);

            BlueprintSerializer.SerializeJSON(blueprints, outFileStream);

            return true;
        }
    }

    public sealed class PackCommand : BaseCommand
    {
        public override string Key { get; } = "pack";
        public override string Shortcut { get; } = "p";
        public override string Usage { get; } = "<game base path> <input directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
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
            _ = ResourceDepot.Instance.Load(Resource.Blueprints);

            string? outputFilePath = arguments.Count() > 1 ? arguments.ElementAt(1) : null;

            TextWriter writer = Console.Out;
            bool outputToFile = false;

            if (outputFilePath != null)
            {
                writer = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None));
                outputToFile = true;
            }

            IEnumerable<Data.Blueprints.Blueprint> blueprints = ResourceDepot.Instance.GetAllBlueprints();

            foreach (Data.Blueprints.Blueprint blueprint in blueprints)
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
