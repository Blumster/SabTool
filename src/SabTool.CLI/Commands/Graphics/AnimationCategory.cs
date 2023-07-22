
using SabTool.CLI.Base;
using SabTool.Depot;

namespace SabTool.CLI.Commands.Graphics;
public sealed class AnimationCategory : BaseCategory
{
    public override string Key => "animation";
    public override string Shortcut => "a";
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
            _ = ResourceDepot.Instance.Load(Resource.Animations);

            string outputDir = arguments.ElementAt(1);

            _ = Directory.CreateDirectory(outputDir);

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

            string inputFilePath = arguments.ElementAt(0);
            string outputDir = arguments.ElementAt(1);

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("ERROR: The materials input file path does not exist!");
                return false;
            }

            _ = Directory.CreateDirectory(outputDir);

            using FileStream inFileStream = new(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return true;
        }
    }
}
