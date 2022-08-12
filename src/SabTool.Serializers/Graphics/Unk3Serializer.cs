using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Graphics;

using SabTool.Data.Graphics;

public static class Unk3Serializer
{
    public static Unk3 DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var unk3 = new Unk3();

        var currentStart = stream.Position;

        stream.Position += 0x4;

        unk3.UnkSize = reader.ReadInt32();
        unk3.NumUnk1 = reader.ReadInt32();
        unk3.NumUnk2 = reader.ReadInt32();
        unk3.NumUnk3 = reader.ReadInt32();

        stream.Position += unk3.UnkSize + 0x18;
        stream.Position += unk3.NumUnk3 * 0x30;
        stream.Position += unk3.NumUnk1 * 0xC;
        stream.Position += unk3.NumUnk2 * 0x10;

        var expectedSize = 0x18 + unk3.UnkSize + unk3.NumUnk1 * 0xC + unk3.NumUnk2 * 0x10 + unk3.NumUnk3 * 0x30;
        if (currentStart + expectedSize != stream.Position)
            throw new Exception($"Under or orver read of the Unk3 part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + expectedSize}");

        return unk3;
    }

    public static void SerializeRaw(Unk3 unk3, Stream stream)
    {

    }

    public static Unk3? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Unk3 unk3, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(unk3, Formatting.Indented));
    }
}
