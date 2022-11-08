namespace SabTool.CLI.Commands.Cinematics;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Cinematics;

public sealed class DialogCategory : BaseCategory
{
    public override string Key { get; } = "dialog";
    public override string Shortcut { get; } = "d";
    public override string Usage { get; } = "";

    public sealed class UnpackCommand : BaseCommand
    {
        private const string DialogRootPath = @"Cinematics\Dialog";

        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
        public override string Usage { get; } = "<game base path> <output dir path> [language]";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            var outputDirectory = arguments.ElementAt(1);
            Directory.CreateDirectory(outputDirectory);

            string? language = null;
            if (arguments.Count() == 3)
                language = arguments.ElementAt(2);

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Cinematics);

            foreach (var entry in ResourceDepot.Instance.GetDialogs())
            {
                if (!string.IsNullOrEmpty(language) && language.ToLowerInvariant() != entry.Key.ToLowerInvariant())
                    continue;

                var outputFilePath = Path.Combine(outputDirectory, DialogRootPath, $@"{entry.Key}\dialogs.json");

                var outputFileDirectory = Path.GetDirectoryName(outputFilePath);
                if (outputFileDirectory == null)
                {
                    Console.WriteLine("ERROR: Output directory is invalid!");
                    return false;
                }

                Directory.CreateDirectory(outputFileDirectory);

                using var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                DialogSerializer.SerializeJSON(entry.Value, fs);
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
