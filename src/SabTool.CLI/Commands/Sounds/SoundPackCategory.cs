using System.Diagnostics;

namespace SabTool.CLI.Commands.Sounds;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Sounds;

public sealed class SoundPackCategory : BaseCategory
{
    public override string Key { get; } = "sound-pack";
    public override string Shortcut { get; } = "s";
    public override string Usage { get; } = "";

    public sealed class DumpCommand : BaseCommand
    {
        public override string Key { get; } = "dump";
        public override string Shortcut { get; } = "d";
        public override string Usage { get; } = "<game base path> <output dir path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Sounds);

            var outputDirectory = arguments.ElementAt(1);
            Directory.CreateDirectory(outputDirectory);

            using var fs = new FileStream(Path.Combine(outputDirectory, "wwiseidtable-dump.json"), FileMode.Create, FileAccess.Write, FileShare.None);

            WWiseIDTableSerializer.SerializeJSON(ResourceDepot.Instance.GetWWiseIDTable(), fs);

            return true;
        }
    }

    public sealed class UnpackCommand : BaseCommand
    {
        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
        public override string Usage { get; } = "<game base path> <output dir path> <ww2ogg path> [convert]";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 3)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            var outputDirectory = arguments.ElementAt(1);
            Directory.CreateDirectory(outputDirectory);

            var ww2OggPath = arguments.ElementAt(2);
            var ww2OggExePath = Path.Combine(ww2OggPath, "ww2ogg.exe");
            if (!Directory.Exists(ww2OggPath) || !File.Exists(ww2OggExePath))
            {
                Console.WriteLine("Invalid ww2ogg path!");
                return false;
            }

            var conv = false;
            if (arguments.Count() == 4)
                conv = arguments.ElementAt(3) == "convert";

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Sounds);

            var ww2offProcessInfos = new List<ProcessStartInfo>();

            foreach (var soundPack in ResourceDepot.Instance.GetSoundPacks())
            {
                var soundPackOutputDir = Path.Combine(outputDirectory, soundPack.FilePath);
                Directory.CreateDirectory(soundPackOutputDir);
                Directory.CreateDirectory(Path.Combine(soundPackOutputDir, "banks"));
                Directory.CreateDirectory(Path.Combine(soundPackOutputDir, "oggs"));

                foreach (var bank in soundPack.SoundBanks)
                {
                    var bankFilePath = Path.Combine(soundPackOutputDir, $"banks\\{bank.Id}.bnk");

                    using var fs = new FileStream(bankFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                    fs.Write(bank.Data, 0, bank.Data.Length);
                }

                foreach (var stream in soundPack.SoundStreams)
                {
                    var streamFilePath = Path.Combine(soundPackOutputDir, $"oggs\\{stream.Id}.wem");

                    using var fs = new FileStream(streamFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                    fs.Write(stream.Data, 0, stream.Data.Length);
                    fs.Close();

                    if (!conv)
                        continue;

                    ww2offProcessInfos.Add(new ProcessStartInfo
                    {
                        WorkingDirectory = ww2OggPath,
                        FileName = ww2OggExePath,
                        Arguments = $"{streamFilePath} -o {Path.ChangeExtension(streamFilePath, "ogg")} --full-setup",
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    });
                }
            }

            if (ww2offProcessInfos.Count > 0)
            {
                var ww2oggProcesses = new List<Process>();

                Console.WriteLine($"Starting {ww2offProcessInfos.Count} processes...");

                var lastEcho = 0L;
                var sw = Stopwatch.StartNew();
                var i = 0;

                foreach (var info in ww2offProcessInfos)
                {
                    ++i;

                    var p = Process.Start(info);

                    if (p is not null)
                        ww2oggProcesses.Add(p);

                    if (lastEcho + 1000 < sw.ElapsedMilliseconds)
                    {
                        Console.WriteLine($"Starting: {i}/{ww2offProcessInfos.Count} processes...");

                        lastEcho = sw.ElapsedMilliseconds;
                    }
                }

                Console.WriteLine($"Processes started in {sw.ElapsedMilliseconds} ms!");

                var totalProcesses = ww2oggProcesses.Count;

                lastEcho = 0;

                while (ww2oggProcesses.Count > 0)
                {
                    var toDelete = new List<Process>();

                    foreach (var process in ww2oggProcesses)
                    {
                        if (process.HasExited)
                        {
                            toDelete.Add(process);
                        }
                    }

                    foreach (var process in toDelete)
                        ww2oggProcesses.Remove(process);

                    if (lastEcho + 1000 < sw.ElapsedMilliseconds)
                    {
                        Console.WriteLine($"{ww2oggProcesses.Count}/{totalProcesses}");

                        lastEcho = sw.ElapsedMilliseconds;
                    }
                }
            }

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
