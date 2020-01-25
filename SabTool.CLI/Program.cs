using System;
using System.Linq;

namespace SabTool.CLI
{
    using Base;

    public class Program
    {
        const string GameBase = @"G:\Games\The Saboteur\";

        public static string GetFilePath(string relPath)
        {
            return string.Format("{0}{1}", GameBase, relPath);
        }

        static void Main(string[] args)
        {
            if (args.Length > 0 && args[1] == "exec")
            {
                CommandParser.ExecuteCommand(args.Skip(1));
                return;
            }

            while (true)
            {
                Console.Write("Enter a command: ");

                var cmd = Console.ReadLine();
                if (cmd == "exit")
                    break;

                CommandParser.ExecuteCommand(cmd);
            }
        }
    }
}
