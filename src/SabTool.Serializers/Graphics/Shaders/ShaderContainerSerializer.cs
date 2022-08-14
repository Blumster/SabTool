using System;
using System.IO;
using System.Text;

namespace SabTool.Serializers.Graphics.Shaders;

using SabTool.Data.Graphics.Shaders;
using SabTool.Utils.Extensions;

public static class ShaderContainerSerializer
{
    public static ShaderContainer DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("SHDR", reversed: true))
            throw new Exception("Invalid ShaderContainer header found!");

        var container = new ShaderContainer();

        var numPixelShaders = reader.ReadUInt32();
        for (var i = 0; i < numPixelShaders; ++i)
        {
            if (!reader.CheckHeaderString("PSHD", reversed: true))
                throw new Exception("Invalid Pixel Shader header found!");

            container.PixelShaders.Add(ShaderSerializer.DeserializeRaw(stream, ShaderType.Pixel));
        }

        var numVertexShaders = reader.ReadUInt32();
        for (var i = 0; i < numVertexShaders; ++i)
        {
            if (!reader.CheckHeaderString("VSHD", reversed: true))
                throw new Exception("Invalid Pixel Shader header found!");

            container.VertexShaders.Add(ShaderSerializer.DeserializeRaw(stream, ShaderType.Vertex));
        }

        return container;
    }

    public static void SerialzieRaw(ShaderContainer shaderContainer, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        writer.WriteHeaderString("SHDR", reversed: true);

        writer.Write((uint)shaderContainer.PixelShaders.Count);
        foreach (var shader in shaderContainer.PixelShaders)
        {
            writer.WriteHeaderString("PSHD", reversed: true);

            ShaderSerializer.SerializeRaw(shader, stream);
        }

        writer.Write((uint)shaderContainer.VertexShaders.Count);
        foreach (var shader in shaderContainer.VertexShaders)
        {
            writer.WriteHeaderString("VSHD", reversed: true);

            ShaderSerializer.SerializeRaw(shader, stream);
        }
    }
}
