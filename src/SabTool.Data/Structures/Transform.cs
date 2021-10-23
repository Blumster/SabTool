using System.IO;

namespace SabTool.Data.Structures
{
    public class Transform
    {
        public Vector3 Translation { get; set; }
        public Vector4 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Transform(BinaryReader reader)
        {
            Translation = new Vector3(reader);

            reader.BaseStream.Position += 4;

            Rotation = new Vector4(reader);
            Scale = new Vector3(reader);

            reader.BaseStream.Position += 4;
        }

        public override string ToString()
        {
            return $"Transform({Translation}, {Rotation}, {Scale})";
        }
    }
}
