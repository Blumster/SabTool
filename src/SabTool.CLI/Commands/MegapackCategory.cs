using System.Text;

using SabTool.CLI.Base;
using SabTool.Depot;

namespace SabTool.CLI.Commands;
public sealed class MegapackCategory : BaseCategory
{
    public override string Key => "megapack";
    public override string Shortcut => "m";
    public override string Usage => "<sub command>";

    public sealed class ListCommand : BaseCommand
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
            _ = ResourceDepot.Instance.Load(Resource.Megapacks);

            TextWriter writer = Console.Out;

            if (arguments.Count() == 2)
                writer = new StreamWriter(arguments.ElementAt(1), false, Encoding.UTF8);

            foreach (string megapackFile in ResourceDepot.Instance.GetMegapacks())
            {
                writer.WriteLine($"Megapack: {megapackFile}");

                foreach (Data.Packs.FileEntry fileEntry in ResourceDepot.Instance.GetFileEntries(megapackFile))
                    writer.WriteLine(fileEntry.ToPrettyString());

                writer.WriteLine();
                writer.WriteLine($"Mappings:");

                foreach ((Utils.Crc crc1, Utils.Crc crc2) in ResourceDepot.Instance.GetBlockPathToNameCrcs(megapackFile))
                    writer.WriteLine($"{crc1} -> {crc2}");

                writer.WriteLine();
            }

            writer.Flush();

            if (writer != Console.Out)
                writer.Close();

            return true;
        }
    }

    public sealed class UnpackCommand : BaseCommand
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
            _ = ResourceDepot.Instance.Load(Resource.Megapacks);

            string outputDir = arguments.ElementAt(1);

            _ = Directory.CreateDirectory(outputDir);

            string? fileToUnpack = null;

            if (arguments.Count() == 3)
                fileToUnpack = arguments.ElementAt(2);

            foreach (string megapackFile in ResourceDepot.Instance.GetMegapacks())
            {
                if (!string.IsNullOrEmpty(fileToUnpack) && megapackFile != fileToUnpack)
                    continue;

                Console.WriteLine($"Unpacking {megapackFile}...");

                string megapackOutputDir = Path.Combine(outputDir, megapackFile);

                int i = 0;

                foreach (Data.Packs.FileEntry fileEntry in ResourceDepot.Instance.GetFileEntries(megapackFile))
                {
                    MemoryStream? entryStream = ResourceDepot.Instance.GetPackStream(fileEntry.Path);
                    if (entryStream == null)
                    {
                        Console.WriteLine($"Unable to unpack {fileEntry.Path}, no data fround for it!");
                        continue;
                    }

                    string realFilePath = fileEntry.Path.GetString();
                    if (string.IsNullOrEmpty(realFilePath))
                        realFilePath = $"{fileEntry.Name.GetStringOrHexString()}.pack"; // TODO: based on the streamblock, can be dynpack, palettepack or pack

                    string outputFilePath = Path.Combine(megapackOutputDir, realFilePath);

                    string? outputFileDirectory = Path.GetDirectoryName(outputFilePath);
                    if (outputFileDirectory == null)
                    {
                        Console.WriteLine("ERROR: Output directory is invalid!");
                        return false;
                    }

                    _ = Directory.CreateDirectory(outputFileDirectory);

                    using FileStream fs = new(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                    entryStream.CopyTo(fs);

                    ++i;
                }

                Console.WriteLine($"Successfully unpacked {megapackFile} to {megapackOutputDir} ({i} files)!");
            }

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
}
