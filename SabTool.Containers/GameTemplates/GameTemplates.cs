using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabTool.Containers.GameTemplates
{
    using Utils.Extensions;

    public class GameTemplates
    {
        public int Unknown { get; set; }

        public void Read(Stream stream)
        {
            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (!br.CheckHeaderString("BLUA", reversed: true))
                    throw new Exception("Invalid header found!");

                var count = br.ReadInt32();
                
                for (var i = 0; i < count; ++i)
                {
                    var off = br.ReadInt32();

                    br.BaseStream.Position += 4;

                    var nextCount = br.ReadInt32();
                    for (var j = 0; j < nextCount; ++j) // TODO: 
                    {
                        var len1 = br.ReadInt32();
                        var str1 = br.ReadStringFromCharArray(len1);
                        var len2 = br.ReadInt32();
                        var str2 = br.ReadStringFromCharArray(len2);

                        Console.WriteLine($"Str1: {str1} | Str2: {str2}");
                    }
                }
            }
        }

        public void Write(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                
            }
        }
    }
}
