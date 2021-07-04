using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Data.Graphics
{
    using Misc;

    public class WSModel
    {
        public WSModel()
        {

        }

        public void LoadFrom(BinaryReader reader)
        {
            reader.BaseStream.Position += 0x4C;

            var vector3 = new Vector3(reader);
        }
    }
}
