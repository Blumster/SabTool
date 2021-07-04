using System;
using System.IO;
using System.Text;

namespace SabTool.Data.GameTemplates
{
    using Data.Blueprint;
    using Utils.Extensions;

    public abstract class GameTemplatesBase
    {
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

                        var typeLen = blua.ReadInt(blua.Offset);

                        blua.Offset += 4;

                        var type = blua.ReadString(typeLen);

                        subReader.Size = templateSize + v7 - (blua.Offset + 4);
                        subReader.Data = blua.Data;
                        subReader.BaseOff = blua.Offset + 4;
                        subReader.Offset = 0;
                        subReader.Count = 0;

                        ReadBlueprint(type, name, subReader);

                        --subCount;
                    }

                    --blua.Count;
                }
                while (blua.Count != 0);
            }
        }

        protected abstract void ReadBlueprint(string type, string name, BluaReader reader);
    }
}
