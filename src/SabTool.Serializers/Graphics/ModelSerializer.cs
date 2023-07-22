using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Graphics;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Graphics;
public static class ModelSerializer
{
    public static Model DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        long currentStart = stream.Position;
        Model model = new();

        stream.Position += 0x4C;

        model.CullingOffset = reader.ReadVector3();
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

        return currentStart + 0xC0 != stream.Position
            ? throw new Exception($"Under or over read of the model part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0xC0}")
            : model;
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
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(model, Formatting.Indented, new CrcConverter()));
    }
}
