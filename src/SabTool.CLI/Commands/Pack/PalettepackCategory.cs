using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands.Pack
{
    using Base;
    using Data;
    using Data.Packs;
    using Data.Structures;
    using Utils;

    public class PalettepackCategory : BaseCategory
    {
        public override string Key => "palettepack";
        public override string Shortcut => "p";
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
                var palettepackFilePath = arguments.ElementAt(1);

                var outputDir = Path.GetDirectoryName(palettepackFilePath);

                if (Directory.Exists(palettepackFilePath))
                    outputDir = palettepackFilePath;

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

                if (Directory.Exists(palettepackFilePath))
                {
                    var files = Directory.GetFiles(palettepackFilePath, "*.palettepack", SearchOption.TopDirectoryOnly);

                    if (files.Length == 0)
                    {
                        Console.WriteLine("ERROR: No files found in the input directory!");
                        return false;
                    }

                    foreach (var file in files)
                    {
                        Process(file, outputDir, globalMap);
                    }

                    return true;
                }
                else if (File.Exists(palettepackFilePath))
                {
                    return Process(palettepackFilePath, outputDir, globalMap);
                }

                return false;
            }

            private static bool Process(string filePath, string outputDir, GlobalMap globalMap)
            {
                Crc crc = null;

                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName.StartsWith("0x"))
                {
                    crc = new Crc(uint.Parse(fileName[2..], System.Globalization.NumberStyles.HexNumber));
                }
                else
                {
                    crc = new Crc(Hash.StringToHash(fileName));
                }

                var streamBlock = globalMap.GetStaticBlock(crc);
                if (streamBlock == null)
                {
                    Console.WriteLine($"Static StreamBlock {crc} was not found in global.map!");
                    return false;
                }

                outputDir = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(filePath));

                Directory.CreateDirectory(outputDir);

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using var reader = new BinaryReader(fs);

                    streamBlock.Read(reader);
                    streamBlock.Export(outputDir);
                }

                return true;
            }
        }
    }
}
