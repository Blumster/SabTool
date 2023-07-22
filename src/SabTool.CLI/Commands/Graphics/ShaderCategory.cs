
using HlslDecompiler;
using HlslDecompiler.DirectXShaderModel;

using SabTool.CLI.Base;
using SabTool.Depot;

namespace SabTool.CLI.Commands.Graphics;
public sealed class ShaderCategory : BaseCategory
{
    public override string Key => "shaders";
    public override string Shortcut => "s";
    public override string Usage => "<sub command>";

    public sealed class UnpackCommand : BaseCommand
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
            _ = ResourceDepot.Instance.Load(Resource.Shaders);

            string outputDir = arguments.ElementAt(1);

            _ = Directory.CreateDirectory(outputDir);

            using FileStream outFs = new(Path.Combine(outputDir, "shaders.txt"), FileMode.Create, FileAccess.Write, FileShare.None);
            using StreamWriter sw = new(outFs);

            sw.WriteLine("Pixel shaders:");

            int index = 0;
            foreach (Data.Graphics.Shaders.Shader shader in ResourceDepot.Instance.GetPixelShaders())
            {
                sw.WriteLine($"{index++}:");
                sw.WriteLine($"  i: {shader.Index} id: {shader.Id}");

                int i = 0;

                foreach (Data.Graphics.Shaders.ShaderData? data in shader.Data)
                {
                    string shaderFileName = $"{shader.Id.GetStringOrHexString()}_p_{i++}.fx";

                    sw.WriteLine($"  {i}: {shaderFileName}");
                    sw.WriteLine($"    Size: {data.Size}");

                    foreach (Data.Graphics.Shaders.ShaderConfigParameter? param in data.Parameters)
                        sw.WriteLine($"    Param: {param.Name} = {param.DefaultValue}");

                    if (data.Size == 0)
                        continue;

                    using MemoryStream stream = new(data.Data, false);
                    using ShaderReader shaderReader = new(stream, true);

                    HlslSimpleWriter writer = new(shaderReader.ReadShader());


                    writer.Write(Path.Combine(outputDir, shaderFileName));
                }
            }

            index = 0;
            sw.WriteLine("Vertex shaders:");

            foreach (Data.Graphics.Shaders.Shader shader in ResourceDepot.Instance.GetVertexShaders())
            {
                sw.WriteLine($"{index++}:");
                sw.WriteLine($"  i: {shader.Index} id: {shader.Id}");

                int i = 0;

                foreach (Data.Graphics.Shaders.ShaderData? data in shader.Data)
                {
                    string shaderFileName = $"{shader.Id.GetStringOrHexString()}_v_{i++}.fx";

                    sw.WriteLine($"  {i}: {shaderFileName}");
                    sw.WriteLine($"    Size: {data.Size}");

                    foreach (Data.Graphics.Shaders.ShaderConfigParameter? param in data.Parameters)
                        sw.WriteLine($"    Param: {param.Name} = {param.DefaultValue}");

                    if (data.Size == 0)
                        continue;

                    using MemoryStream stream = new(data.Data, false);
                    using ShaderReader shaderReader = new(stream, true);

                    HlslSimpleWriter writer = new(shaderReader.ReadShader());

                    writer.Write(Path.Combine(outputDir, shaderFileName));
                }
            }

            return true;
        }
    }

    public sealed class PackCommand : BaseCommand
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

            string inputFilePath = arguments.ElementAt(0);
            string outputDir = arguments.ElementAt(1);

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("ERROR: The materials input file path does not exist!");
                return false;
            }

            _ = Directory.CreateDirectory(outputDir);

            using FileStream inFileStream = new(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return true;
        }
    }
}
