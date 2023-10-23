using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Blueprints;

using SabTool.Data.Blueprints;
using SabTool.Serializers.Misc;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class BlueprintSerializer
{
    public static List<Blueprint> DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("BLUA", reversed: true))
            throw new Exception("Invalid Blueprint header found!");

        var blueprintCount = reader.ReadInt32();

        var blueprints = new List<Blueprint>(blueprintCount);

        for (var i = 0; i < blueprintCount; ++i)
        {
            var bpSize = reader.ReadInt32();

            var bpStartPosition = reader.BaseStream.Position;

            var unknown = reader.ReadInt32();
            var innerCount = reader.ReadInt32();

            for (var j = 0; j < innerCount; ++j)
            {
                var name = reader.ReadStringWithMaxLength(reader.ReadInt32());
                var type = reader.ReadStringWithMaxLength(reader.ReadInt32());
                var propertyCount = reader.ReadInt32();

                var properites = PropertySerializer.DeserializeMultipleRaw(reader, propertyCount);

                if (!Enum.IsDefined(typeof(BlueprintType), Hash.StringToHash(type)))
                    throw new Exception($"Unknown BlueprintType encountered: {type}!");

                blueprints.Add(new Blueprint((BlueprintType)Hash.StringToHash(type), name, properites));

                // Store hashes
                Hash.FNV32string(name);
                Hash.StringToHash(name);
            }

            if (bpStartPosition + bpSize != reader.BaseStream.Position)
                Debugger.Break();
        }

        return blueprints;
    }

    public static void SerializeRaw(List<Blueprint> blueprints, Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        writer.WriteHeaderString("BLUA", reversed: true);

        writer.Write(blueprints.Count);

        foreach (var blueprint in blueprints)
        {
            var templateSize = 0; // TODO

            writer.Write(templateSize);

            var bpStartPosition = writer.BaseStream.Position;

            writer.Write(0);
            writer.Write(blueprint.Properties.Count);

            foreach (var prop in blueprint.Properties)
            {

            }
        }
    }

    public static List<Blueprint> DeserialzieJSON(Stream stream)
    {
        var blueprints = new List<Blueprint>();

        return blueprints;
    }

    public static void SerializeJSON(List<Blueprint> blueprints, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(blueprints));
    }
}
