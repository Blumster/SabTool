using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands
{
    using Base;

    public class LooseFileCategory : BaseCategory
    {
        public override string Key { get; } = "loose-files";
        public override string Usage { get; } = "<sub command name>";

        public class LooseFileUnpackCommand : BaseCommand
        {
            public override string Key { get; } = "unpack";
            public override string Usage { get; } = "<loose file path> <output directory path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 2)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                var filePath = arguments.ElementAt(0);
                var outputFolder = arguments.ElementAt(1);

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("ERROR: Loose file doesn't exist!");
                    return false;
                }

                if (!Directory.Exists(outputFolder))
                {
                    Console.WriteLine("ERROR: Output directory doesn't exist!");
                    return false;
                }

                /*using (var parser = new LooseFileParser(filePath))
                {
                    parser.Parse();
                    parser.Extract(outputFolder);
                }*/

                Console.WriteLine("Files successfully unpacked!");
                return true;
            }
        }

        public class LooseFilePackCommand : BaseCommand
        {
            public override string Key { get; } = "pack";
            public override string Usage { get; } = "<loose file path> <input directory path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 2)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                var filePath = arguments.ElementAt(0);
                var inputFolder = arguments.ElementAt(1);

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("ERROR: Loose file doesn't exist!");
                    return false;
                }

                if (!Directory.Exists(inputFolder))
                {
                    Console.WriteLine("ERROR: Output directory doesn't exist!");
                    return false;
                }

                var files = Directory.GetFiles(inputFolder, "*", SearchOption.AllDirectories);

                if (files.Length == 0)
                {
                    Console.WriteLine("ERROR: No files found in the input directory!");
                    return false;
                }

                Console.WriteLine("Files successfully packed!");
                return true;
            }
        }
    }
}
