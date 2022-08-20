using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;
using SabTool.Depot;
using SabTool.Serializers.Misc;

public class MiscCategory : BaseCategory
{
    public override string Key => "misc";
    public override string Shortcut => "mi";
    public override string Usage => "<sub command name>";

    public class UnpackHei5Command : BaseCommand
    {
        public override string Key { get; } = "unpack-hei5";
        public override string Shortcut { get; } = "uh5";
        public override string Usage { get; } = "<game base path> <output directory path>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            ResourceDepot.Instance.Initialize(arguments.ElementAt(0));
            ResourceDepot.Instance.Load(Resource.Misc);

            var outputFolder = arguments.ElementAt(1);

            Directory.CreateDirectory(outputFolder);

            using var outFile = new FileStream(Path.Combine(outputFolder, "hei5.json"), FileMode.Create, FileAccess.Write, FileShare.None);

            Hei5Serializer.SerializeJSON(ResourceDepot.Instance.Hei5Container!, outFile);

            return true;
        }
    }
}
