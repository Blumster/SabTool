using System.IO;
using System.Text;

namespace SabTool.Serializers.Graphics.Shaders;

using SabTool.Data.Graphics.Shaders;
using SabTool.Utils.Extensions;

public static class ShaderSerializer
{
    public static Shader DeserializeRaw(Stream stream, ShaderType type)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var shader = new Shader(type)
        {
            Index = reader.ReadUInt32()
        };

        if (reader.ReadUInt32() == 0)
            return shader;

        var dataCount = reader.ReadUInt32();

        shader.Id = new(reader.ReadUInt32());

        if (dataCount == 0)
        {
            System.Diagnostics.Debugger.Break();
        }

        for (var i = 0; i < dataCount; ++i)
            shader.Data.Add(DeserializeShaderDataRaw(stream));

        return shader;
    }

    public static ShaderData DeserializeShaderDataRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var shaderData = new ShaderData
        {
            Size = reader.ReadInt32()
        };
        shaderData.Data = reader.ReadBytes(shaderData.Size);

        var configParameterCount = reader.ReadInt32();
        for (var i = 0; i < configParameterCount; ++i)
            shaderData.Parameters.Add(DeserializeShaderConfigParameterRaw(stream));

        return shaderData;
    }

    public static ShaderConfigParameter DeserializeShaderConfigParameterRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        return new ShaderConfigParameter
        {
            Name = reader.ReadUTF8StringOn(reader.ReadInt32()),
            DefaultValue = reader.ReadInt32()
        };
    }

    public static void SerializeRaw(Shader shader, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        writer.Write(shader.Index);
        writer.Write(shader.Data.Count != 0 ? 1 : 0);

        if (shader.Data.Count == 0)
            return;

        writer.Write(shader.Data.Count);
        writer.Write(shader.Id.Value);

        foreach (var data in shader.Data)
            SerializeRaw(data, stream);
    }

    public static void SerializeRaw(ShaderData shaderData, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        writer.Write(shaderData.Size);
        writer.Write(shaderData.Data);
        writer.Write(shaderData.Parameters.Count);

        foreach (var parameter in shaderData.Parameters)
            SerializeRaw(parameter, stream);
    }

    public static void SerializeRaw(ShaderConfigParameter shaderConfigParameter, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        writer.Write(shaderConfigParameter.Name.Length + 1);
        writer.WriteUtf8StringOn(shaderConfigParameter.Name, shaderConfigParameter.Name.Length + 1);
    }
}
