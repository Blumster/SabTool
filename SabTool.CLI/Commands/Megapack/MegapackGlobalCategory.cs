using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.CLI.Commands.Megapack
{
    using Base;

    public class MegapackGlobalCategory : BaseCategory
    {
        public override string Key => "global";

        public override string Usage => "<sub command>";

        public class UnpackCommand : BaseCommand
        {
            public override string Key { get; } = "unpack";
            public override string Usage { get; } = "<game base path> <output directory path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                return true;
            }
        }

        public class PackCommand : BaseCommand
        {
            public override string Key { get; } = "pack";
            public override string Usage { get; } = "<game base path> <input directory path>";

            public override bool Execute(IEnumerable<string> arguments)
            {
                return true;
            }
        }
    }
}
