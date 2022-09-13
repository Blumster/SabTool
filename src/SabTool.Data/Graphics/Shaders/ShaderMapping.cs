namespace SabTool.Data.Graphics.Shaders;

public sealed class ShaderMapping
{
    public List<ShaderMappingData> Mappings { get; } = new();
}

public sealed class ShaderMappingData
{
    public Crc Unk { get; set; }
    public Crc Pass { get; set; }
    public Crc Unk2 { get; set; }
    public Crc Unk3 { get; set; }
}
