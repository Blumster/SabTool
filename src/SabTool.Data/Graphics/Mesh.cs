using System;
using System.IO;

namespace SabTool.Data.Graphics
{
    public class Mesh
    {
        public Skeleton Skeleton { get; set; }
        public Segment[] Segments { get; set; }
        public Primitive[] Primitives { get; set; }
        public VertexHolder[] VertexHolders { get; set; }
        public Unk1[] Unk1s { get; set; }
        public Unk3[] Unk3s { get; set; }
        public int NumBones { get; set; }
        public int NumUnk1 { get; set; }
        public int Field14 { get; set; }
        public short NumVertexHolder { get; set; }
        public short NumPrimitives { get; set; }
        public short NumUnk3 { get; set; }
        public short Field1E { get; set; }
        public int Field20 { get; set; }
        public int Field24 { get; set; }
        public short NumSegments { get; set; }
        public byte Field2A { get; set; }
        public byte Field2B { get; set; }
        public int Field2C { get; set; }
        public byte Field30 { get; set; }
        public byte Field31 { get; set; }
        public byte Field32 { get; set; }
        public byte Field33 { get; set; }

        public bool Read(BinaryReader reader)
        {
            var currentStart = reader.BaseStream.Position;

            reader.BaseStream.Position += 0xC;

            NumBones = reader.ReadInt32();
            NumUnk1 = reader.ReadInt32();
            Field14 = reader.ReadInt32();
            NumVertexHolder = reader.ReadInt16();
            NumPrimitives = reader.ReadInt16();
            NumUnk3 = reader.ReadInt16();
            Field1E = reader.ReadInt16();
            Field20 = reader.ReadInt32();
            Field24 = reader.ReadInt32();
            NumSegments = reader.ReadInt16();
            Field2A = reader.ReadByte();
            Field2B = reader.ReadByte();

            if (currentStart + 0x2C != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the base Mesh part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x2C}");
                return false;
            }

            Primitives = new Primitive[NumPrimitives];
            Segments = new Segment[NumSegments];
            VertexHolders = new VertexHolder[NumVertexHolder];
            Unk1s = new Unk1[NumUnk1];
            Unk3s = new Unk3[NumUnk3];

            currentStart = reader.BaseStream.Position;

            Field2C = reader.ReadInt32();
            Field30 = reader.ReadByte();
            Field31 = reader.ReadByte();
            Field32 = reader.ReadByte();
            Field33 = reader.ReadByte();

            if (currentStart + 0x8 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the Mesh part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x8}");
                return false;
            }

            if (NumBones <= 1)
            {
                Skeleton = Skeleton.SingleBoneInstance;
            }
            else
            {
                Skeleton = new Skeleton
                {
                    NumBones = NumBones
                };

                if (!Skeleton.Read(reader))
                    return false;
            }

            if (NumUnk1 > 0)
            {
                // TODO
                reader.BaseStream.Position += 0x8;

                for (var i = 0; i < NumUnk1; ++i)
                {
                    Unk1s[i] = new Unk1();
                    Unk1s[i].Read(reader);
                }
            }

            for (var i = 0; i < NumUnk3; ++i)
            {
                Unk3s[i] = new Unk3();
                Unk3s[i].Read(reader);
            }

            for (var i = 0; i < NumVertexHolder; ++i)
            {
                VertexHolders[i] = new VertexHolder();
                VertexHolders[i].Read(reader);
            }

            for (var i = 0; i < NumPrimitives; ++i)
            {
                Primitives[i] = new Primitive(this);
                Primitives[i].Read(reader);
            }

            for (var i = 0; i < NumSegments; ++i)
            {
                Segments[i] = new Segment(this);
                Segments[i].Read(reader);
            }

            return true;
        }
    }
}
