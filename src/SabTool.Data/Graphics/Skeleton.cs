using System.Numerics;

using SharpGLTF.Transforms;

namespace SabTool.Data.Graphics
{
    public class Skeleton
    {
        public static Skeleton SingleBoneInstance { get; } = new Skeleton
        {
            NumBones = 1,
            Indices = new short[] { -1 },
            Bones = new Bone[1] { Bone.DefaultBone },
            UnkBasePoses = new AffineTransform[1] { AffineTransform.Identity }
        };

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
        public Matrix4x4[] BasePoses { get; set; }
        public AffineTransform[] UnkBasePoses { get; set; }
        public short[] Indices { get; set; }
        public Bone[] Bones { get; set; }
    }
}
