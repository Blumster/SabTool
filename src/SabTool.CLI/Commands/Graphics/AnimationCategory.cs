namespace SabTool.CLI.Commands.Graphics;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Animations;

public sealed class AnimationCategory : BaseCategory
{
    public override string Key => "animation";
    public override string Shortcut => "a";
    public override string Usage => "<sub command>";

    public sealed class DumpCommand : BaseCommand
    {
        public override string Key { get; } = "dump";
        public override string Shortcut { get; } = "d";
        public override string Usage { get; } = "<game base path> <output directory>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Animations);

            var outputDir = arguments.ElementAt(1);

            Directory.CreateDirectory(outputDir);

            using var fs = new FileStream(Path.Combine(outputDir, "animation-pack-dump.txt"), FileMode.Create, FileAccess.Write, FileShare.None);

            AnimationsContainerSerializer.SerializeJSON(ResourceDepot.Instance.AnimationsContainer, fs);

            return true;
        }
    }

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
            ResourceDepot.Instance.Load(Resource.Animations);

            var outputDir = arguments.ElementAt(1);

            Directory.CreateDirectory(outputDir);

            return true;
        }
    }

    public sealed class PackCommand : BaseCommand
    {
        public override string Key { get; } = "pack";
        public override string Shortcut { get; } = "p";
        public override string Usage { get; } = "<input file path> <output dir path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            var inputFilePath = arguments.ElementAt(0);
            var outputDir = arguments.ElementAt(1);

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("ERROR: The materials input file path does not exist!");
                return false;
            }

            Directory.CreateDirectory(outputDir);

            using var inFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return true;
        }
    }
}
