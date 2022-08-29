using System.Globalization;
using System.Text.RegularExpressions;

namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Megapacks;
using SabTool.Serializers.Packs;
using SabTool.Utils;

public class PackCategory : BaseCategory
{
    public override string Key { get; } = "pack";
    public override string Shortcut { get; } = "p";
    public override string Usage { get; } = "<sub command>";

    public class UnpackCommand : BaseCommand
    {
        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
        public override string Usage { get; } = "<game base path> <packs file path> [output directory]";

        private static readonly Regex ChunkRegex = new(@"[fF]rance\\(?:(?:\d\d\\\d+)|(?:\d+))\.[pP]ack$", RegexOptions.Compiled);

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Maps);

            var packsBaseDir = arguments.ElementAt(1);
            var outputDir = Path.GetDirectoryName(packsBaseDir);

            if (Directory.Exists(packsBaseDir))
                outputDir = packsBaseDir;

            if (arguments.Count() > 2)
                outputDir = arguments.ElementAt(2);

            if (outputDir == null)
            {
                Console.WriteLine("ERROR: No output directory is given!");
                return false;
            }

            Directory.CreateDirectory(outputDir);

            foreach (var pack in Directory.GetFiles(packsBaseDir, "*.*", SearchOption.AllDirectories))
                ProcessPack(pack, Path.Combine(outputDir, Path.GetDirectoryName(pack)![(packsBaseDir.Length + 1)..]));

            return true;
        }

        private static bool ProcessPack(string filePath, string outputDir)
        {
            Console.WriteLine($"Processing {filePath}...");

            Crc crc;

            var fileName = Path.GetFileNameWithoutExtension(filePath);

            if (ChunkRegex.IsMatch(filePath.ToLowerInvariant()))
                crc = new Crc(uint.Parse(fileName));
            else if (fileName.StartsWith("0x"))
                crc = new Crc(uint.Parse(fileName[2..], NumberStyles.HexNumber));
            else
                crc = new Crc(Hash.StringToHash(fileName));

            var streamBlock = ResourceDepot.Instance.GetStreamBlock(crc);
            if (streamBlock == null)
            {
                Console.WriteLine($"StreamBlock {crc} was not found!");
                return false;
            }

            outputDir = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(filePath));
            outputDir = outputDir.Replace(" ", "");

            Directory.CreateDirectory(outputDir);

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using var reader = new BinaryReader(fs);

                PackSerializer.DeserializeRaw(fs, streamBlock, true);

                StreamBlockSerializer.Export(streamBlock, outputDir);
            }

            return true;
        }
    }
}
