using System;
using System.IO;

namespace SabTool.Data.Graphics
{
    public class Unk1
    {
        public bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;

            // TODO
            reader.BaseStream.Position += 0x44;

            if (currentStart + 0x44 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the Unk3 part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x44}");
                return false;
            }

            return true;
        }
    }
}
