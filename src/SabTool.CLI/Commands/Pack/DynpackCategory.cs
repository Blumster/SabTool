using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands.Pack
{
    using Base;
    using Data;
    using Data.Packs;
    using Utils;

    public class DynpackCategory : BaseCategory
    {
        public override string Key => "dynpack";
        public override string Shortcut => "d";
        public override string Usage => "<sub command name>";

        public class UnpackCommand : BaseCommand
        {
            public override string Key { get; } = "unpack";
            public override string Shortcut { get; } = "u";
            public override string Usage { get; } = "<game base path> <dynpack file path> [output directory path]";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 2)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                var basePath = arguments.ElementAt(0);
                var dynpackFilePath = arguments.ElementAt(1);

                var outputDir = Path.GetDirectoryName(dynpackFilePath);

                if (Directory.Exists(dynpackFilePath))
                    outputDir = dynpackFilePath;

                if (arguments.Count() > 2)
                {
                    outputDir = arguments.ElementAt(2);
                }

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

                if (Directory.Exists(dynpackFilePath))
                {
                    var files = Directory.GetFiles(dynpackFilePath, "*.dynpack", SearchOption.TopDirectoryOnly);

                    if (files.Length == 0)
                    {
                        Console.WriteLine("ERROR: No files found in the input directory!");
                        return false;
                    }

                    foreach (var file in files)
                    {
                        if (!Process(file, outputDir, globalMap))
                            return false;
                    }

                    return true;
                }
                else if (File.Exists(dynpackFilePath))
                {
                    return Process(dynpackFilePath, outputDir, globalMap);
                }

                return false;
            }

            private static bool Process(string filePath, string outputDir, GlobalMap globalMap)
            {
                Console.WriteLine($"Processing {filePath}...");
                var crc = new Crc(Hash.StringToHash(Path.GetFileNameWithoutExtension(filePath)));

                var streamBlock = globalMap.GetDynamicBlock(crc);
                if (streamBlock == null)
                {
                    Console.WriteLine($"Dynamic StreamBlock {crc} was not found in global.map!");
                    return false;
                }

                outputDir = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(filePath));
                outputDir = outputDir.Replace(" ", "");

                Directory.CreateDirectory(outputDir);

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using var reader = new BinaryReader(fs);

                    streamBlock.Read(reader);
                    streamBlock.Export(outputDir);
                }

                Console.WriteLine();

                return true;
            }
        }
    }
}
