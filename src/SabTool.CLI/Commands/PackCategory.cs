using System.Globalization;

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

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Megapacks);

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

            foreach (var dynPack in Directory.GetFiles(packsBaseDir, "*.dynpack", SearchOption.AllDirectories))
                ProcessDynpack(dynPack, Path.Combine(outputDir, Path.GetDirectoryName(dynPack)![(packsBaseDir.Length + 1)..]));

            foreach (var palettePack in Directory.GetFiles(packsBaseDir, "*.palettepack", SearchOption.AllDirectories))
                ProcessPalettepack(palettePack, Path.Combine(outputDir, Path.GetDirectoryName(palettePack)![(packsBaseDir.Length + 1)..]));

            foreach (var pack in Directory.GetFiles(packsBaseDir, "*.pack", SearchOption.AllDirectories))
            {
                var outputPath = Path.Combine(outputDir, Path.GetDirectoryName(pack)![(packsBaseDir.Length + 1)..]);
                var packLower = pack.ToLowerInvariant();

                // Hack for unnamed palettepack and dynpack entities
                if (packLower.Contains("palettes"))
                    ProcessPalettepack(pack, outputPath);
                else if (packLower.Contains("dynamic"))
                    ProcessDynpack(pack, outputPath);
            }

            return true;
        }

        private static bool ProcessDynpack(string filePath, string outputDir)
        {
            Console.WriteLine($"Processing {filePath}...");

            Crc crc;

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName.StartsWith("0x"))
                crc = new Crc(uint.Parse(fileName[2..], NumberStyles.HexNumber));
            else
                crc = new Crc(Hash.StringToHash(fileName));

            var streamBlock = ResourceDepot.Instance.GlobalMap!.GetDynamicBlock(crc);
            if (streamBlock == null)
                streamBlock = ResourceDepot.Instance.DLCGlobalMap!.GetDynamicBlock(crc);

            if (streamBlock == null)
            {
                Console.WriteLine($"Dynamic StreamBlock {crc} was not found in global.map or DLC's global.map!");
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

            Console.WriteLine();

            return true;
        }

        private static bool ProcessPalettepack(string filePath, string outputDir)
        {
            Console.WriteLine($"Processing {filePath}...");

            Crc crc;

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName.StartsWith("0x"))
                crc = new Crc(uint.Parse(fileName[2..], NumberStyles.HexNumber));
            else
                crc = new Crc(Hash.StringToHash(fileName));

            var streamBlock = ResourceDepot.Instance.GetStreamBlock(crc);
            if (streamBlock == null)
            {
                Console.WriteLine($"Dynamic StreamBlock {crc} was not found in global.map!");
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

            Console.WriteLine();

            return true;
        }
    }
}
