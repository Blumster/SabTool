using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SabTool.CLI.Commands
{
    using Base;
    using Depot;

    public class MegapackCategory : BaseCategory
    {
        public override string Key => "megapack";

        public override string Shortcut => "m";

        public override string Usage => "<sub command>";

        public class ListCommand : BaseCommand
        {
            public override string Key { get; } = "list";
            public override string Shortcut { get; } = "l";
            public override string Usage { get; } = "<game base path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 1)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
                ResourceDepot.Instance.Load(Resource.Megapacks);

                var writer = Console.Out;

                if (arguments.Count() == 2)
                {
                    writer = new StreamWriter(arguments.ElementAt(1), false, Encoding.UTF8);
                }

                foreach (var megapackFile in ResourceDepot.Instance.GetMegapacks())
                {
                    writer.WriteLine($"Megapack: {megapackFile}");

                    foreach (var fileEntry in ResourceDepot.Instance.GetFileEntries(megapackFile))
                    {
                        writer.WriteLine(fileEntry.ToPrettyString());
                    }

                    writer.WriteLine();
                }
                
                writer.Flush();

                if (writer != Console.Out)
                {
                    writer.Close();
                }

                return true;
            }
        }

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

                /*var looseFilePath = Path.Combine(basePath, @"France\loosefiles_BinPC.pack");

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

                using var ms = new MemoryStream(globalMapEntry.Data);
                
                var globalMap = GlobalMapSerializer.DeserializeRaw(ms);

                foreach (var filePath in Megafiles)
                {
                    var fullPath = Path.Combine(basePath, filePath);
                    var megaOutDir = Path.Combine(outputDir, Path.GetFileName(fullPath));

                    if (!File.Exists(fullPath))
                        continue;

                    using var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    var megafile = GlobalMegaFileSerializer.DeserializeRaw(fs);
                    megafile.Export(fs, megaOutDir);
                }

                Console.WriteLine("Successfully unpacked the global megapacks!");
                return true;*/
                Console.WriteLine("Temporarily disabled...");
                return false;
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
