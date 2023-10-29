namespace SabTool.Data.Entities;

using SabTool.Data.Packs;

public class EntityDesc
{
    public int Id { get; set; }
    public Crc Name { get; set; }
    public Matrix4x4 WorldMatrix { get; set; }
    public AIFenceDesc FenceDesc { get; set; }
}

public class AIFenceDesc
{
    public AIFencePost[] Posts { get; set; }
    public AIFencePathNode[] PathNodes { get; set; }
    public byte Flags { get; set; }
}
