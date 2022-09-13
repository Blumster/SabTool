namespace SabTool.Data.Graphics.Shaders;

using SabTool.Utils;

public enum ShaderType
{
    Pixel,
    Vertex
}

public sealed class Shader
{
    public ShaderType Type { get; }
    public uint Index { get; set; }
    public Crc Id { get; set; }
    public List<ShaderData> Data { get; } = new();

    public Shader(ShaderType type)
    {
        Type = type;
    }
}

public sealed class ShaderData
{
    public int Size { get; set; }
    public byte[] Data { get; set; }
    public List<ShaderConfigParameter> Parameters { get; } = new();
}

public sealed class ShaderConfigParameter
{
    public string Name { get; set; }
    public int DefaultValue { get; set; }
}
