using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SabTool.Utils
{
    public static class Hash
    {
        private static readonly Dictionary<uint, string> lookupTable = new Dictionary<uint, string>();

        public const uint FNV32Offset = 0x811C9DC5u;
        public const uint FNV32Prime = 0x1000193u;

        static Hash()
        {
            foreach (var line in File.ReadAllLines("Hashes.txt"))
            {
                var parts = line.Split(':');
                if (parts.Length < 2)
                {
                    Console.WriteLine($"HASH: Invalid line found: \"{line}\"");
                    continue;
                }

                if (!uint.TryParse(parts[0].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint hash))
                {
                    Console.WriteLine($"HASH: Unable to parse {parts[0]} as int!");
                    continue;
                }

                if (lookupTable.ContainsKey(hash))
                {
                    if (lookupTable[hash].ToLowerInvariant() != parts[1].ToLowerInvariant())
                        Console.WriteLine($"HASH: Matching hashes for different string! Hash: 0x{hash:X8}: \"{parts[1]}\" != \"{lookupTable[hash]}\"");

                    continue;
                }

                lookupTable.Add(hash, parts[1]);
            }

            /* Debug code: It writes back the contents of the lookup table. This way redundant entries can be filtered out after addig rows to the file.
             * Notice: It will overwrite the file that was copied to the output directory, not the one in the project files!
            var lines = new string[lookupTable.Count];
            var i = 0;
            foreach (var pair in lookupTable)
            {
                lines[i++] = $"0x{pair.Key:X8}:{pair.Value}";
            }

            File.WriteAllLines("Hashes.txt", lines);
            */
        }

        public static uint FNV32string(string source, int maxLen = -1)
        {
            if (string.IsNullOrEmpty(source))
                return 0;

            var bytes = Encoding.UTF8.GetBytes(source);
            var hash = FNV32Offset;

            for (var i = 0; i < bytes.Length && (maxLen == -1 || i < maxLen); ++i)
                hash = FNV32Prime * (hash ^ (bytes[i] | 0x20u));
            
            return (hash ^ 0x2Au) * FNV32Prime;
        }

        public static string HashToString(uint hash)
        {
            if (lookupTable.ContainsKey(hash))
                return lookupTable[hash];

            return null;
        }

        public static uint StringToHash(string source)
        {
            foreach (var pair in lookupTable)
                if (pair.Value.ToLowerInvariant() == source.ToLowerInvariant())
                    return pair.Key;

            var hash = FNV32string(source);

            if (!lookupTable.ContainsKey(hash))
            {
                lookupTable.Add(hash, source);

                return hash;
            }

            if (source.ToLowerInvariant() != lookupTable[hash].ToLowerInvariant())
            {
                Console.WriteLine($"HASH: Different string for the same hash! Hash: 0x{hash:X8}: \"{source}\" != \"{lookupTable[hash]}\"");
                return 0xFFFFFFFFu;
            }

            return hash;
        }

        private const int CharCount = 45;
        private static char[] Characters = new char[CharCount] {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
            'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
            'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '_', '-', '.', ',', '/', ' ', '+', '\'', '\\'
        };
        private static readonly StringBuilder Builder = new();

        public static void Bruteforce(int length, uint hash)
        {
            /*var lastLine = File.ReadAllLines("X:\\Projects\\The_Saboteur\\CalculatedHashes.txt").Last();

            var sepIndex = lastLine.IndexOf(':');
            lastLine = lastLine[(sepIndex + 1)..];

            if (lastLine.Length > length)
            {
                Console.WriteLine("Bruteforce: Already processed longer words!");
                return;
            }

            if (lastLine.Length < length - 1)
            {
                Console.WriteLine("Bruteforce: Not yet processed all the previous lengths!");
                return;
            }

            if (lastLine.Length == length - 1 && lastLine != new string('\\', length - 1))
            {
                Console.WriteLine("Bruteforce: The previous length is not fully processed!");
                return;
            }

            if (lastLine.Length == length && lastLine == new string('\\', length))
            {
                Console.WriteLine("Bruteforce: The length is already finished!");
                return;
            }*/

            while (true)
            {
                var strVals = new int[length];

                /*if (lastLine.Length == length)
                {
                    int off = 0;

                    foreach (var c in lastLine)
                    {
                        for (var i = 0; i < CharCount; ++i)
                        {
                            if (c == Characters[i])
                            {
                                strVals[off] = i;
                                break;
                            }
                        }

                        ++off;
                    }
                }*/

                //using var sw = new StreamWriter(new FileStream("X:\\Projects\\The_Saboteur\\CalculatedHashes.txt", FileMode.Append, FileAccess.Write, FileShare.None));

                long total = (long)Math.Pow(CharCount, length);
                long curr = AlreadyDone(strVals) + 1;
                var stopAfter = false;

                do
                {
                    var str = CalcString(strVals);

                    if (FNV32string(str) == hash)
                    {
                        Console.WriteLine($"Bruteforce: {str} => 0x{hash:X8}");
                        stopAfter = true;
                    }

                    //sw.WriteLine($"0x{FNV32string(str):X8}:{str}");

                    if (++curr % 100000000 == 0)
                    {
                        Console.Title = $"Bruteforce: {length}: {curr}/{total}: {curr/(double)total*100.0:4}%";

                        //sw.Flush();
                    }
                }
                while (IncString(strVals));

                ++length;

                if (stopAfter)
                    break;
            }
        }

        private static bool IncString(int[] values)
        {
            for (var i = values.Length; i > 0; --i)
            {
                // return false if the first character would overflow
                if (i == 1 && values[i - 1] == CharCount - 1)
                    return false;

                // increase the last character
                values[i - 1] += 1;

                // check if the character overflowed
                if (values[i - 1] < CharCount)
                {
                    return true;
                }

                // on overflow, set to zero and increase the next character
                values[i - 1] = 0;
            }

            return false;
        }

        private static string CalcString(int[] values)
        {
            Builder.Clear();

            for (var i = 0; i < values.Length; ++i)
            {
                Builder.Append(Characters[values[i]]);
            }

            return Builder.ToString();
        }

        private static long AlreadyDone(int[] values)
        {
            long val = 0;
            int pow = 0;

            for (var i = values.Length; i > 0; --i)
            {
                val += (long)Math.Pow(CharCount, pow++) * values[i - 1];
            }

            return val;
        }
    }
}
