using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands
{
    using Base;
    using Data;
    using Data.GameTemplatesOld;
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

                var basePath = arguments.ElementAt(0);
                var outputDir = arguments.ElementAt(1);

                if (!Directory.Exists(basePath))
                {
                    Console.WriteLine("ERROR: The game base path does not exist!");
                    return false;
                }

                Directory.CreateDirectory(outputDir);

                var looseFilePath = Path.Combine(basePath, @"France\loosefiles_BinPC.pack");
                if (!File.Exists(looseFilePath))
                {
                    Console.WriteLine($"ERROR: Loosefile could not be found under \"{looseFilePath}\"!");
                    return false;
                }

                var looseFile = new LooseFile();

                using (var fs = new FileStream(looseFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    looseFile.Read(fs);

                var gameTemplatesEntry = looseFile.Files.Where(f => f.Name == "GameTemplates.wsd").FirstOrDefault();
                if (gameTemplatesEntry == null)
                {
                    Console.WriteLine("ERROR: Could not read GameTemplates.wsd from loosefiles!");
                    return false;
                }

                using var ms = new MemoryStream(gameTemplatesEntry.Data);

                var blueprints = BlueprintSerializer.DeserializeRaw(ms);

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
            public override string Usage { get; } = "<game base path | input file path> [output file path]";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Any())
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                var basePathOrFilePath = arguments.ElementAt(0);
                var outputFilePath = arguments.Count() > 1 ? arguments.ElementAt(1) : null;

                byte[] data = null;

                var attrs = File.GetAttributes(basePathOrFilePath);
                if ((attrs & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var looseFilePath = Path.Combine(basePathOrFilePath, @"France\loosefiles_BinPC.pack");
                    if (!File.Exists(looseFilePath))
                    {
                        Console.WriteLine($"ERROR: Loosefile could not be found under \"{looseFilePath}\"!");
                        return false;
                    }

                    var looseFile = new LooseFile();

                    using (var fs = new FileStream(looseFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        looseFile.Read(fs);

                    var gameTemplatesEntry = looseFile.Files.Where(f => f.Name == "GameTemplates.wsd").FirstOrDefault();
                    if (gameTemplatesEntry == null)
                    {
                        Console.WriteLine("ERROR: Could not read GameTemplates.wsd from loosefiles!");
                        return false;
                    }

                    data = gameTemplatesEntry.Data;
                }
                else
                {
                    data = File.ReadAllBytes(basePathOrFilePath);
                }

                StreamWriter writer = null;

                if (outputFilePath != null)
                {
                    writer = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None));
                }

                using var ms = new MemoryStream(data);

                var blueprints = BlueprintSerializer.DeserializeRaw(ms);

                foreach (var blueprint in blueprints)
                {
                    writer.WriteLine(blueprint.ToString());
                }

                /*var gameTemplates = new GameTemplatesDump(writer);

                using (var ms = new MemoryStream(data))
                    gameTemplates.Read(ms);*/

                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                }

                return true;
            }
        }
    }
}
