using System.Diagnostics;
using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Blueprints;
using SabTool.Serializers.Misc;
using SabTool.Utils;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Blueprints;
public static class BlueprintSerializer
{
    public static List<Blueprint> DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("BLUA", reversed: true))
            throw new Exception("Invalid Blueprint header found!");

        int blueprintCount = reader.ReadInt32();

        List<Blueprint> blueprints = new(blueprintCount);

        for (int i = 0; i < blueprintCount; ++i)
        {
            int bpSize = reader.ReadInt32();

            long bpStartPosition = reader.BaseStream.Position;

            int unknown = reader.ReadInt32();
            int innerCount = reader.ReadInt32();

            for (int j = 0; j < innerCount; ++j)
            {
                string name = reader.ReadStringWithMaxLength(reader.ReadInt32());
                string type = reader.ReadStringWithMaxLength(reader.ReadInt32());
                int propertyCount = reader.ReadInt32();

                List<Data.Misc.Property> properites = PropertySerializer.DeserializeMultipleRaw(reader, propertyCount);

                if (!Enum.IsDefined(typeof(BlueprintType), Hash.StringToHash(type)))
                    throw new Exception($"Unknown BlueprintType encountered: {type}!");

                blueprints.Add(new Blueprint((BlueprintType)Hash.StringToHash(type), name, properites));
            }

            if (bpStartPosition + bpSize != reader.BaseStream.Position)
                Debugger.Break();
        }

        return blueprints;
    }

    public static void SerializeRaw(List<Blueprint> blueprints, Stream stream)
    {
        using BinaryWriter writer = new(stream);
        writer.WriteHeaderString("BLUA", reversed: true);

        writer.Write(blueprints.Count);

        foreach (Blueprint blueprint in blueprints)
        {
            int templateSize = 0; // TODO

            writer.Write(templateSize);

            long bpStartPosition = writer.BaseStream.Position;

            writer.Write(0);
            writer.Write(blueprint.Properties.Count);

            foreach (Data.Misc.Property? prop in blueprint.Properties)
            {

            }
        }
    }

    public static List<Blueprint> DeserialzieJSON(Stream stream)
    {
        List<Blueprint> blueprints = new();

        return blueprints;
    }

    public static void SerializeJSON(List<Blueprint> blueprints, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(blueprints));
    }
}
