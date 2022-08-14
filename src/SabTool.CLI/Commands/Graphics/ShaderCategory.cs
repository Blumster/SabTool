using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HlslDecompiler;
using HlslDecompiler.DirectXShaderModel;

namespace SabTool.CLI.Commands.Graphics;

using Base;
using Depot;
using SabTool.Data.Graphics.Shaders;
using Serializers.Graphics.Shaders;

public class ShaderCategory : BaseCategory
{
    public const string MaterialsRawFileName = "France.shaders";
    public const string MaterialsJsonFileName = "shaders.json";

    public override string Key => "shaders";

    public override string Shortcut => "s";

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

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Shaders);

            var outputDir = arguments.ElementAt(1);

            Directory.CreateDirectory(outputDir);

            //using var outFileStream = new FileStream(Path.Combine(outputDir, MaterialsJsonFileName), FileMode.Create, FileAccess.Write, FileShare.None);

            foreach (var shader in ResourceDepot.Instance.GetPixelShaders())
            {
                var i = 0;

                foreach (var data in shader.Data)
                {
                    if (data.Size == 0)
                        continue;

                    using var stream = new MemoryStream(data.Data, false);
                    using var shaderReader = new ShaderReader(stream, true);

                    var writer = new HlslSimpleWriter(shaderReader.ReadShader());

                    writer.Write(Path.Combine(outputDir, $"{shader.Id.GetStringOrHexString()}_{i++}.fx"));
                }
            }

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

            return true;
        }
    }
}
