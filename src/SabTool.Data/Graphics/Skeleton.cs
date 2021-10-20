using System;
using System.IO;

namespace SabTool.Data.Graphics
{
    using Structures;

    public class Skeleton
    {
        #region Static
        public static Skeleton SingleBoneInstance { get; }

        static Skeleton()
        {
            SingleBoneInstance = new Skeleton
            {
                NumBones = 1
            };
        }
        #endregion

        public uint NumBones { get; set; }
        public Matrix[] BasePoses { get; set; }
        public Transform[] UnkBasePoses { get; set; }
        public short[] Indices { get; set; }
        public Bone[] Bones { get; set; }

        public bool Read(BinaryReader reader)
        {
            var someSkip = reader.ReadInt32();
            var int_4 = reader.ReadInt32();
            var int_8 = reader.ReadInt32();
            var someSkip2 = reader.ReadInt32();
            var int_10 = reader.ReadInt32();
            var int_14 = reader.ReadInt32();
            var int_18 = reader.ReadInt32();
            var int_1C = reader.ReadInt32();
            var int_20 = reader.ReadInt32();
            var int_24 = reader.ReadInt32();

            reader.BaseStream.Position += 0x4;

            reader.BaseStream.Position += someSkip;

            BasePoses = new Matrix[NumBones];
            for (var i = 0U; i < NumBones; ++i)
            {
                BasePoses[i] = new Matrix(reader);
            }

            for (var i = 0U; i < NumBones; ++i)
            {
                var currentStart = reader.BaseStream.Position;

                // TODO
                reader.BaseStream.Position += 0x40;

                /*_ = reader.ReadInt32();
                _ = reader.ReadInt32();

                reader.BaseStream.Position += 0xC;*/

                if (currentStart + 0x40 != reader.BaseStream.Position)
                {
                    Console.WriteLine($"Under or orver read of the Skeleton Unk1 part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x40}");
                    return false;
                }
            }

            UnkBasePoses = new Transform[NumBones];
            for (var i = 0U; i < NumBones; ++i)
                UnkBasePoses[i] = new Transform(reader);

            Indices = new short[NumBones];
            for (var i = 0U; i < NumBones; ++i)
                Indices[i] = reader.ReadInt16();

            reader.BaseStream.Position += someSkip2;

            // TODO

            return true;
        }
    }
}
