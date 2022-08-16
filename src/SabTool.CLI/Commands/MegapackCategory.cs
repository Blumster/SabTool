using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SabTool.CLI.Commands
{
    using Base;
    using Depot;

    public class MegapackCategory : BaseCategory
    {
        public override string Key => "megapack";

        public override string Shortcut => "m";

        public override string Usage => "<sub command>";

        public class ListCommand : BaseCommand
        {
            public override string Key { get; } = "list";
            public override string Shortcut { get; } = "l";
            public override string Usage { get; } = "<game base path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 1)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
                ResourceDepot.Instance.Load(Resource.Megapacks);

                var writer = Console.Out;

                if (arguments.Count() == 2)
                {
                    writer = new StreamWriter(arguments.ElementAt(1), false, Encoding.UTF8);
                }

                //var found = false;

                foreach (var megapackFile in ResourceDepot.Instance.GetMegapacks())
                {
                    writer.WriteLine($"Megapack: {megapackFile}");

                    foreach (var fileEntry in ResourceDepot.Instance.GetFileEntries(megapackFile))
                    {
                        /*if (fileEntry.Name.Value < 0x80000000u)
                        {
                            var strValue = fileEntry.Name.Value.ToString();

                            if (strValue.Length >= 2)
                            {
                                var pathString = $@"France\{strValue[..2]}\{strValue}.pack";
                                var pathHash = Hash.FNV32string(pathString, addToLookup: false);
                                var existing = Hash.HashToString(pathHash);

                                if (pathHash == fileEntry.Path.Value)
                                {
                                    if (string.IsNullOrEmpty(existing))
                                    {
                                        Hash.StringToHash(pathString);

                                        found = true;
                                    }
                                    else if (existing.ToLowerInvariant() != pathString.ToLowerInvariant())
                                    {
                                        Console.WriteLine($"Hash 0x{pathHash:X8} collision for \"{existing.ToLowerInvariant()}\" and \"{pathString.ToLowerInvariant()}\"!");
                                    }
                                }
                            }
                            else
                            {
                                var pathString = $@"France\{strValue}.pack";
                                var pathHash = Hash.FNV32string(pathString, addToLookup: false);
                                var existing = Hash.HashToString(pathHash);

                                if (pathHash == fileEntry.Path.Value)
                                {
                                    if (string.IsNullOrEmpty(existing))
                                    {
                                        Hash.StringToHash(pathString);

                                        found = true;
                                    }
                                    else if (existing.ToLowerInvariant() != pathString.ToLowerInvariant())
                                    {
                                        Console.WriteLine($"Hash 0x{pathHash:X8} collision for \"{existing.ToLowerInvariant()}\" and \"{pathString.ToLowerInvariant()}\"!");
                                    }
                                }
                            }
                        }*/
                        writer.WriteLine(fileEntry.ToPrettyString());
                    }

                    writer.WriteLine();
                }
                
                writer.Flush();

                if (writer != Console.Out)
                {
                    writer.Close();
                }

                /*if (found)
                {
                    Hash.Save();
                    Hash.SaveMissing();
                }*/

                return true;
            }
        }

        public class UnpackCommand : BaseCommand
        {
            public override string Key { get; } = "unpack";
            public override string Shortcut { get; } = "u";
            public override string Usage { get; } = "<game base path> <output directory> [relative megapack file path]";

            public override bool Execute(IEnumerable<string> arguments)
            {
                if (arguments.Count() < 2)
                {
                    Console.WriteLine("ERROR: Not enough arguments given!");
                    return false;
                }

                ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
                ResourceDepot.Instance.Load(Resource.Megapacks);

                var outputDir = arguments.ElementAt(1);

                Directory.CreateDirectory(outputDir);

                string fileToUnpack = null;

                if (arguments.Count() == 3)
                    fileToUnpack = arguments.ElementAt(2);

                foreach (var megapackFile in ResourceDepot.Instance.GetMegapacks())
                {
                    if (!string.IsNullOrEmpty(fileToUnpack) && megapackFile != fileToUnpack)
                        continue;

                    Console.WriteLine($"Unpacking {megapackFile}...");
                    
                    var megapackOutputDir = Path.Combine(outputDir, megapackFile);

                    var i = 0;

                    foreach (var fileEntry in ResourceDepot.Instance.GetFileEntries(megapackFile))
                    {
                        var entryStream = ResourceDepot.Instance.GetPackStream(fileEntry.Path);
                        if (entryStream == null)
                        {
                            Console.WriteLine($"Unable to unpack {fileEntry.Path}, no data fround for it!");
                            continue;
                        }

                        var realFilePath = fileEntry.Path.GetString();
                        if (string.IsNullOrEmpty(realFilePath))
                            realFilePath = $"{fileEntry.Name.GetStringOrHexString()}.pack"; // TODO: based on the streamblock, can be dynpack, palettepack or pack

                        var outputFilePath = Path.Combine(megapackOutputDir, realFilePath);

                        var outputFileDir = Path.GetDirectoryName(outputFilePath);
                        Directory.CreateDirectory(outputFileDir);

                        using var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                        entryStream.CopyTo(fs);

                        ++i;
                    }

                    Console.WriteLine($"Successfully unpacked {megapackFile} to {megapackOutputDir} ({i} files)!");
                }

                return true;
            }
        }

        public class PackCommand : BaseCommand
        {
            public override string Key { get; } = "pack";
            public override string Shortcut { get; } = "p";
            public override string Usage { get; } = "<game base path> <input directory path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                return true;
            }
        }
    }
}
