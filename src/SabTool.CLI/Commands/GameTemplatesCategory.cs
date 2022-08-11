using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands
{
    using Base;
    using Data;
    using Depot;
    using Serializers;

    public class GameTemplatesCategory : BaseCategory
    {
        public override string Key => "game-templates";

        public override string Shortcut => "g";

        public override string Usage => "<sub command>";

        public class UnpackCommand : BaseCommand
        {
            public override string Key { get; } = "unpack";
            public override string Shortcut { get; } = "u";
            public override string Usage { get; } = "<game base path> <output directory>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 2)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
                ResourceDepot.Instance.Load(Resource.All);

                var gameTemplatesStream = ResourceDepot.Instance.GetLooseFile("GameTemplates.wsd");
                if (gameTemplatesStream == null)
                {
                    Console.WriteLine("ERROR: Could not read GameTemplates.wsd from LooseFiles!");
                    return false;
                }

                var outputDir = arguments.ElementAt(1);

                Directory.CreateDirectory(outputDir);

                var blueprints = BlueprintSerializer.DeserializeRaw(gameTemplatesStream);

                using var outFileStream = new FileStream(Path.Combine(outputDir, "blueprints.json"), FileMode.Create, FileAccess.Write, FileShare.None);

                BlueprintSerializer.SerializeJSON(blueprints, outFileStream);

                return true;
            }
        }

        public class PackCommand : BaseCommand
        {
            public override string Key { get; } = "pack";
            public override string Shortcut { get; } = "p";
            public override string Usage { get; } = "<game base path> <input directory path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                return true;
            }
        }

        public class DumpCommand : BaseCommand
        {
            public override string Key { get; } = "dump";
            public override string Shortcut { get; } = "d";
            public override string Usage { get; } = "<game base path> [output file path]";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (!arguments.Any())
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
                ResourceDepot.Instance.Load(Resource.LooseFiles);

                var gameTemplatesStream = ResourceDepot.Instance.GetLooseFile("GameTemplates.wsd");
                if (gameTemplatesStream == null)
                {
                    Console.WriteLine("ERROR: Could not read GameTemplates.wsd from LooseFiles!");
                    return false;
                }

                var outputFilePath = arguments.Count() > 1 ? arguments.ElementAt(1) : null;

                var writer = Console.Out;
                var outputToFile = false;

                if (outputFilePath != null)
                {
                    writer = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None));
                    outputToFile = true;
                }

                var blueprints = BlueprintSerializer.DeserializeRaw(gameTemplatesStream);

                foreach (var blueprint in blueprints)
                {
                    writer.WriteLine(blueprint.ToString());
                }

                if (writer != null && outputToFile)
                {
                    writer.Flush();
                    writer.Close();
                }

                return true;
            }
        }
    }
}
