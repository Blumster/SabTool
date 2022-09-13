namespace SabTool.Data.Packs;

using SabTool.Utils;

public sealed class EditNodes
{
    public List<EditNode> Nodes { get; } = new();
}

public sealed class EditNode
{
    public Crc Name { get; set; }
    public int Length { get; set; }
    public int Offset { get; set; }
    public byte[] Data { get; set; }
}
