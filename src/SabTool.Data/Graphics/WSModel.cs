using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Data.Graphics
{
    public class WSModel
    {
        public WSModel()
        {

        }

        public void LoadFrom(BinaryReader reader)
        {
            reader.BaseStream.Position += 0x4C;

            var vector3_4C = new Vector3(reader);
            var boxAndRadius = new Vector4(reader);
            var int_68 = reader.ReadUInt32();

            reader.BaseStream.Position += 0xC;

            var int_78 = reader.ReadUInt32();

            reader.BaseStream.Position += 0x18;

            var crc_94 = reader.ReadUInt32();

            reader.BaseStream.Position += 0x18;

            var int_B0 = reader.ReadUInt32();

            reader.BaseStream.Position += 0x5;

            var byte_B9 = reader.ReadByte();

            reader.BaseStream.Position += 0x1;

            var byte_BB = reader.ReadByte();

            reader.BaseStream.Position += 0x3;

            var byte_BF = reader.ReadByte();
        }
    }
}
