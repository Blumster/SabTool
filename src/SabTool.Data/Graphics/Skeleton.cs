using System;
using System.IO;
using System.Numerics;

using SharpGLTF.Transforms;

namespace SabTool.Data.Graphics
{
    using Utils.Extensions;

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

        public int NumBones { get; set; }
        public int SomeSize { get; set; }
        public int Int4 { get; set; }
        public int Int8 { get; set; }
        public int IntC { get; set; }
        public int SomeSize2 { get; set; }
        public int Int14 { get; set; }
        public int Int18 { get; set; }
        public int Int1C { get; set; }
        public int Int20 { get; set; }
        public int Int24 { get; set; }
        public byte[] UnkData { get; set; }
        public Matrix4x4[] BasePoses { get; set; }
        public AffineTransform[] UnkBasePoses { get; set; }
        public short[] Indices { get; set; }
        public Bone[] Bones { get; set; }

        public bool Read(BinaryReader reader)
        {
            SomeSize = reader.ReadInt32();
            Int4 = reader.ReadInt32();
            Int8 = reader.ReadInt32();
            IntC = reader.ReadInt32();
            SomeSize2 = reader.ReadInt32();
            Int14 = reader.ReadInt32();
            Int18 = reader.ReadInt32();
            Int1C = reader.ReadInt32();
            Int20 = reader.ReadInt32();
            Int24 = reader.ReadInt32();

            reader.BaseStream.Position += 0x4;

            // Should this be above or under the unk bytes?
            reader.BaseStream.Position += SomeSize;

            UnkData = reader.ReadBytes(NumBones);

            BasePoses = new Matrix4x4[NumBones];
            for (var i = 0; i < NumBones; ++i)
            {
                BasePoses[i] = reader.ReadMatrix4x4();
            }

            Bones = new Bone[NumBones];
            for (var i = 0; i < NumBones; ++i)
            {
                Bones[i] = new Bone();
                Bones[i].Read(reader);
                Bones[i].Index = (short)i;
            }

            UnkBasePoses = new AffineTransform[NumBones];
            for (var i = 0; i < NumBones; ++i)
                UnkBasePoses[i] = reader.ReadAffineTransform();

            Indices = new short[NumBones];
            for (var i = 0; i < NumBones; ++i)
                Indices[i] = reader.ReadInt16();

            reader.BaseStream.Position += SomeSize2;

            // Pointer array without data to skip allocation
            reader.BaseStream.Position += 4 * NumBones;

            return true;
        }
    }
}
