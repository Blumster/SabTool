namespace SabTool.CLI.Commands.Cinematics;

using SabTool.CLI.Base;
using SabTool.Data.Misc;
using SabTool.Depot;
using SabTool.Serializers.Misc;
using SabTool.Utils;

public sealed class MovieCategory : BaseCategory
{
    public override string Key { get; } = "movie";
    public override string Shortcut { get; } = "m";
    public override string Usage { get; } = "";

    static string[] BinkMovies = new string[]
    {
        "115_CinA_Cheat",
        "121_CinA_Question",
        "127_CinA_Intro",
        "201_CinA_3Months",
        "217_CinB_Box",
        "221_CinA_BlimpCrash",
        "333_CinA_RaceBoom",
        "407_cinb_flashback",
        "408_CinB_DoppBoom",
        "Ambient_FP",
        "Sab_Placeholder",
        "saboteur",
        "saboteur_de"
    };

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

            var outputDirectory = arguments.ElementAt(1);
            Directory.CreateDirectory(outputDirectory);

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Cinematics);

            // TODO: preload data about binks in depot

            foreach (var bink in BinkMovies)
            {
                var hash = Hash.StringToHash(bink);

                if (!File.Exists(ResourceDepot.Instance.GetGamePath($"Bink\\{bink}.bik")))
                {
                    Console.WriteLine($"Missing bink movie: {bink}!");
                    return false;
                }

                var povPath = ResourceDepot.Instance.GetGamePath($"{hash:x}.pov");
                if (!File.Exists(povPath))
                {
                    Console.WriteLine($"Missing pov file {hash:x} for movie: {bink}!");
                    return false;
                }

                using var fs = new FileStream(povPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var povFile = BinkPOVSerializer.DeserializeRaw(fs);

                Console.WriteLine(povFile);
            }

            return true;
        }
    }
}
