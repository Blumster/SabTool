using System.Text;

using SabTool.Data.Graphics.Shaders;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Graphics.Shaders;
public static class ShaderSerializer
{
    public static Shader DeserializeRaw(Stream stream, ShaderType type)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        Shader shader = new(type)
        {
            Index = reader.ReadUInt32()
        };

        if (reader.ReadUInt32() == 0)
            return shader;

        uint dataCount = reader.ReadUInt32();

        shader.Id = new(reader.ReadUInt32());

        if (dataCount == 0)
        {
            System.Diagnostics.Debugger.Break();
        }

        for (int i = 0; i < dataCount; ++i)
            shader.Data.Add(DeserializeShaderDataRaw(stream));

        return shader;
    }

    public static ShaderData DeserializeShaderDataRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        ShaderData shaderData = new()
        {
            Size = reader.ReadInt32()
        };
        shaderData.Data = reader.ReadBytes(shaderData.Size);

        int configParameterCount = reader.ReadInt32();
        for (int i = 0; i < configParameterCount; ++i)
            shaderData.Parameters.Add(DeserializeShaderConfigParameterRaw(stream));

        return shaderData;
    }

    public static ShaderConfigParameter DeserializeShaderConfigParameterRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        return new ShaderConfigParameter
        {
            Name = reader.ReadUTF8StringOn(reader.ReadInt32()),
            DefaultValue = reader.ReadInt32()
        };
    }

    public static void SerializeRaw(Shader shader, Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        writer.Write(shader.Index);
        writer.Write(shader.Data.Count != 0 ? 1 : 0);

        if (shader.Data.Count == 0)
            return;

        writer.Write(shader.Data.Count);
        writer.Write(shader.Id.Value);

        foreach (ShaderData? data in shader.Data)
            SerializeRaw(data, stream);
    }

    public static void SerializeRaw(ShaderData shaderData, Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        writer.Write(shaderData.Size);
        writer.Write(shaderData.Data);
        writer.Write(shaderData.Parameters.Count);

        foreach (ShaderConfigParameter? parameter in shaderData.Parameters)
            SerializeRaw(parameter, stream);
    }

    public static void SerializeRaw(ShaderConfigParameter shaderConfigParameter, Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        writer.Write(shaderConfigParameter.Name.Length + 1);
        writer.WriteUtf8StringOn(shaderConfigParameter.Name, shaderConfigParameter.Name.Length + 1);
    }
}
