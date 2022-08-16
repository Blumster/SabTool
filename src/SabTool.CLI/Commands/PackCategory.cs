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

            var dynPacks = Directory.GetFiles(packsBaseDir, "*.dynpack", SearchOption.AllDirectories);
            if (dynPacks.Length == 0)
            {
                Console.WriteLine("ERROR: No files found in the input directory!");
                return false;
            }

            foreach (var dynPack in dynPacks)
            {
                ProcessDynpack(dynPack, Path.Combine(outputDir, Path.GetDirectoryName(dynPack)![(packsBaseDir.Length + 1)..]));
            }

            /*var palettePacks = Directory.GetFiles(packsBaseDir, "*.palettepack", SearchOption.AllDirectories);
            if (palettePacks.Length == 0)
            {
                Console.WriteLine("ERROR: No files found in the input directory!");
                return false;
            }

            foreach (var palettePack in palettePacks)
            {
                ProcessPalettepack(palettePack, Path.Combine(outputDir, Path.GetDirectoryName(palettePack)![(packsBaseDir.Length + 1)..]));
            }*/

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

                PackSerializer.DeserializeRaw(fs, streamBlock);

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

            var streamBlock = ResourceDepot.Instance.GlobalMap!.GetDynamicBlock(crc);
            if (streamBlock == null)
            {
                Console.WriteLine($"Dynamic StreamBlock {crc} was not found in global.map!");
                return false;
            }

            outputDir = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(filePath));
            outputDir = outputDir.Replace(" ", "");

            Directory.CreateDirectory(outputDir);

            /*using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using var reader = new BinaryReader(fs);

                PackSerializer.DeserializeRaw(fs, streamBlock);

                StreamBlockSerializer.Export(streamBlock, outputDir);
            }*/

            Console.WriteLine();

            return true;
        }
    }
}
