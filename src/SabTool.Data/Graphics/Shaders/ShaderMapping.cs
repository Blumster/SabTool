using System.Collections.Generic;

namespace SabTool.Data.Graphics.Shaders;

using SabTool.Utils;

public class ShaderMapping
{
    public List<ShaderMappingData> Mappings { get; } = new();
}

public class ShaderMappingData
{
    public Crc Unk { get; set; }
    public Crc Pass { get; set; }
    public Crc Unk2 { get; set; }
    public Crc Unk3 { get; set; }
}
