using System.Collections.Generic;
using System.Numerics;

namespace SabTool.Data.Lua
{
    public class WaterQuad
    {
        public string Name { get; set; }
        public int Slot { get; set; }
        public float VertexY { get; set; }
        public List<Vector2> VerticesXZ { get; set; } = new List<Vector2>(); // Always 4 entries
    }
}
