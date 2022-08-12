using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Graphics;

using SabTool.Data.Graphics;

public static class Unk1Serializer
{
    public static Unk1 DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var unk1 = new Unk1();

        var currentStart = stream.Position;

        // TODO
        stream.Position += 0x44;

        if (currentStart + 0x44 != stream.Position)
            throw new Exception($"Under or orver read of the Unk3 part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x44}");

        return unk1;
    }

    public static void SerializeRaw(Unk1 unk1, Stream stream)
    {

    }

    public static Unk1? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Unk1 unk1, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(unk1, Formatting.Indented));
    }
}
