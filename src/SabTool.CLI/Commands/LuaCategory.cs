using System.Text;

using SabTool.CLI.Base;
using SabTool.Depot;

namespace SabTool.CLI.Commands;
public sealed class LuaCategory : BaseCategory
{
    public override string Key { get; } = "lua";
    public override string Shortcut { get; } = "lu";
    public override string Usage { get; } = "";

    public sealed class UnpackCommand : BaseCommand
    {
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

            string outputDirectory = arguments.ElementAt(1);
            _ = Directory.CreateDirectory(outputDirectory);

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            _ = ResourceDepot.Instance.Load(Resource.Lua);

            foreach (Data.Lua.LuaPackage.Entry entry in ResourceDepot.Instance.GetLuaEntries())
            {
                string outputPath = string.Empty;

                string path = entry.PathCRC.GetString();

                if (!string.IsNullOrEmpty(path))
                {
                    outputPath = @$"{outputDirectory}\{path[3..]}";
                }
                else
                {
                    // todo: make this more generic?
                    int sourceLen = BitConverter.ToInt32(entry.Data, 12);
                    string source = Encoding.UTF8.GetString(entry.Data, 17, sourceLen - 2);
                    if (source.StartsWith(@"D:\projects\WildStar\pov\BinCommon\"))
                    {
                        outputPath = @$"{outputDirectory}\{source[35..]}c";
                    }
                }

                if (!string.IsNullOrEmpty(outputPath))
                {
                    string? folder = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(folder))
                        _ = Directory.CreateDirectory(folder!);

                    File.WriteAllBytes(outputPath, entry.Data);
                }
                else
                {
                    Console.WriteLine($"ERROR: No output path could be calculated from Lua entry: {entry.PathCRC}");
                }
            }

            return true;
        }
    }

    public sealed class PackCommand : BaseCommand
    {
        public override string Key { get; } = "pack";
        public override string Shortcut { get; } = "p";
        public override string Usage { get; } = "<game base path> <input folder path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            Console.WriteLine("NOT YET IMPLEMENTED!");
            return false;
        }
    }

    public sealed class DecompileCommand : BaseCommand
    {
        public override string Key { get; } = "decompile";
        public override string Shortcut { get; } = "d";
        public override string Usage { get; } = "<game base path> <input folder path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            Console.WriteLine("NOT YET IMPLEMENTED!");
            return false;
        }
    }

    public sealed class CompileCommand : BaseCommand
    {
        public override string Key { get; } = "compile";
        public override string Shortcut { get; } = "c";
        public override string Usage { get; } = "<game base path> <input folder path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            Console.WriteLine("NOT YET IMPLEMENTED!");
            return false;
        }
    }
}
