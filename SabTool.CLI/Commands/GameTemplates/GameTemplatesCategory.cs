using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands.GameTemplates
{
    using Base;
    using Containers.GameTemplates;
    using Containers.LooseFiles;

    public class GameTemplatesCategory : BaseCategory
    {
        public override string Key => "base";

        public override string Usage => "<sub command>";

        public class UnpackCommand : BaseCommand
        {
            public override string Key { get; } = "unpack";
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

                var gameTemplates = new GameTemplates();

                using (var ms = new MemoryStream(gameTemplatesEntry.Data))
                    gameTemplates.Read(ms);

                return true;
            }
        }

        public class PackCommand : BaseCommand
        {
            public override string Key { get; } = "pack";
            public override string Usage { get; } = "<game base path> <input directory path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                return true;
            }
        }

        public class DumpCommand : BaseCommand
        {
            public override string Key { get; } = "dump";
            public override string Usage { get; } = "<game base path> <output file path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 1)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                var basePath = arguments.ElementAt(0);
                var outputDir = arguments.Count() > 1 ? arguments.ElementAt(1) : null;

                if (!Directory.Exists(basePath))
                {
                    Console.WriteLine("ERROR: The game base path does not exist!");
                    return false;
                }

                if (outputDir != null)
                {
                    Directory.CreateDirectory(outputDir);
                }

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

                StreamWriter writer = null;
                if (outputDir != null)
                {
                    writer = new StreamWriter(new FileStream(Path.Combine(outputDir, "game-templates.txt"), FileMode.Create, FileAccess.Write, FileShare.None));
                }

                var gameTemplates = new GameTemplatesDump(writer);

                using (var ms = new MemoryStream(gameTemplatesEntry.Data))
                    gameTemplates.Read(ms);

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
