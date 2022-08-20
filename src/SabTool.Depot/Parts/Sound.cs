namespace SabTool.Depot
{
    using Data.Sounds;
    using Serializers.Sounds;

    public partial class ResourceDepot
    {
        private List<SoundPack> SoundPacks { get; set; } = new();
        private List<SoundPack> DLCSoundPacks { get; set; } = new();

        private bool LoadSounds()
        {
            Console.WriteLine("Loading Sounds...");

            LoadSoundPacks();
            LoadDLCSoundPacks();

            Console.WriteLine("Sounds loaded!");

            LoadedResources |= Resource.Sounds;

            return true;
        }

        private void LoadSoundPacks()
        {
            Console.WriteLine("  Loading Sound packs...");

            var inputFolder = GetGamePath(@"Sound");

            foreach (var filePath in Directory.EnumerateFiles(inputFolder, "*.pck"))
            {
                var relativeFilePath = Path.GetRelativePath(inputFolder, filePath);

                Console.WriteLine($"    Loading {relativeFilePath}...");

                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var soundPack = SoundPackSerializer.DeserializeRaw(fs);

                soundPack.FilePath = relativeFilePath;

                SoundPacks.Add(soundPack);
            }

            Console.WriteLine("  Sound packs loaded!");
        }

        private void LoadDLCSoundPacks()
        {
            Console.WriteLine("  Loading DLC Sound packs...");

            var inputFolder = GetGamePath(@"DLC\01\sound");

            foreach (var filePath in Directory.EnumerateFiles(inputFolder, "*.pck"))
            {
                var relativeFilePath = Path.GetRelativePath(inputFolder, filePath);

                Console.WriteLine($"    Loading {relativeFilePath}...");

                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var soundPack = SoundPackSerializer.DeserializeRaw(fs);

                soundPack.FilePath = relativeFilePath;

                DLCSoundPacks.Add(soundPack);
            }

            Console.WriteLine("  DLC Sound packs loaded!");
        }

        public IEnumerable<SoundPack> GetSoundPacks()
        {
            return SoundPacks;
        }

        public IEnumerable<SoundPack> GetDLCSoundPacks()
        {
            return DLCSoundPacks;
        }
    }
}
