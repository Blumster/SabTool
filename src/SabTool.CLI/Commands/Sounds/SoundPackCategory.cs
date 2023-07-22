using System.Diagnostics;

using SabTool.CLI.Base;
using SabTool.Depot;

namespace SabTool.CLI.Commands.Sounds;
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

            string outputDirectory = arguments.ElementAt(1);
            _ = Directory.CreateDirectory(outputDirectory);

            string ww2OggPath = arguments.ElementAt(2);
            string ww2OggExePath = Path.Combine(ww2OggPath, "ww2ogg.exe");
            if (!Directory.Exists(ww2OggPath) || !File.Exists(ww2OggExePath))
            {
                Console.WriteLine("Invalid ww2ogg path!");
                return false;
            }

            bool conv = false;
            if (arguments.Count() == 4)
                conv = arguments.ElementAt(3) == "convert";

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            _ = ResourceDepot.Instance.Load(Resource.Sounds);

            foreach (Data.Sounds.SoundPack soundPack in ResourceDepot.Instance.GetSoundPacks())
            {
                string soundPackOutputDir = Path.Combine(outputDirectory, soundPack.FilePath);
                _ = Directory.CreateDirectory(soundPackOutputDir);
                _ = Directory.CreateDirectory(Path.Combine(soundPackOutputDir, "banks"));
                _ = Directory.CreateDirectory(Path.Combine(soundPackOutputDir, "oggs"));

                foreach (Data.Sounds.SoundBank? bank in soundPack.SoundBanks)
                {
                    string bankName = bank.Id.GetStringOrHexString();
                    string bankFilePath = Path.Combine(soundPackOutputDir, $"banks\\{bankName}.bnk");

                    using FileStream fs = new(bankFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                    fs.Write(bank.Data, 0, bank.Data.Length);
                }

                foreach (Data.Sounds.SoundStream? stream in soundPack.SoundStreams)
                {
                    string streamName = stream.Id.GetStringOrHexString();
                    if (streamName.Contains('\\'))
                    {
                        Console.WriteLine($"Possible bad Crc for stream: {stream.Id}");

                        streamName = stream.Id.GetHexString();
                    }

                    string streamFilePath = Path.Combine(soundPackOutputDir, $"oggs\\{streamName}.wem");

                    using FileStream fs = new(streamFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                    fs.Write(stream.Data, 0, stream.Data.Length);
                    fs.Close();

                    if (!conv)
                        continue;

                    ProcessStartInfo info = new()
                    {
                        WorkingDirectory = ww2OggPath,
                        FileName = ww2OggExePath,
                        Arguments = $"{streamFilePath} -o {Path.ChangeExtension(streamFilePath, "ogg")} --full-setup"
                    };

                    _ = Process.Start(info);
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
