using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Graphics;

using SabTool.Data.Graphics;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class ModelSerializer
{
    public static Model DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var currentStart = stream.Position;
        var model = new Model();

        stream.Position += 0x4C;

        model.Field4C = reader.ReadVector3();
        model.BoxAndRadius = reader.ReadVector4();
        model.Field68 = reader.ReadInt32();

        stream.Position += 0xC;

        model.Field78 = reader.ReadInt32();

        stream.Position += 0x18;

        model.Name = new Crc(reader.ReadUInt32());

        stream.Position += 0x18;

        model.FieldB0 = reader.ReadInt32();

        stream.Position += 0x5;

        model.FieldB9 = reader.ReadByte();

        stream.Position += 0x1;

        model.FieldBB = reader.ReadByte();

        stream.Position += 0x3;

        model.FieldBF = reader.ReadByte();

        if (currentStart + 0xC0 != stream.Position)
            throw new Exception($"Under or over read of the model part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0xC0}");

        return model;
    }

    public static void SerializeRaw(Model model, Stream stream)
    {

    }

    public static Model? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Model model, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(model, Formatting.Indented, new CrcConverter()));
    }
}
