using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands.Megapack
{
    using Base;
    using Containers.LooseFiles;
    using Containers.Megapacks.France;

    public class MegapackFranceCategory : BaseCategory
    {
        public override string Key => "france";

        public override string Usage => "<sub command>";

        public class UnpackCommand : BaseCommand
        {
            public static readonly List<string> Megapacks = new List<string>()
            {
                @"France\Mega.megapack",
                @"France\Mega0.megapack",
                @"France\Mega1.megapack",
                @"France\Mega2.megapack",
                @"France\Mega3.megapack",
                @"France\Mega4.megapack",
                @"France\patchmega.megapack",
                @"France\patchmega0.megapack",
                @"France\patchmega1.megapack",
                @"France\patchmega2.megapack",
                @"France\patchmega3.megapack"
            };

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

                var franceMapEntry = looseFile.Files.Where(f => f.Name == "France.map").FirstOrDefault();
                if (franceMapEntry == null)
                {
                    Console.WriteLine("ERROR: Could not read global.map from loosefiles!");
                    return false;
                }

                var franceMap = new Map();

                using (var ms = new MemoryStream(franceMapEntry.Data))
                    franceMap.Read(ms);

                var megapacks = new List<Megapack>();

                foreach (var filePath in Megapacks)
                {
                    var fullPath = Path.Combine(basePath, filePath);

                    if (!File.Exists(fullPath))
                        continue;

                    var megapack = new Megapack(franceMap);

                    using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        Console.WriteLine($"Reading: {Path.GetFileName(fullPath)}");
                        megapack.Read(fs);
                    }

                    megapacks.Add(megapack);
                }

                foreach (var megapack in megapacks)
                {
                    // TODO: extract when we have the proper file reading methods
                }

                Console.WriteLine("Successfully unpacked the global megapacks!");
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
    }
}
