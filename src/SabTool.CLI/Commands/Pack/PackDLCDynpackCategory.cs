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

    public class PackDLCDynpackCategory : BaseCategory
    {
        public override string Key => "dlc-dynpack";
        public override string Shortcut => "dd";
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

                var globalFilePath = Path.Combine(basePath, @"DLC\01\Global.map");

                if (!File.Exists(globalFilePath))
                {
                    Console.WriteLine($"ERROR: Loosefile could not be found under \"{globalFilePath}\"!");
                    return false;
                }

                var globalMap = new GlobalMap();

                using (var ms = new FileStream(globalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using var reader = new BinaryReader(ms);

                    globalMap.Read(reader, globalFilePath);
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
                var crc = new Crc(Hash.StringToHash(Path.GetFileNameWithoutExtension(filePath)));

                var streamBlock = globalMap.GetDynamicBlock(crc);
                if (streamBlock == null)
                {
                    Console.WriteLine($"Dynamic StreamBlock {crc} was not found in global.map!");
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
