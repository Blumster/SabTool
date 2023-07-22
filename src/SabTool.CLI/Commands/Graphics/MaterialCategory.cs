
using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Graphics;

namespace SabTool.CLI.Commands.Graphics;
public sealed class MaterialCategory : BaseCategory
{
    public const string MaterialsRawFileName = "France.materials";
    public const string MaterialsJsonFileName = "materials.json";

    public override string Key => "materials";
    public override string Shortcut => "ma";
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

            if (!ResourceDepot.IsInitialized)
            {
                ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            }

            _ = ResourceDepot.Instance.Load(Resource.Materials);

            string outputDir = arguments.ElementAt(1);

            _ = Directory.CreateDirectory(outputDir);

            using FileStream outFileStream = new(Path.Combine(outputDir, MaterialsJsonFileName), FileMode.Create, FileAccess.Write, FileShare.None);

            IEnumerable<KeyValuePair<Utils.Crc, Data.Graphics.Material>> materials = ResourceDepot.Instance.GetMaterials();

            MaterialSerializer.SerializeJSON(materials.Select(p => p.Value).ToList(), outFileStream);

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

            List<Data.Graphics.Material> materials = MaterialSerializer.DeserialzieJSON(inFileStream);

            using FileStream outFileStream = new(Path.Combine(outputDir, MaterialsRawFileName), FileMode.Create, FileAccess.Write, FileShare.None);

            MaterialSerializer.SerializeRaw(materials, outFileStream);

            return true;
        }
    }
}
