using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands
{
    using Base;
    using Depot;
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

                if (!ResourceDepot.IsInitialized)
                {
                    ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
                }

                ResourceDepot.Instance.Load(Resource.Materials);

                var outputDir = arguments.ElementAt(1);

                Directory.CreateDirectory(outputDir);

                using var outFileStream = new FileStream(Path.Combine(outputDir, MaterialsJsonFileName), FileMode.Create, FileAccess.Write, FileShare.None);

                var materials = ResourceDepot.Instance.GetMaterials();

                MaterialSerializer.SerializeJSON(materials.Select(p => p.Value).ToList(), outFileStream);

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
