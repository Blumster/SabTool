namespace SabTool.Depot;

using SabTool.Data.Packs;
using SabTool.Serializers.Megapacks;
using SabTool.Utils;

public enum MegapackType
{
    Global,
    France,
    GlobalDLC,
    FranceDLC
}

public sealed partial class ResourceDepot
{
    private static readonly List<string> PossibleMegapackFiles = new()
    {
        @"Global\Dynamic0.megapack",
        @"Global\palettes0.megapack",
        @"Global\patchdynamic0.megapack",
        @"Global\patchpalettes0.megapack",
        @"France\BelleStart0.kiloPack",
        @"France\Mega.megapack",
        @"France\Mega0.megapack",
        @"France\Mega1.megapack",
        @"France\Mega2.megapack",
        @"France\Mega3.megapack",
        @"France\Mega4.megapack",
        @"France\Start_German0.kiloPack",
        @"France\Start0.kiloPack",
        @"France\patchmega.megapack",
        @"France\patchmega0.megapack",
        @"France\patchmega1.megapack",
        @"France\patchmega2.megapack",
        @"France\patchmega3.megapack",
        @"DLC\01\dlc01mega0.megapack",
        @"DLC\01\Dynamic0.megapack",
    };

    private Dictionary<string, MegapackEntry> Megapacks { get; } = new();

    private bool LoadMegapacks()
    {
        Console.WriteLine("Loading Megapacks...");

        foreach (var megapack in PossibleMegapackFiles)
        {
            var filePath = GetGamePath(megapack);

            if (!File.Exists(filePath))
                continue;

            var entry = new MegapackEntry(filePath);

            if (!LoadMegapack(entry))
                continue;

            Megapacks.Add(megapack, entry);
        }

        LoadEditNodes();

        LoadedResources |= Resource.Megapacks;

        Console.WriteLine($"Loaded {Megapacks.Count} Megapacks!");

        return true;
    }

    private static bool LoadMegapack(MegapackEntry entry)
    {
        try
        {
            entry.Megapack = MegapackSerializer.DeserializeRaw(entry.Stream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while loading Megapack {entry.FilePath}! Exception: {ex}");

            return false;
        }

        Console.WriteLine($"Loaded Megapack: {entry.FilePath}");
        
        return true;
    }

    private static void LoadEditNodes()
    {
        Console.WriteLine($"Loaded EditNodes: ");
    }

    public MemoryStream? GetPackStream(Crc key)
    {
        foreach (var megapackPairs in Megapacks)
        {
            var megapackEntry = megapackPairs.Value;
            var megapack = megapackEntry.Megapack;
            if (megapack == null)
                continue;

            if (megapack.FileEntries.TryGetValue(key, out var fileEntry))
            {
                var memoryStream = new MemoryStream(fileEntry.Size);
                memoryStream.SetLength(fileEntry.Size);

                lock (megapackEntry.Stream)
                {
                    megapackEntry.Stream.Position = fileEntry.Offset;

                    if (megapackEntry.Stream.Read(memoryStream.GetBuffer(), 0, fileEntry.Size) != fileEntry.Size)
                        return null;
                }

                return memoryStream;
            }
        }

        return null;
    }

    public IEnumerable<string> GetMegapacks()
    {
        foreach (var megapack in Megapacks)
            yield return megapack.Key;
    }

    public IEnumerable<FileEntry> GetFileEntries(string key)
    {
        if (Megapacks.TryGetValue(key, out var entry))
        {
            if (entry.Megapack == null)
                yield break;

            foreach (var entryPair in entry.Megapack.FileEntries)
                yield return entryPair.Value;
        }
    }

    public IEnumerable<(Crc, Crc)> GetBlockPathToNameCrcs(string key)
    {
        if (Megapacks.TryGetValue(key, out var entry))
        {
            if (entry.Megapack == null)
                yield break;

            foreach (var entryPair in entry.Megapack.BlockPathToNameCrcs)
                yield return entryPair.ToValueTuple();
        }
    }

    private class MegapackEntry
    {
        public string FilePath { get; set; }
        public Megapack? Megapack { get; set; }
        public FileStream Stream { get; set; }

        public MegapackEntry(string filePath)
        {
            FilePath = filePath;
            Stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
