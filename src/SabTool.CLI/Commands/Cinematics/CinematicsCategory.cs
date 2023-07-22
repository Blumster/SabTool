
using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Cinematics;

namespace SabTool.CLI.Commands.Cinematics;
public sealed class CinematicsCategory : BaseCategory
{
    public override string Key { get; } = "cinematics";
    public override string Shortcut { get; } = "c";
    public override string Usage { get; } = "";

    public sealed class UnpackCommand : BaseCommand
    {
        private const string CinematicsRootPath = @"Cinematics";
        private const string CienmaticsRootPathDLC = @"DLC\01\Cinematics";

        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
        public override string Usage { get; } = "<game base path> <output dir path>";

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

            // Main
            string outputFilePath = Path.Combine(outputDirectory, CinematicsRootPath, "cinematics.json");

            string? outputFileDirectory = Path.GetDirectoryName(outputFilePath);
            if (outputFileDirectory == null)
            {
                Console.WriteLine("ERROR: Output directory is invalid!");
                return false;
            }

            _ = Directory.CreateDirectory(outputFileDirectory);

            using FileStream fs = new(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

            CinematicsSerializer.SerializeJSON(ResourceDepot.Instance.GetCinematics().ToList(), fs);

            // DLC
            outputFilePath = Path.Combine(outputDirectory, CienmaticsRootPathDLC, "cinematics.json");

            outputFileDirectory = Path.GetDirectoryName(outputFilePath);
            if (outputFileDirectory == null)
            {
                Console.WriteLine("ERROR: Output directory is invalid!");
                return false;
            }

            _ = Directory.CreateDirectory(outputFileDirectory);

            using FileStream fsDLC = new(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

            CinematicsSerializer.SerializeJSON(ResourceDepot.Instance.GetDLCCinematics().ToList(), fsDLC);

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
