namespace SabTool.Data.Graphics.Shaders;

public sealed class ShaderContainer
{
    public List<Shader> PixelShaders { get; } = new();
    public List<Shader> VertexShaders { get; } = new();
}
