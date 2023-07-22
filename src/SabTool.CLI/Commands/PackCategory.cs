using System.Globalization;
using System.Text.RegularExpressions;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Megapacks;
using SabTool.Serializers.Packs;
using SabTool.Utils;

using ShellProgressBar;

namespace SabTool.CLI.Commands;
public sealed class PackCategory : BaseCategory
{
    public override string Key { get; } = "pack";
    public override string Shortcut { get; } = "p";
    public override string Usage { get; } = "<sub command>";

    public sealed class UnpackCommand : BaseCommand
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
            _ = ResourceDepot.Instance.Load(Resource.Maps);

            string packsBaseDir = arguments.ElementAt(1);
            string outputDir = arguments.ElementAt(2);
            if (outputDir == null)
            {
                Console.WriteLine("ERROR: No output directory is given!");
                return false;
            }

            _ = Directory.CreateDirectory(outputDir);

            TextWriter consoleError = Console.Error;

            using StreamWriter logWriter = new(new FileStream(Path.Combine(outputDir, "packlog.txt"), FileMode.Create, FileAccess.Write, FileShare.Read));

            Console.SetError(logWriter);

            string[] files = Directory.GetFiles(packsBaseDir, "*.*", SearchOption.AllDirectories);

            ProgressBarOptions options = new()
            {
                EnableTaskBarProgress = true,
                ForegroundColor = ConsoleColor.White,
                CollapseWhenFinished = true,
                ProgressBarOnBottom = true
            };

            ProgressBarOptions childOptions = new()
            {
                ForegroundColor = ConsoleColor.White,
                CollapseWhenFinished = true,
                ProgressBarOnBottom = true
            };

            using ProgressBar progressBar = new(files.Length, "Unpacking packs...", options);

            foreach (string pack in files)
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

            string fileName = Path.GetFileNameWithoutExtension(filePath);

            crc = ChunkRegex.IsMatch(filePath.ToLowerInvariant())
                ? new Crc(uint.Parse(fileName))
                : fileName.StartsWith("0x") ? new Crc(uint.Parse(fileName[2..], NumberStyles.HexNumber)) : new Crc(Hash.StringToHash(fileName));

            Data.Packs.StreamBlock? streamBlock = ResourceDepot.Instance.GetStreamBlock(crc);
            if (streamBlock == null)
            {
                Console.WriteLine($"Could not find StreamBlock for {crc} ({filePath})");
                return;
            }

            outputDir = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(filePath));
            outputDir = outputDir.Replace(" ", "");

            _ = Directory.CreateDirectory(outputDir);

            using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            using ChildProgressBar child = progressBar.Spawn(streamBlock.EntryCounts.Select(c => (int)c).Sum(), filePath, childOptions);

            PackSerializer.DeserializeRaw(fs, streamBlock);

            StreamBlockSerializer.ReadPayloads(streamBlock, fs);
            StreamBlockSerializer.Export(streamBlock, outputDir, child.AsProgress<string>());

            streamBlock.FreePayloads();
        }
    }
}
