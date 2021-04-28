using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SabTool.CLI.Commands
{
    using Base;
    using Utils;

    public class HashCategory : BaseCategory
    {
        public override string Key => "hash";

        public override string Usage => "<sub command>";

        public class CalcCommand : BaseCommand
        {
            public override string Key { get; } = "calc";
            public override string Usage { get; } = "<string to calculate hash from>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 1)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                Console.WriteLine("\"{0}\" => 0x{1:X8}", arguments.ElementAt(0), Hash.StringToHash(arguments.ElementAt(0)));
                return true;
            }
        }

        public class ResolveCommand : BaseCommand
        {
            public override string Key { get; } = "resolve";
            public override string Usage { get; } = "<hash value>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 1)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                var hashStr = arguments.ElementAt(0);
                if (hashStr.StartsWith("0x"))
                    hashStr = hashStr[2..];

                if (!uint.TryParse(hashStr, NumberStyles.HexNumber, null, out uint hash))
                {
                    Console.WriteLine("ERROR: Invalid hash was given!");
                    return false;
                }

                var value = Hash.HashToString(hash);

                if (!string.IsNullOrEmpty(value))
                {
                    Console.WriteLine("\"0x{0}\" => {1}", hashStr, value);
                }
                else
                {
                    Console.WriteLine("\"0x{0}\" => <NO STRING FOUND FOR THIS HASH>", hashStr, value);
                }
                
                return true;
            }
        }

        public class BruteforceCommand : BaseCommand
        {
            public override string Key { get; } = "bruteforce";
            public override string Usage { get; } = "<length> <hash>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 1)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                var lenStr = arguments.ElementAt(0);

                var hashStr = arguments.ElementAt(1);
                if (hashStr.StartsWith("0x"))
                    hashStr = hashStr[2..];

                if (int.TryParse(lenStr, out int length) && uint.TryParse(hashStr, NumberStyles.HexNumber, null, out uint hash))
                {
                    Hash.Bruteforce(length, hash);

                    return true;
                }

                Console.WriteLine("ERROR: Invalid length argument given!");
                return false;
            }
        }

        public class GenerateFromFileCommand : BaseCommand
        {
            public override string Key { get; } = "generate-from-file";
            public override string Usage { get; } = "<input file path> [output file path]";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 1)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                var inputFilePath = arguments.ElementAt(0);
                if (!File.Exists(inputFilePath))
                {
                    Console.WriteLine("ERROR: Non-existant input file given!");
                    return false;
                }

                var outFilePath = Path.Join(Path.GetDirectoryName(inputFilePath), $"{Path.GetFileNameWithoutExtension(inputFilePath)}-hash{Path.GetExtension(inputFilePath)}");

                if (arguments.Count() == 2)
                {
                    outFilePath = arguments.ElementAt(1);
                }

                var lines = File.ReadAllLines(inputFilePath);

                for (var i = 0; i < lines.Length; ++i)
                {
                    lines[i] = $"0x{Hash.FNV32string(lines[i]):X8}:{lines[i]}";
                }

                File.WriteAllLines(outFilePath, lines);

                return true;
            }
        }
    }
}
