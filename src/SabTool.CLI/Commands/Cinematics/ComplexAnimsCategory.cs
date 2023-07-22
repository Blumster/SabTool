
using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Cinematics;

namespace SabTool.CLI.Commands.Cinematics;
public sealed class ComplexAnimsCategory : BaseCategory
{
    public override string Key { get; } = "complex-anims";
    public override string Shortcut { get; } = "ca";
    public override string Usage { get; } = "";

    public sealed class UnpackCommand : BaseCommand
    {
        private const string ComplexAnimsRootPath = @"Cinematics\ComplexAnimations";

        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
        public override string Usage { get; } = "<game base path> <output dir path> [language]";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            string outputDirectory = arguments.ElementAt(1);
            _ = Directory.CreateDirectory(outputDirectory);

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            _ = ResourceDepot.Instance.Load(Resource.Cinematics);

            string outputFilePath = Path.Combine(outputDirectory, ComplexAnimsRootPath, "complex-anims.json");

            string outputFileDirectory = Path.GetDirectoryName(outputFilePath)!;
            _ = Directory.CreateDirectory(outputFileDirectory);

            using FileStream fs = new(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

            ComplexAnimsSerializer.SerializeJSON(ResourceDepot.Instance.GetComplexAnimStructures().ToList(), fs);

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
            Console.WriteLine("NOT YET IMPLEMENTED!");
            return false;
        }
    }
}
