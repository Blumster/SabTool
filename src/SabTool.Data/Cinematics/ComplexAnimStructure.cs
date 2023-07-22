
using SabTool.Data.Cinematics.ComplexAnimationElements;

namespace SabTool.Data.Cinematics;
public sealed class ComplexAnimStructure
{
    public Crc Crc { get; set; }
    public List<ComplexAnimElement> Elements { get; set; } = new();
}
