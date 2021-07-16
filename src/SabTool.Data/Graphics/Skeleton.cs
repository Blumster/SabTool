using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Data.Graphics
{
    public class Skeleton
    {
        public Skeleton()
        {

        }

        public void LoadFrom(BinaryReader reader)
        {
            var int_0 = reader.ReadUInt32(); // some extra offset to skip
            var int_4 = reader.ReadUInt32();
            var int_8 = reader.ReadUInt32();
            var int_C = reader.ReadUInt32(); // some count
            var int_10 = reader.ReadUInt32();
            var int_14 = reader.ReadUInt32();
            var int_18 = reader.ReadUInt32();
            var int_1C = reader.ReadUInt32();
            var int_20 = reader.ReadUInt32();
            var int_24 = reader.ReadUInt32();

            reader.BaseStream.Position += 0x4;
        }
    }
}
