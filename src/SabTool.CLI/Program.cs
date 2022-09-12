namespace SabTool.CLI;

using SabTool.CLI.Base;
using SabTool.Utils;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Loading Hashes...");

        Hash.FNV32string("dummy, to statically init the Hash class", addToLookup: false);

        Console.WriteLine("Loaded hashes!");

        if (args.Length > 0 && args[1] == "exec")
        {
            CommandParser.ExecuteCommand(args.Skip(1));
            return;
        }

        while (true)
        {
            Console.Write("sab-tool> ");

            var cmd = Console.ReadLine();
            if (cmd == null)
                continue;

            if (cmd == "exit" || cmd == "e")
                break;

            CommandParser.ExecuteCommand(cmd);
        }
    }
}
