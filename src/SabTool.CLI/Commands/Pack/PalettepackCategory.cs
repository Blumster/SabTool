using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.CLI.Commands.Pack
{
    using Base;

    public class PalettepackCategory : BaseCategory
    {
        public override string Key => "palettepack";
        public override string Shortcut => "p";
        public override string Usage => "<sub command name>";

        public class UnpackCommand : BaseCommand
        {
            public override string Key { get; } = "unpack";
            public override string Shortcut { get; } = "u";
            public override string Usage { get; } = "<palettepack file path> [output directory path]";

            public override bool Execute(IEnumerable<string> arguments)
            {
                return true;
            }
        }
    }
}
