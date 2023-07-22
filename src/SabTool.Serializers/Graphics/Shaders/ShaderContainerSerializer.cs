using System.Text;

using SabTool.Data.Graphics.Shaders;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Graphics.Shaders;
public static class ShaderContainerSerializer
{
    public static ShaderContainer DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("SHDR", reversed: true))
            throw new Exception("Invalid ShaderContainer header found!");

        ShaderContainer container = new();

        uint numPixelShaders = reader.ReadUInt32();
        for (int i = 0; i < numPixelShaders; ++i)
        {
            if (!reader.CheckHeaderString("PSHD", reversed: true))
                throw new Exception("Invalid Pixel Shader header found!");

            container.PixelShaders.Add(ShaderSerializer.DeserializeRaw(stream, ShaderType.Pixel));
        }

        uint numVertexShaders = reader.ReadUInt32();
        for (int i = 0; i < numVertexShaders; ++i)
        {
            if (!reader.CheckHeaderString("VSHD", reversed: true))
                throw new Exception("Invalid Pixel Shader header found!");

            container.VertexShaders.Add(ShaderSerializer.DeserializeRaw(stream, ShaderType.Vertex));
        }

        return container;
    }

    public static void SerialzieRaw(ShaderContainer shaderContainer, Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        writer.WriteHeaderString("SHDR", reversed: true);

        writer.Write((uint)shaderContainer.PixelShaders.Count);
        foreach (Shader? shader in shaderContainer.PixelShaders)
        {
            writer.WriteHeaderString("PSHD", reversed: true);

            ShaderSerializer.SerializeRaw(shader, stream);
        }

        writer.Write((uint)shaderContainer.VertexShaders.Count);
        foreach (Shader? shader in shaderContainer.VertexShaders)
        {
            writer.WriteHeaderString("VSHD", reversed: true);

            ShaderSerializer.SerializeRaw(shader, stream);
        }
    }
}
