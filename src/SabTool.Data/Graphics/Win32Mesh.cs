using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Data.Graphics
{
    public class Win32Mesh
    {
        public Win32Mesh()
        {

        }

        public void LoadFrom(BinaryReader reader)
        {
            reader.BaseStream.Position += 0xC;

            var int_C = reader.ReadUInt32(); // numBones?
            var int_10 = reader.ReadUInt32(); // numUnk1
            var int_14 = reader.ReadUInt32();
            var short_18 = reader.ReadUInt16(); // numUnk2
            var short_1A = reader.ReadUInt16(); // numUnk4
            var short_1C = reader.ReadUInt16(); // numUnk3
            var short_1E = reader.ReadUInt16();
            var int_20 = reader.ReadUInt32();
            var int_24 = reader.ReadUInt32();
            var short_28 = reader.ReadUInt16(); // numUnk5
            var byte_2A = reader.ReadByte();
            var byte_2B = reader.ReadByte();
            var int_2C = reader.ReadUInt32();
            var byte_30 = reader.ReadByte();
            var byte_31 = reader.ReadByte();
            var byte_32 = reader.ReadByte();
            var byte_33 = reader.ReadByte();
        }
    }
}
