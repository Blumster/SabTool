using System.Diagnostics;

namespace SabTool.CLI.Commands.Sounds;

using SabTool.CLI.Base;
using SabTool.Depot;

public sealed class SoundPackCategory : BaseCategory
{
    public override string Key { get; } = "sound-pack";
    public override string Shortcut { get; } = "s";
    public override string Usage { get; } = "";

    public sealed class UnpackCommand : BaseCommand
    {
        private const string DialogRootPath = @"Cinematics\Dialog";

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

            foreach (var soundPack in ResourceDepot.Instance.GetSoundPacks())
            {
                var soundPackOutputDir = Path.Combine(outputDirectory, soundPack.FilePath);
                Directory.CreateDirectory(soundPackOutputDir);

                foreach (var bank in soundPack.SoundBanks)
                {
                    var bankName = bank.Id.GetStringOrHexString();
                    var bankFilePath = Path.Combine(soundPackOutputDir, $"{bankName}.bnk");

                    using var fs = new FileStream(bankFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                    fs.Write(bank.Data, 0, bank.Data.Length);
                }

                foreach (var stream in soundPack.SoundStreams)
                {
                    var streamName = stream.Id.GetStringOrHexString();
                    if (streamName.Contains('\\'))
                    {
                        Console.WriteLine($"Possible bad Crc for stream: {stream.Id}");

                        streamName = stream.Id.GetHexString();
                    }

                    var streamFilePath = Path.Combine(soundPackOutputDir, $"{streamName}.wwogg");

                    using var fs = new FileStream(streamFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                    fs.Write(stream.Data, 0, stream.Data.Length);
                    fs.Close();

                    if (!conv)
                        continue;

                    var info = new ProcessStartInfo
                    {
                        WorkingDirectory = ww2OggPath,
                        FileName = ww2OggExePath,
                        Arguments = $"{streamFilePath} -o {Path.ChangeExtension(streamFilePath, "ogg")} --full-setup"
                    };

                    Process.Start(info);
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
