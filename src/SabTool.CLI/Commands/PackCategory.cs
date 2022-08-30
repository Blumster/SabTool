using System.Globalization;
using System.Text.RegularExpressions;

using ShellProgressBar;

namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Megapacks;
using SabTool.Serializers.Packs;
using SabTool.Utils;
using System.Linq;

public class PackCategory : BaseCategory
{
    public override string Key { get; } = "pack";
    public override string Shortcut { get; } = "p";
    public override string Usage { get; } = "<sub command>";

    public class UnpackCommand : BaseCommand
    {
        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
        public override string Usage { get; } = "<game base path> <packs file path> <output directory>";

        private static readonly Regex ChunkRegex = new(@"[fF]rance\\(?:(?:\d\d\\\d+)|(?:\d+))\.[pP]ack$", RegexOptions.Compiled);

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 3)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Maps);

            var packsBaseDir = arguments.ElementAt(1);
            var outputDir = arguments.ElementAt(2);
            if (outputDir == null)
            {
                Console.WriteLine("ERROR: No output directory is given!");
                return false;
            }

            Directory.CreateDirectory(outputDir);

            var consoleError = Console.Error;

            using var logWriter = new StreamWriter(new FileStream(Path.Combine(outputDir, "packlog.txt"), FileMode.Create, FileAccess.Write, FileShare.Read));

            Console.SetError(logWriter);

            var files = Directory.GetFiles(packsBaseDir, "*.*", SearchOption.AllDirectories);

            var options = new ProgressBarOptions
            {
                EnableTaskBarProgress = true,
                ForegroundColor = ConsoleColor.White,
                CollapseWhenFinished = true,
                ProgressBarOnBottom = true
            };

            var childOptions = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.White,
                CollapseWhenFinished = true,
                ProgressBarOnBottom = true
            };

            using var progressBar = new ProgressBar(files.Length, "Unpacking packs...", options);

            foreach (var pack in files)
            {
                ProcessPack(pack, Path.Combine(outputDir, Path.GetDirectoryName(pack)![(packsBaseDir.Length + 1)..]), progressBar, childOptions);

                progressBar.Tick();
            }

            Console.SetError(consoleError);

            return true;
        }

        private static void ProcessPack(string filePath, string outputDir, ProgressBar progressBar, ProgressBarOptions childOptions)
        {
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
                return;

            outputDir = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(filePath));
            outputDir = outputDir.Replace(" ", "");

            Directory.CreateDirectory(outputDir);

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            using var child = progressBar.Spawn(streamBlock.EntryCounts.Select(c => (int)c).Sum(), filePath, childOptions);

            PackSerializer.DeserializeRaw(fs, streamBlock);

            StreamBlockSerializer.ReadPayloads(streamBlock, fs);
            StreamBlockSerializer.Export(streamBlock, outputDir, child.AsProgress<string>());

            streamBlock.FreePayloads();
        }
    }
}
