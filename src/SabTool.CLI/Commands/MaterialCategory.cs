using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands
{
    using Base;
    using Serializers.Graphics;

    public class MaterialCategory : BaseCategory
    {
        public const string MaterialsRawFileName = "France.materials";
        public const string MaterialsJsonFileName = "materials.json";

        public override string Key => "materials";

        public override string Shortcut => "ma";

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

                var materialsFilePath = Path.Combine(basePath, MaterialsRawFileName);
                if (!File.Exists(materialsFilePath))
                {
                    Console.WriteLine($"ERROR: Materials file could not be found under \"{materialsFilePath}\"!");
                    return false;
                }

                using var inFileStream = new FileStream(materialsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var materials = MaterialSerializer.DeserializeRaw(inFileStream);

                using var outFileStream = new FileStream(Path.Combine(outputDir, MaterialsJsonFileName), FileMode.Create, FileAccess.Write, FileShare.None);

                MaterialSerializer.SerializeJSON(materials, outFileStream);

                return true;
            }
        }

        public class PackCommand : BaseCommand
        {
            public override string Key { get; } = "pack";
            public override string Shortcut { get; } = "p";
            public override string Usage { get; } = "<input file path> <output dir path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 2)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                var inputFilePath = arguments.ElementAt(0);
                var outputDir = arguments.ElementAt(1);

                if (!File.Exists(inputFilePath))
                {
                    Console.WriteLine("ERROR: The materials input file path does not exist!");
                    return false;
                }

                Directory.CreateDirectory(outputDir);

                using var inFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var materials = MaterialSerializer.DeserialzieJSON(inFileStream);

                using var outFileStream = new FileStream(Path.Combine(outputDir, MaterialsRawFileName), FileMode.Create, FileAccess.Write, FileShare.None);

                MaterialSerializer.SerializeRaw(materials, outFileStream);

                return true;
            }
        }
    }
}
