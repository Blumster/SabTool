using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.CLI.Commands.GameTemplates
{
    using Base;

    public class GameTemplatesDLCCategory : BaseCategory
    {
        public override string Key => "dlc";

        public override string Usage => "<sub command>";

        public class UnpackCommand : BaseCommand
        {
            public override string Key { get; } = "unpack";
            public override string Usage { get; } = "<game base path> <output directory>";

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
