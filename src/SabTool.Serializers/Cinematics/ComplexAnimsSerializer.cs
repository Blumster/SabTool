using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Cinematics;
using SabTool.Data.Cinematics.ComplexAnimationElements;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Cinematics;
public static class ComplexAnimsSerializer
{
    private const int Version = 9;

    public static List<ComplexAnimStructure> DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        short version = reader.ReadInt16();
        if (version != Version)
            throw new Exception($"Invalid version {version} for ComplexAnimations!");

        List<ComplexAnimStructure> structures = new();

        while (stream.Position < stream.Length)
        {
            structures.Add(DeserializeCXA2(reader));

            if (stream.Position == stream.Length - 4 && !reader.CheckHeaderString("CEND", reversed: true))
                throw new Exception("Invalid end for ComplexAnimations!");
        }

        return structures;
    }

    private static ComplexAnimStructure DeserializeCXA2(BinaryReader reader)
    {
        if (!reader.CheckHeaderString("CXA2", reversed: true))
            throw new Exception("Invalid magic found for ComplexAnimStructure!");

        ComplexAnimStructure structure = new()
        {
            Crc = new Crc(reader.ReadUInt32())
        };

        while (true)
        {
            string magic = reader.ReadHeaderString(4, reversed: true);
            if (magic == "CEND")
            {
                break;
            }

            if (magic != "ELEM")
                throw new Exception("Invalid element tag found for ComplexAnimStructure!");

            Crc typeCrc = new(reader.ReadUInt32());

            ComplexAnimElement element = typeCrc.Value switch
            {
                0xE1DACE7E => new ComplexAnimElement(ElementType.Say), // Hash("Say")
                0xDD62BA1A => new SoundElement(), // Hash("Sound")
                0xB097CD21 => new ComplexAnimElement(ElementType.Listener), // Hash("Listener")
                0xC023ACD3 or 0x4FD7B0DD => new ExpressionElement(), // Hash("Expression") or Hash("<UNKNOWN>")
                0x91FAB51F => new ComplexAnimElement(ElementType.Unk7), // Hash("<UNKNOWN>")
                0x7B794547 => new AnimTargetElement(), // Hash("Animation_Target")
                0x180A0045 => new TargetElement(), // Hash("Lookat_Target")
                0x18166555 => new AnimElement(), // Hash("Animation")
                _ => new ComplexAnimElement(ElementType.Unk8)
            };

            structure.Elements.Add(element);

            element.UnkCrc = new Crc(reader.ReadUInt32());
            element.Id = new Crc(reader.ReadUInt32());
            element.UnkInt = reader.ReadInt32();
            element.UnkF1 = reader.ReadSingle();
            element.UnkF2 = reader.ReadSingle();

            switch (element)
            {
                case AnimTargetElement animTargetElement:
                    animTargetElement.UnkBool1 = reader.ReadInt16() != 0;
                    animTargetElement.UnkInt2 = reader.ReadInt32();
                    animTargetElement.UnkBool2 = reader.ReadInt16() != 0;
                    break;

                case AnimElement animElement:
                    animElement.UnkBool1 = reader.ReadInt16() != 0;
                    animElement.UnkBool2 = reader.ReadInt16() != 0;
                    animElement.UnkFloat = reader.ReadSingle();
                    break;

                case SoundElement soundElement:
                    soundElement.UnkBool = reader.ReadInt16() != 0;
                    break;

                case TargetElement targetElement:
                    targetElement.UnkBool = reader.ReadInt16() != 0;
                    break;

                case ExpressionElement expressionElement:
                    expressionElement.UnkFloat = reader.ReadSingle();
                    expressionElement.UnkInt2 = reader.ReadInt32();
                    break;
            }
        }

        return structure;
    }

    public static object? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(List<ComplexAnimStructure> structures, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(structures, Formatting.Indented, new CrcConverter()));
    }
}
