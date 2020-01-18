using System;
using System.Linq;

namespace SabTool.CLI
{
    using Base;
    using Client.Pebble;

    public class Program
    {
        const string GameBase = @"G:\Games\The Saboteur\";

        public static string GetFilePath(string relPath)
        {
            return string.Format("{0}{1}", GameBase, relPath);
        }

        static void Main(string[] args)
        {
            PblLooseFile.GetStaticDiscFileInstance(GetFilePath(@"France\loosefiles_Binpc.pack"));

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

                Console.WriteLine();
            }

            /*using (var fs = new FileStream(@"X:\RE\Projects\The Saboteur\global.map", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var mapParser = new MapParser(fs, "global.map"))
                {
                    mapParser.Parse();
                }
            }*/
        }
    }
}
