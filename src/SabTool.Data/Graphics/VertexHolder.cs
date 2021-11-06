using System;
using System.Collections.Generic;
using System.IO;

namespace SabTool.Data.Graphics
{
    public class VertexHolder
    {
        public List<VertexDeclaration> Decl1 { get; set; }
        public List<VertexDeclaration> Decl2 { get; set; }

        public int[] Counts { get; } = new int[4];
        public int[] Formats { get; } = new int[4];
        public long[] UVFormats { get; } = new long[4];
        public int[] ArrayOffsets { get; } = new int[4];
        public int[] ArraySizes { get; } = new int[4];
        public byte[] Sizes { get; } = new byte[4];
        public int IndexArrayOffset { get; set; }
        public int IndexArraySize { get; set; }
        public uint SomeFlags { get; set; }
        public int ArrayCount { get; set; }
        public int IndexCount { get; set; }

        public bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;

            reader.BaseStream.Position += 0x18;

            for (var i = 0; i < 4; ++i)
                Counts[i] = reader.ReadInt32();

            for (var i = 0; i < 4; ++i)
                Formats[i] = reader.ReadInt32();

            for (var i = 0; i < 4; ++i)
                UVFormats[i] = reader.ReadInt64();

            for (var i = 0; i < 4; ++i)
                ArrayOffsets[i] = reader.ReadInt32();

            for (var i = 0; i < 4; ++i)
                ArraySizes[i] = reader.ReadInt32();

            for (var i = 0; i < 4; ++i)
                Sizes[i] = reader.ReadByte();

            reader.BaseStream.Position += 0x4;

            IndexArrayOffset = reader.ReadInt32();
            IndexArraySize = reader.ReadInt32();
            SomeFlags = reader.ReadUInt32();
            ArrayCount = reader.ReadInt32();
            IndexCount = reader.ReadInt32();

            reader.BaseStream.Position += 0x4;

            if (currentStart + 0x98 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the unk4 part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x98}");
                return false;
            }

            int flags = 0x21;
            var flagBytes = BitConverter.GetBytes(flags);

            Decl1 = VertexDeclaration.Build(flagBytes, Formats, ArrayCount, 0);
            Decl2 = VertexDeclaration.Build(flagBytes, Formats, ArrayCount, 1);

            return true;
        }
    }
}
