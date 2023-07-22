
using SabTool.CLI.Base;
using SabTool.Data.Graphics.Shaders;
using SabTool.Serializers.Graphics.Shaders;
using SabTool.Utils;

namespace SabTool.CLI.Commands.Graphics;
public sealed class ShaderMappingCategory : BaseCategory
{
    public override string Key => "shadermappings";
    public override string Shortcut => "sm";
    public override string Usage => "<sub command>";

    public sealed class UnpackCommand : BaseCommand
    {
        public override string Key { get; } = "unpack";
        public override string Shortcut { get; } = "u";
        public override string Usage { get; } = "<input file path> <output directory>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            string inputFile = arguments.ElementAt(0);
            string outputDir = arguments.ElementAt(1);

            _ = Directory.CreateDirectory(outputDir);

            using FileStream fs = new(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using FileStream outFs = new(Path.Combine(outputDir, "shadermappings.txt"), FileMode.Create, FileAccess.Write, FileShare.None);
            using StreamWriter sw = new(outFs);

            ShaderMapping mapping = ShaderMappingSerializer.DeserializeRaw(fs);

            foreach (ShaderMappingData? data in mapping.Mappings)
            {
                sw.WriteLine($"Unk: {data.Unk}");
                sw.WriteLine($"Pass: {data.Pass}");
                sw.WriteLine($"Unk: {data.Unk2}");
                sw.WriteLine($"Unk: {data.Unk3}");

                sw.WriteLine(new Crc(Hash.StringToHash($"{data.Unk.Value:x}_{data.Pass.Value:x}_P")));
                sw.WriteLine(new Crc(Hash.StringToHash($"{data.Unk.Value:x}_{data.Pass.Value:x}_V")));
                sw.WriteLine();
            }

            return true;
        }
    }
}
