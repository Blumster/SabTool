using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    }
}
