using System.Text;

namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;
using SabTool.Depot;

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

            var outputDirectory = arguments.ElementAt(1);
            Directory.CreateDirectory(outputDirectory);

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Lua);

            foreach (var entry in ResourceDepot.Instance.GetLuaEntries())
            {
                var outputPath = string.Empty;

                var path = entry.PathCRC.GetString();

                if (!string.IsNullOrEmpty(path))
                {
                    outputPath = @$"{outputDirectory}\{path[3..]}";
                }
                else
                {
                    // todo: make this more generic?
                    var sourceLen = BitConverter.ToInt32(entry.Data, 12);
                    var source = Encoding.UTF8.GetString(entry.Data, 17, sourceLen - 2);
                    if (source.StartsWith(@"D:\projects\WildStar\pov\BinCommon\"))
                    {
                        outputPath = @$"{outputDirectory}\{source[35..]}c";
                    }
                }

                if (!string.IsNullOrEmpty(outputPath))
                {
                    var folder = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder!);

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
