namespace SabTool.CLI.Commands.Cinematics;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Cinematics;

public class ConversationsCategory : BaseCategory
{
    public override string Key { get; } = "conversations";
    public override string Shortcut { get; } = "co";
    public override string Usage { get; } = "";

    public class UnpackCommand : BaseCommand
    {
        private const string ConversationsRootPath = @"Cinematics\Conversations";
        private const string ConversationsRootPathDLC = @"DLC\01\Cinematics\Conversations";

        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
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

            // Main
            var outputFilePath = Path.Combine(outputDirectory, ConversationsRootPath, "Conversations.json");

            var outputFileDirectory = Path.GetDirectoryName(outputFilePath)!;
            Directory.CreateDirectory(outputFileDirectory);

            using var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

            ConversationsSerializer.SerializeJSON(ResourceDepot.Instance.GetConversations().ToList(), fs);

            // DLC
            outputFilePath = Path.Combine(outputDirectory, ConversationsRootPathDLC, "Conversations.json");

            outputFileDirectory = Path.GetDirectoryName(outputFilePath)!;
            Directory.CreateDirectory(outputFileDirectory);

            using var fsDLC = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

            ConversationsSerializer.SerializeJSON(ResourceDepot.Instance.GetDLCConversations().ToList(), fsDLC);

            return true;
        }
    }

    public class PackCommand : BaseCommand
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
