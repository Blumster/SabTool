using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SabTool.Serializers.Graphics.Shaders;

using SabTool.Data.Graphics.Shaders;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class ShaderMappingSerializer
{
    private static uint EndianSwap(uint value)
    {
        var bytes = BitConverter.GetBytes(value);

        Array.Reverse(bytes);

        return BitConverter.ToUInt32(bytes);
    }

    public static ShaderMapping DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("WSTO"))
            throw new Exception("Invalid ShaderMapping header found!");

        var mappings = new ShaderMapping();

        var count = EndianSwap(reader.ReadUInt32());
        for (var i = 0u; i < count; ++i)
        {
            var mapping = new ShaderMappingData
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
