using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SabTool.Utils;

public static class Hash
{
    private static readonly Dictionary<uint, string> lookupTable = new();
    private static readonly HashSet<uint> missingHashes = new();

    public const uint FNV32Offset = 0x811C9DC5u;
    public const uint FNV32Prime = 0x1000193u;

    static Hash()
    {
        foreach (var line in File.ReadAllLines("Hashes.txt"))
        {
            var parts = line.Split(':', 2);
            if (parts.Length < 2)
            {
                Console.WriteLine($"HASH: Invalid line found: \"{line}\"");
                continue;
            }

            if (!uint.TryParse(parts[0][2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint hash))
            {
                Console.WriteLine($"HASH: Unable to parse {parts[0]} as uint!");
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

        if (File.Exists("Missing.txt"))
        {
            foreach (var line in File.ReadAllLines("Missing.txt"))
            {
                if (!uint.TryParse(line[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint hash))
                {
                    Console.WriteLine($"HASH: Unable to parse {line} as uint!");
                    continue;
                }

                missingHashes.Add(hash);
            }
        }

        /* Debug code: It writes back the contents of the lookup table. This way redundant entries can be filtered out after addig rows to the file.
         * Notice: It will overwrite the file that was copied to the output directory, not the one in the project files!
        Save();
        SaveMissing();
        */
    }

    public static void Save()
    {
        Cleanup();

        Console.WriteLine("Saving hashes...");

        var lines = new string[lookupTable.Count];
        var i = 0;

        foreach (var pair in lookupTable.OrderBy(p => p.Key))
        {
            lines[i++] = $"0x{pair.Key:X8}:{pair.Value}";
        }

        File.WriteAllLines("Hashes.txt", lines);

        Console.WriteLine("Saving missing hashes...");

        lines = new string[missingHashes.Count];
        i = 0;

        foreach (var hash in missingHashes.OrderBy(h => h))
        {
            lines[i++] = $"0x{hash:X8}";
        }

        File.WriteAllLines("Missing.txt", lines);
    }

    public static void Cleanup()
    {
        Console.WriteLine("Cleaning up hashes...");

        var toRemove = new List<uint>();
        var toRecalc = new List<string>();

        foreach (var hash in lookupTable)
        {
            var reHash = InternalFNV32string(hash.Value);
            if (reHash != hash.Key)
            {
                Console.WriteLine($"Invalid saved hash 0x{hash.Key:X8} found for \"0x{reHash:X8} -> {hash.Value}\"!");

                toRemove.Add(hash.Key);
                toRecalc.Add(hash.Value);
                continue;
            }

            if (missingHashes.Remove(hash.Key))
            {
                Console.WriteLine($"Removed already known hash \"0x{hash.Key:X8} -> {hash.Value}\" from missing hashes!");
            }
        }

        foreach (var hash in toRemove)
            lookupTable.Remove(hash);

        foreach (var str in toRecalc)
            StringToHash(str);
    }

    public static void PrintStatistics()
    {
        Console.WriteLine("Hash statistics:");
        Console.WriteLine($"  Table entry count: {lookupTable.Count}");
        Console.WriteLine($"  Missing hash count: {missingHashes.Count}");
        Console.WriteLine();
    }

    public static uint FNV32string(string source, int maxLen = -1, bool addToLookup = true)
    {
        var hash = InternalFNV32string(source, maxLen);

        if (addToLookup && missingHashes.Remove(hash))
        {
            if (lookupTable.ContainsKey(hash))
            {
                if (source.ToLowerInvariant() != lookupTable[hash].ToLowerInvariant())
                {
                    Console.WriteLine($"HASH: Different string for the same hash! Hash: 0x{hash:X8}: \"{source}\" != \"{lookupTable[hash]}\"");
                }
            }
            else
            {
                lookupTable.Add(hash, source);

                Console.WriteLine($"Found hash 0x{hash:X8}:{source}");
            }
        }

        return hash;
    }

    private static uint InternalFNV32string(string source, int maxLen = -1)
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

        missingHashes.Add(hash);

        return null;
    }

    public static uint StringToHash(string source)
    {
        /*foreach (var pair in lookupTable)
            if (pair.Value.ToLowerInvariant() == source.ToLowerInvariant())
                return pair.Key;*/

        var hash = InternalFNV32string(source);

        if (!lookupTable.ContainsKey(hash))
        {
            lookupTable.Add(hash, source);

            missingHashes.Remove(hash);

            return hash;
        }

        if (source.ToLowerInvariant() != lookupTable[hash].ToLowerInvariant())
        {
            Console.WriteLine($"HASH: Different string for the same hash! Hash: 0x{hash:X8}: \"{source}\" != \"{lookupTable[hash]}\"");
            return 0xFFFFFFFFu;
        }

        return hash;
    }

    private const int BasicCharCount = 26;
    private const int CharCount = 45;
    private static readonly char[] Characters = new char[CharCount] {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
        'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
        'u', 'v', 'w', 'x', 'y', 'z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        '_', '-', '.', ',', '/', ' ', '+', '\'', '\\'
    };

    private const int TaskCount = 6;
    private static readonly long[] Progress = new long[TaskCount];
    private static readonly StringBuilder[] Builders = new StringBuilder[TaskCount];

    public static void BruteforceMissing(int length, bool basic = false, int maxLength = -1)
    {
        var missing = missingHashes.Skip(10000);
        foreach (var hash in missing)
        {
            Bruteforce(length, hash, basic, maxLength);

            //Console.WriteLine("Press enter to continue...");
            //Console.ReadKey();
        }
    }

    public static void Bruteforce(int length, uint hash, bool basic = false, int maxLength = -1)
    {
        var tasks = new List<Task<bool>>();

        while (maxLength == -1 || length <= maxLength)
        {
            tasks.Clear();

            var stopAfter = false;

            long total = (long)Math.Pow(basic ? BasicCharCount : CharCount, length);
            long onePct = (long)(total / 100.0d);
            long oneTaskCount = total / TaskCount;
            long remaining = total;
            long totalCount = 0;

            Console.WriteLine($"Starting bruteforce for hash 0x{hash:X8} with length {length}...");

            // Create and start tasks
            for (var i = 0; i < TaskCount; ++i)
            {
                if (Builders[i] == null)
                    Builders[i] = new();

                var strVals = new int[length];
                var count = oneTaskCount;

                // Compensate for integer division, the last task takes all the remaining work
                if (i == TaskCount - 1)
                    count = remaining;

                // Setup the start values for the task
                if (totalCount > 0)
                    AddCount(strVals, totalCount, basic);

                var id = i;
                var localCount = count;
                var localStrVals = strVals;

                Console.WriteLine($"Task {id} from {totalCount,10} to {totalCount + localCount - 1,10} ({total,10}) ({CalcString(localStrVals, id, basic),10}) with count {localCount,10} looking for hash 0x{hash:X8}");

                // Start and store the task
                tasks.Add(Task.Factory.StartNew(() => Bruteforce(id, localStrVals, hash, localCount)));

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

                Thread.Sleep(1000);
            }

            Task.WaitAll(tasks.ToArray());

            ++length;

            if (stopAfter)
            {
                Console.ReadKey();
                break;
            }
        }
    }

    
    private static bool Bruteforce(int taskId, int[] strVals, uint hash, long itrCount, bool basic = false)
    {
        var stopAfter = false;
        var i = 0L;

        Progress[taskId] = 0;

        do
        {
            var str = CalcString(strVals, taskId, basic);

            if (InternalFNV32string(str) == hash)
            {
                Console.WriteLine($"Bruteforce: {str} => 0x{hash:X8}");
                stopAfter = true;
            }

            Progress[taskId] += 1;

            if (++i == itrCount)
                break;
        }
        while (IncString(strVals, basic));

        return stopAfter;
    }

    private static bool IncString(int[] values, bool basic)
    {
        for (var i = values.Length; i > 0; --i)
        {
            // return false if the first character would overflow
            if (i == 1 && values[i - 1] == (basic ? BasicCharCount : CharCount) - 1)
                return false;

            // increase the last character
            values[i - 1] += 1;

            // check if the character overflowed
            if (values[i - 1] < (basic ? BasicCharCount : CharCount))
            {
                return true;
            }

            // on overflow, set to zero and increase the next character
            values[i - 1] = 0;
        }

        return false;
    }

    private static void AddCount(int[] values, long count, bool basic)
    {
        for (var i = values.Length; i > 0; --i)
        {
            values[i - 1] = (int)(count % (basic ? BasicCharCount : CharCount));

            count /= (basic ? BasicCharCount : CharCount);
        }
    }

    private static string CalcString(int[] values, int taskId, bool basic)
    {
        Builders[taskId].Clear();

        for (var i = 0; i < values.Length; ++i)
            Builders[taskId].Append(Characters[values[i]]);

        return Builders[taskId].ToString();
    }

    private static long AlreadyDone(int[] values, bool basic)
    {
        long val = 0;
        int pow = 0;

        for (var i = values.Length; i > 0; --i)
        {
            val += (long)Math.Pow((basic ? BasicCharCount : CharCount), pow++) * values[i - 1];
        }

        return val;
    }
}
