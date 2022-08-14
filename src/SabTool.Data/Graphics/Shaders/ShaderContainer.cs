using System.Collections.Generic;

namespace SabTool.Data.Graphics.Shaders;

public class ShaderContainer
{
    public List<Shader> PixelShaders { get; } = new();
    public List<Shader> VertexShaders { get; } = new();
}
