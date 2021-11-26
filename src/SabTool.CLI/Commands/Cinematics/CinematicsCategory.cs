using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabTool.CLI.Commands.Cinematics
{
    using Base;
    using Depot;
    using Serializers.Cinematics;

    public class CinematicsCategory : BaseCategory
    {
        public override string Key { get; } = "cinematics";
        public override string Shortcut { get; } = "c";
        public override string Usage { get; } = "";

        public class UnpackCommand : BaseCommand
        {
            private const string CinematicsRootPath = @"Cinematics";
            private const string CienmaticsRootPathDLC = @"DLC\01\Cinematics";

            public override string Key { get; } = "unpack";
            public override string Shortcut { get; } = "u";
            public override string Usage { get; } = "<game base path> <output dir path>";

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

                // Main
                var outputFilePath = Path.Combine(outputDirectory, CinematicsRootPath, "cinematics.json");

                var outputFileDirectory = Path.GetDirectoryName(outputFilePath);
                Directory.CreateDirectory(outputFileDirectory);

                using var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                CinematicsSerializer.SerializeJSON(ResourceDepot.Instance.GetCinematics().ToList(), fs);

                // DLC
                outputFilePath = Path.Combine(outputDirectory, CienmaticsRootPathDLC, "cinematics.json");

                outputFileDirectory = Path.GetDirectoryName(outputFilePath);
                Directory.CreateDirectory(outputFileDirectory);

                using var fsDLC = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                CinematicsSerializer.SerializeJSON(ResourceDepot.Instance.GetDLCCinematics().ToList(), fsDLC);

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
