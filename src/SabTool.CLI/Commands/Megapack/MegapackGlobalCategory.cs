using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands.Megapack
{
    using Base;
    using Data;
    using Data.Packs;

    public class MegapackGlobalCategory : BaseCategory
    {
        public override string Key => "global";

        public override string Shortcut => "g";

        public override string Usage => "<sub command>";

        public class UnpackCommand : BaseCommand
        {
            public static readonly List<string> Megafiles = new()
            {
                @"Global\Dynamic0.megapack",
                @"Global\palettes0.megapack",
                @"Global\patchdynamic0.megapack",
                @"Global\patchpalettes0.megapack"
            };

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

                foreach (var filePath in Megafiles)
                {
                    var fullPath = Path.Combine(basePath, filePath);

                    if (!filePath.Contains("patch") && !File.Exists(fullPath))
                    {
                        Console.WriteLine($"ERROR: Missing non-optional megapack: {filePath}!");
                        return false;
                    }
                }

                var looseFile = new LooseFile();

                using (var fs = new FileStream(looseFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    looseFile.Read(fs);

                var globalMapEntry = looseFile.Files.Where(f => f.Name == "global.map").FirstOrDefault();
                if (globalMapEntry == null)
                {
                    Console.WriteLine("ERROR: Could not read global.map from loosefiles!");
                    return false;
                }

                var globalMap = new GlobalMap();

                using (var ms = new MemoryStream(globalMapEntry.Data))
                {
                    using var reader = new BinaryReader(ms);

                    globalMap.Read(reader, globalMapEntry.Name);
                }

                foreach (var filePath in Megafiles)
                {
                    var fullPath = Path.Combine(basePath, filePath);
                    var megaOutDir = Path.Combine(outputDir, Path.GetFileName(fullPath));

                    if (!File.Exists(fullPath))
                        continue;

                    using var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var reader = new BinaryReader(fs);

                    var megafile = new GlobalMegaFile(globalMap);

                    megafile.Read(reader);
                    megafile.Export(reader, megaOutDir);
                }

                Console.WriteLine("Successfully unpacked the global megapacks!");
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
    }
}
