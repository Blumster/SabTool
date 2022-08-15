using HlslDecompiler;
using HlslDecompiler.DirectXShaderModel;

namespace SabTool.CLI.Commands.Graphics;

using Base;
using Depot;

public class ShaderCategory : BaseCategory
{
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

            using var outFs = new FileStream(Path.Combine(outputDir, "shaders.txt"), FileMode.Create, FileAccess.Write, FileShare.None);
            using var sw = new StreamWriter(outFs);

            sw.WriteLine("Pixel shaders:");

            var index = 0;
            foreach (var shader in ResourceDepot.Instance.GetPixelShaders())
            {
                sw.WriteLine($"{index++}:");
                sw.WriteLine($"  i: {shader.Index} id: {shader.Id}");

                var i = 0;

                foreach (var data in shader.Data)
                {
                    var shaderFileName = $"{shader.Id.GetStringOrHexString()}_p_{i++}.fx";

                    sw.WriteLine($"  {i}: {shaderFileName}");
                    sw.WriteLine($"    Size: {data.Size}");

                    foreach (var param in data.Parameters)
                        sw.WriteLine($"    Param: {param.Name} = {param.DefaultValue}");

                    if (data.Size == 0)
                        continue;

                    using var stream = new MemoryStream(data.Data, false);
                    using var shaderReader = new ShaderReader(stream, true);

                    var writer = new HlslSimpleWriter(shaderReader.ReadShader());

                    
                    writer.Write(Path.Combine(outputDir, shaderFileName));
                }
            }

            index = 0;
            sw.WriteLine("Vertex shaders:");

            foreach (var shader in ResourceDepot.Instance.GetVertexShaders())
            {
                sw.WriteLine($"{index++}:");
                sw.WriteLine($"  i: {shader.Index} id: {shader.Id}");

                var i = 0;

                foreach (var data in shader.Data)
                {
                    var shaderFileName = $"{shader.Id.GetStringOrHexString()}_v_{i++}.fx";

                    sw.WriteLine($"  {i}: {shaderFileName}");
                    sw.WriteLine($"    Size: {data.Size}");

                    foreach (var param in data.Parameters)
                        sw.WriteLine($"    Param: {param.Name} = {param.DefaultValue}");

                    if (data.Size == 0)
                        continue;

                    using var stream = new MemoryStream(data.Data, false);
                    using var shaderReader = new ShaderReader(stream, true);

                    var writer = new HlslSimpleWriter(shaderReader.ReadShader());

                    writer.Write(Path.Combine(outputDir, shaderFileName));
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
