using System.Numerics;

namespace SabTool.Data.Cinematics.CinematicElements;

public class CinemaXSICamera : CinemaElement
{
    public struct Sub
    {
        public Matrix4x4 Matrix { get; set; }
        public float FOV { get; set; }
        public float F68 { get; set; }
        public float F72 { get; set; }
        public float F76 { get; set; }
        public float F80 { get; set; }
        public float F84 { get; set; }
    };

    public float UnkFloat1 { get; set; }
    public float EndTime { get; set; }
    public Sub[] Subs { get; set; }
    public int UnkInt { get; set; }
}
