using System.IO;
using System.Numerics;

namespace SabTool.Data.Structures
{
    using Utils.Extensions;

    public class Transform
    {
        public Vector3 Translation { get; set; }
        public Vector4 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Transform(BinaryReader reader)
        {
            Translation = reader.ReadVector3();

            reader.BaseStream.Position += 4;

            Rotation = reader.ReadVector4();
            Scale = reader.ReadVector3();

            reader.BaseStream.Position += 4;
        }

        public override string ToString()
        {
            return $"Transform({Translation}, {Rotation}, {Scale})";
        }
    }
}
