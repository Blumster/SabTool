using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabTool.Containers.GameTemplates
{
    using Client;
    using Client.Blueprint;
    using SabTool.Client.Streaming;
    using Utils;
    using Utils.Extensions;

    public class GameTemplates : GameTemplatesBase
    {
        public int Unknown { get; set; }

        protected override void ReadBlueprint(string type, string name, BluaReader reader)
        {
            WSBlueprint.Create(type, name, reader);
        }

        public void Write(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                
            }
        }
    }
}
