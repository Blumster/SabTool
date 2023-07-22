using System.Text;

using SabTool.Data.Graphics.Shaders;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Graphics.Shaders;
public static class ShaderMappingSerializer
{
    private static uint EndianSwap(uint value)
    {
        byte[] bytes = BitConverter.GetBytes(value);

        Array.Reverse(bytes);

        return BitConverter.ToUInt32(bytes);
    }

    public static ShaderMapping DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("WSTO"))
            throw new Exception("Invalid ShaderMapping header found!");

        ShaderMapping mappings = new();

        uint count = EndianSwap(reader.ReadUInt32());
        for (uint i = 0u; i < count; ++i)
        {
            ShaderMappingData mapping = new()
            {
                Unk = new(EndianSwap(reader.ReadUInt32())),
                Pass = new(EndianSwap(reader.ReadUInt32())),
                Unk2 = new(EndianSwap(reader.ReadUInt32())),
                Unk3 = new(EndianSwap(reader.ReadUInt32()))
            };

            mappings.Mappings.Add(mapping);
        }

        return mappings;
    }
}
