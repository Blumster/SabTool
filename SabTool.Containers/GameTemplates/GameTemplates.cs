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

    public class GameTemplates
    {
        public int Unknown { get; set; }

        public void Read(Stream stream)
        {
            var blua = new BluaReader();
            var subReader = new BluaReader();

            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (!br.CheckHeaderString("BLUA", reversed: true))
                    throw new Exception("Invalid header found!");

                blua.Size = (int)br.BaseStream.Length;
                blua.Data = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
            }

            blua.Count = blua.ReadInt();

            if (blua.Count > 0)
            {
                var off = blua.Offset;

                do
                {
                    var templateSize = blua.ReadInt(off);
                    var v7 = off + 4;
                    var subCountOff = off + 8;
                    var subCount = blua.ReadInt(subCountOff);

                    for (off = subCountOff + 4; subCount != 0; off = templateSize + v7)
                    {
                        var nameLen = blua.ReadInt(off);

                        blua.Offset = off + 4;

                        var name = blua.ReadString(nameLen);

                        var categoryLen = blua.ReadInt(blua.Offset);

                        blua.Offset += 4;

                        var category = blua.ReadString(categoryLen);

                        subReader.Size = templateSize + v7 - (blua.Offset + 4);
                        subReader.Data = blua.Data;
                        subReader.BaseOff = blua.Offset + 4;
                        subReader.Offset = 0;
                        subReader.Count = 0;

                        WSBlueprint.Create(category, name, subReader);

                        --subCount;
                    }

                    --blua.Count;
                }
                while (blua.Count != 0);
            }

            /*using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (!br.CheckHeaderString("BLUA", reversed: true))
                    throw new Exception("Invalid header found!");

                var count = br.ReadInt32();
                
                for (var i = 0; i < count; ++i)
                {
                    var off = br.ReadUInt32();
                    var off2 = br.ReadUInt32();
                    var innerCount = br.ReadInt32();

                    for (var j = 0; j < innerCount; ++j) // TODO: 
                    {
                        var len1 = br.ReadInt32();
                        var str1 = br.ReadStringFromCharArray(len1);
                        var len2 = br.ReadInt32();
                        var str2 = br.ReadStringFromCharArray(len2);

                        Console.WriteLine($"Str1: {str1} | Str2: {str2}");

                        SubRead(str2, str1, br);
                    }
                }
            }*/
        }

        public void Write(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                
            }
        }
    }
}
