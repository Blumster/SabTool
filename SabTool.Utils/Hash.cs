using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        private const int TaskCount = 2;
        private static long[] Progress = new long[TaskCount];
        private static StringBuilder[] Builders = new StringBuilder[TaskCount];

        public static void Bruteforce(int length, uint hash)
        {
            /*while (true)
            {
                var strVals = new int[length];

                long total = (long)Math.Pow(CharCount, length);
                long curr = AlreadyDone(strVals) + 1;
                long onePct = (long)Math.Max(total / 100.0d, 1000000.0d);
                var stopAfter = false;

                do
                {
                    var str = CalcString(strVals);

                    if (FNV32string(str) == hash)
                    {
                        Console.WriteLine($"Bruteforce: {str} => 0x{hash:X8}");
                        stopAfter = true;
                    }

                    if (++curr % onePct == 0)
                    {
                        Console.Title = $"Bruteforce: {length}: {curr}/{total}: {(double)curr/(double)total*100.0:0.00}%";
                    }
                }
                while (IncString(strVals));

                ++length;

                if (stopAfter)
                    break;
            }*/

            var tasks = new List<Task<bool>>();

            while (true)
            {
                tasks.Clear();

                var stopAfter = false;

                long total = (long)Math.Pow(CharCount, length);
                long onePct = (long)(total / 100.0d);
                long oneTaskCount = total / TaskCount;
                long remaining = total;
                long totalCount = 0;

                // Create and start tasks
                for (var i = 0; i < TaskCount; ++i)
                {
                    if (Builders[i] == null)
                        Builders[i] = new();

                    var strVals = new int[length];
                    var count = oneTaskCount;

                    // Compensate for integer division, the last task takes all the remaining work
                    if (i == TaskCount - 1)
                    {
                        count = remaining;
                    }

                    // Setup the start values for the task
                    if (totalCount > 0)
                    {
                        AddCount(strVals, totalCount);
                    }

                    var id = i;
                    var localCount = count;
                    var localStrVals = strVals;

                    Console.WriteLine($"Task {id} from {totalCount,10} to {totalCount + localCount - 1,10} ({total,10}) ({CalcString(localStrVals, id),10}) with count {localCount,10} looking for hash 0x{hash:X8}");

                    // Start and store the task
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        return Bruteforce(id, localStrVals, hash, localCount);
                    }));

                    // Update the counters
                    remaining -= count;
                    totalCount += count;
                }

                Console.WriteLine();

                var lastCurr = 0L;

                while (true)
                {
                    var stop = true;

                    // Check if the tasks have completed
                    foreach (var task in tasks)
                    {
                        if (!task.IsCompleted)
                        {
                            stop = false;
                        }
                        else if (task.Result)
                        {
                            stopAfter = true;
                        }
                    }

                    if (stop)
                        break;

                    var curr = 0L;

                    foreach (var current in Progress)
                    {
                        curr += current;
                    }

                    if (curr - lastCurr > onePct)
                    {
                        Console.Title = $"Bruteforce: {length}: {curr}/{total}: {curr / (double)total * 100.0d:0.00}%";

                        lastCurr = curr;
                    }

                    Thread.Sleep(100);
                }

                Task.WaitAll(tasks.ToArray());

                ++length;

                if (stopAfter)
                    break;
            }
        }

        
        private static bool Bruteforce(int taskId, int[] strVals, uint hash, long itrCount)
        {
            var stopAfter = false;
            var i = 0L;

            Progress[taskId] = 0;

            do
            {
                var str = CalcString(strVals, taskId);

                if (FNV32string(str) == hash)
                {
                    Console.WriteLine($"Bruteforce: {str} => 0x{hash:X8}");
                    stopAfter = true;
                }

                Progress[taskId] += 1;

                if (++i == itrCount)
                    break;
            }
            while (IncString(strVals));

            return stopAfter;
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

        private static void AddCount(int[] values, long count)
        {
            for (var i = values.Length; i > 0; --i)
            {
                values[i - 1] = (int)(count % CharCount);

                count /= CharCount;
            }
        }

        private static string CalcString(int[] values, int taskId)
        {
            Builders[taskId].Clear();

            for (var i = 0; i < values.Length; ++i)
            {
                Builders[taskId].Append(Characters[values[i]]);
            }

            return Builders[taskId].ToString();
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
