namespace SabTool.Data.Packs;

using SabTool.Utils;

public class EditNodes
{
    public List<EditNode> Nodes { get; } = new();
}

public class EditNode
{
    public Crc Name { get; set; }
    public int Length { get; set; }
    public int Offset { get; set; }
    public byte[] Data { get; set; }
}
