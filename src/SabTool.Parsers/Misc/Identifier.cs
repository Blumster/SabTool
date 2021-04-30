using System;
using System.IO;

namespace SabTool.Parsers.Misc
{
    using Base;

    public class Identifier : Parser
    {
        public override string Name { get; } = "Identifier";

        public Identifier(Stream stream)
            : base(stream)
        {

        }

        static Identifier()
        {
            // TODO: collect every parser in this class library and save them to a dictionary
        }

        public override bool Parse()
        {
            // TODO: try every parser on the stream and see which can handle it to identify files
            throw new NotImplementedException();
        }
    }
}
