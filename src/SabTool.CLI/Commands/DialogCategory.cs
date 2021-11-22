using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands
{
    using Base;
    using Depot;
    using Serializers;

    public class DialogCategory : BaseCategory
    {
        public override string Key { get; } = "dialog";
        public override string Shortcut { get; } = "d";
        public override string Usage { get; } = "";

        public class UnpackCommand : BaseCommand
        {
            public override string Key { get; } = "unpack";
            public override string Shortcut { get; } = "u";
            public override string Usage { get; } = "<game base path> <output dir path> [language]";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 2)
                {
                    return false;
                }

                var outputDirectory = arguments.ElementAt(1);
                Directory.CreateDirectory(outputDirectory);

                string language = null;
                if (arguments.Count() == 3)
                    language = arguments.ElementAt(2);

                ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
                ResourceDepot.Instance.Load(Resource.Dialogs);

                foreach (var entry in ResourceDepot.Instance.GetDialogs())
                {
                    if (!string.IsNullOrEmpty(language) && language.ToLowerInvariant() != entry.Key.ToLowerInvariant())
                        continue;

                    var outputFilePath = Path.Combine(outputDirectory, $"{entry.Key}.dialogs.json");

                    using var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                    DialogSerializer.SerializeJSON(entry.Value, fs);
                }

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
}
