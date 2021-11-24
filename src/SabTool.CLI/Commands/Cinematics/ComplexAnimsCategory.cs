using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands.Cinematics
{
    using Base;
    using Depot;
    using Serializers.Cinematics;

    public class ComplexAnimsCategory : BaseCategory
    {
        public override string Key { get; } = "complex-anims";
        public override string Shortcut { get; } = "ca";
        public override string Usage { get; } = "";

        public class UnpackCommand : BaseCommand
        {
            private const string ComplexAnimsRootPath = @"Cinematics\ComplexAnimations";

            public override string Key { get; } = "unpack";
            public override string Shortcut { get; } = "u";
            public override string Usage { get; } = "<game base path> <output dir path> [language]";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 2)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                var outputDirectory = arguments.ElementAt(1);
                Directory.CreateDirectory(outputDirectory);

                ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
                ResourceDepot.Instance.Load(Resource.Cinematics);

                var outputFilePath = Path.Combine(outputDirectory, ComplexAnimsRootPath, "complex-anims.json");

                var outputFileDirectory = Path.GetDirectoryName(outputFilePath);
                Directory.CreateDirectory(outputFileDirectory);

                using var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                ComplexAnimsSerializer.SerializeJSON(ResourceDepot.Instance.GetComplexAnimStructures().ToList(), fs);

                return true;
            }
        }

        public class PackCommand : BaseCommand
        {
            public override string Key { get; } = "pack";
            public override string Shortcut { get; } = "p";
            public override string Usage { get; } = "<game base path> <input file path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                Console.WriteLine("NOT YET IMPLEMENTED!");
                return false;
            }
        }
    }
}
