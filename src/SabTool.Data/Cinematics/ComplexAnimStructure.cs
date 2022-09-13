namespace SabTool.Data.Cinematics;

using SabTool.Data.Cinematics.ComplexAnimationElements;
using SabTool.Utils;

public sealed class ComplexAnimStructure
{
    public Crc Crc { get; set; }
    public List<ComplexAnimElement> Elements { get; set; } = new();
}
