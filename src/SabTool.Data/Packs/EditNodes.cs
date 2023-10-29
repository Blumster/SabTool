using SabTool.Data.Blueprints;

namespace SabTool.Data.Packs;

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
    public int Id { get; set; }
    public Crc Name { get; set; }
    public Crc? ClassName { get; set; }
    public Crc? Type { get; set; }
    public Crc? CollisionModule {  get; set; }
    public float? Height { get; set; }
    public string? Script { get; set; }
    public string? ParentName { get; set; }
    public Matrix4x4 WorldMatrix = new(); // field on purpose
    public AIFencePathNode[] PathNodes { get; set; }
    public AIFencePost[] Posts { get; set; }
    public List<(Crc, object)> InstanceData { get; } = new();
}

public sealed class EditNodeInstanceProperty
{
    public BlueprintType BlueprintType { get; set; }
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
