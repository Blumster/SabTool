using System;
using System.Linq;

namespace SabTool.CLI
{
    using Base;
    using Utils;

    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading Hashes...");

            Hash.FNV32string("dummy, to statically init the Hash class");

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
                if (cmd == "exit" || cmd == "e")
                    break;

                CommandParser.ExecuteCommand(cmd);
            }
        }
    }
}
