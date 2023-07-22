
using SabTool.CLI.Base;
using SabTool.Utils;

namespace SabTool.CLI;
public sealed class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Loading Hashes...");

        _ = Hash.FNV32string("dummy, to statically init the Hash class", addToLookup: false);

        Console.WriteLine("Loaded hashes!");

        if (args.Length > 0 && args[1] == "exec")
        {
            _ = CommandParser.ExecuteCommand(args.Skip(1));
            return;
        }

        while (true)
        {
            Console.Write("sab-tool> ");

            string? cmd = Console.ReadLine();
            if (cmd == null)
                continue;

            if (cmd is "exit" or "e")
                break;

            _ = CommandParser.ExecuteCommand(cmd);
        }
    }
}
