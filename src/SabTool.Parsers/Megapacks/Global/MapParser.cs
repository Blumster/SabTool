using System;
using System.Collections.Generic;
using System.IO;

namespace SabTool.Parsers.Megapacks.Global
{
    using Base;
    using Client.Streaming;

    public class MapParser : Parser
    {
        public override string Name { get; } = "Global Megapacks' map parser";

        public string FileName { get; set; }

        public MapParser(Stream stream, string fileName)
            : base(stream)
        {
            FileName = fileName;
        }

        public override bool Parse()
        {
            if (!WSStreamingManager.Instance.ReadMap1(Reader, FileName))
                return false;

            return true;
        }
    }
}
