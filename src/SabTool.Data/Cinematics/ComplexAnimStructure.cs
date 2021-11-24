using System.Collections.Generic;

namespace SabTool.Data.Cinematics
{
    using ComplexAnimationElements;
    using Utils;

    public class ComplexAnimStructure
    {
        public Crc Crc { get; set; }
        public List<ComplexAnimElement> Elements { get; set; } = new();
    }
}
