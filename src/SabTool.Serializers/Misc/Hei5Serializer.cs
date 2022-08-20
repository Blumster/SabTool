using System;
using System.IO;
using System.Text;

namespace SabTool.Serializers.Misc;

using Newtonsoft.Json;
using SabTool.Data;
using SabTool.Data.Misc;
using SabTool.Utils.Extensions;
using System.Collections.Generic;

public static class Hei5Serializer
{
    public static Hei5Container DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("HEI5", reversed: true))
            throw new Exception("Invalid HEI5 header found!");

        var count = reader.ReadInt32();

        var hei5Container = new Hei5Container
        {
            MaxGridX = reader.ReadInt32(),
            MaxGridZ = reader.ReadInt32(),
            GridSize = reader.ReadSingle(),
            TopLeftX = reader.ReadSingle(),
            TopLeftY = reader.ReadSingle(),
            TopLeftZ = reader.ReadSingle()
        };

        for (var i = 0; i < count; ++i)
        {
            if (!reader.CheckHeaderString("HEI1", reversed: true))
                throw new Exception("Invalid HEI1 header found!");

            var hei1 = new Hei1
            {
                Unk32 = reader.ReadInt32(),
                Unk36 = reader.ReadInt32(),
                Unk40 = reader.ReadSingle(),
                Unk44 = reader.ReadSingle()
            };

            hei1.Unk44Calc = (hei1.Unk44 - hei1.Unk40) / 255.0f;
            hei1.Unk48 = reader.ReadBytes(hei1.Unk32 * hei1.Unk36);
            hei1.StartPosX = reader.ReadSingle();
            hei1.StartPosY = reader.ReadSingle();
            hei1.StartPosZ = reader.ReadSingle();
            hei1.EndPosX = reader.ReadSingle();
            hei1.EndPosY = reader.ReadSingle();
            hei1.EndPosZ = reader.ReadSingle();

            hei5Container.Hei1s.Add(hei1);
        }

        if (stream.Position != stream.Length)
            throw new Exception("Under reading HEI5 file!");

        return hei5Container;
    }

    public static void SerializeRaw(Hei5Container hei5Container, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
    }

    public static Hei5Container? DeserialzieJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Hei5Container hei5Container, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(hei5Container, Formatting.Indented));
    }
}
