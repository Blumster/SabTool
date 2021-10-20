using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Data.Graphics
{
    public class Bone
    {
        public bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;


            if (currentStart + 0x0 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the Bone part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x0}");
                return false;
            }

            return true;
        }
    }
}
