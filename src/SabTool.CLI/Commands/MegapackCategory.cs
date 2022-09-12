using System.Text;

namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;
using SabTool.Depot;

public class MegapackCategory : BaseCategory
{
    public override string Key => "megapack";

    public override string Shortcut => "m";

    public override string Usage => "<sub command>";

    public class ListCommand : BaseCommand
    {
        public override string Key { get; } = "list";
        public override string Shortcut { get; } = "l";
        public override string Usage { get; } = "<game base path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (!arguments.Any())
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Megapacks);

            var writer = Console.Out;

            if (arguments.Count() == 2)
                writer = new StreamWriter(arguments.ElementAt(1), false, Encoding.UTF8);

            foreach (var megapackFile in ResourceDepot.Instance.GetMegapacks())
            {
                writer.WriteLine($"Megapack: {megapackFile}");

                foreach (var fileEntry in ResourceDepot.Instance.GetFileEntries(megapackFile))
                    writer.WriteLine(fileEntry.ToPrettyString());

                writer.WriteLine();
                writer.WriteLine($"Mappings:");

                foreach (var (crc1, crc2) in ResourceDepot.Instance.GetBlockPathToNameCrcs(megapackFile))
                    writer.WriteLine($"{crc1} -> {crc2}");

                writer.WriteLine();
            }
            
            writer.Flush();

            if (writer != Console.Out)
                writer.Close();

            return true;
        }
    }

    public class UnpackCommand : BaseCommand
    {
        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
        public override string Usage { get; } = "<game base path> <output directory> [relative megapack file path]";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Megapacks);

            var outputDir = arguments.ElementAt(1);

            Directory.CreateDirectory(outputDir);

            string? fileToUnpack = null;

            if (arguments.Count() == 3)
                fileToUnpack = arguments.ElementAt(2);

            foreach (var megapackFile in ResourceDepot.Instance.GetMegapacks())
            {
                if (!string.IsNullOrEmpty(fileToUnpack) && megapackFile != fileToUnpack)
                    continue;

                Console.WriteLine($"Unpacking {megapackFile}...");
                
                var megapackOutputDir = Path.Combine(outputDir, megapackFile);

                var i = 0;

                foreach (var fileEntry in ResourceDepot.Instance.GetFileEntries(megapackFile))
                {
                    var entryStream = ResourceDepot.Instance.GetPackStream(fileEntry.Path);
                    if (entryStream == null)
                    {
                        Console.WriteLine($"Unable to unpack {fileEntry.Path}, no data fround for it!");
                        continue;
                    }

                    var realFilePath = fileEntry.Path.GetString();
                    if (string.IsNullOrEmpty(realFilePath))
                        realFilePath = $"{fileEntry.Name.GetStringOrHexString()}.pack"; // TODO: based on the streamblock, can be dynpack, palettepack or pack

                    var outputFilePath = Path.Combine(megapackOutputDir, realFilePath);

                    var outputFileDirectory = Path.GetDirectoryName(outputFilePath);
                    if (outputFileDirectory == null)
                    {
                        Console.WriteLine("ERROR: Output directory is invalid!");
                        return false;
                    }

                    Directory.CreateDirectory(outputFileDirectory);

                    using var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                    entryStream.CopyTo(fs);

                    ++i;
                }

                Console.WriteLine($"Successfully unpacked {megapackFile} to {megapackOutputDir} ({i} files)!");
            }

            return true;
        }
    }

    public class PackCommand : BaseCommand
    {
        public override string Key { get; } = "pack";
        public override string Shortcut { get; } = "p";
        public override string Usage { get; } = "<game base path> <input directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            return true;
        }
    }
}
