namespace SabTool.Data.Packs;

using SabTool.Data.Entities;

public sealed class EditNodes
{
    public List<EditNode> Nodes { get; } = new();
}

public sealed class EditNode
{
    public Crc Name { get; set; }
    public Dictionary<Crc, EditNodeObject> Objects { get; } = new();
}

public sealed class EditNodeObject
{
    public Crc Name { get; set; }
    public EntityDesc Entity { get; set; }
}

public sealed class AIFencePathNode
{
    public Vector4 Offset { get; set; }
    public uint Flags { get; set; }
}

public sealed class AIFencePost
{
    public Vector4 FenceNodeOffset { get; set; }
    public uint NodeFlags { get; set; }
    public uint WallFlags { get; set; }
}
