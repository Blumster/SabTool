using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Blueprints;

using SabTool.GameData;
using SabTool.Serializers.Json.Converters;
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

                // Store hashes
                Hash.FNV32string(name);
                Hash.StringToHash(name);

                var propertyCount = reader.ReadInt32();

                var blueprintData = reader.ReadBytes(bpSize - (int)(stream.Position - bpStartPosition));

                using var subReader = new BinaryReader(new MemoryStream(blueprintData, false));

                var bp = Blueprint.Create(type, name, subReader);
                if (bp is null)
                {
                    Console.WriteLine($"Blueprint({type}, {name}) cannot be created!");
                    continue;
                }

                blueprints.Add(bp);
            }

            if (bpStartPosition + bpSize != stream.Position)
                Console.WriteLine($"Didn't properly read the blueprint! Start: {bpStartPosition}, size: {bpSize}, end: {stream.Position}, expectedEnd: {bpStartPosition + bpSize}");
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
            var bpStartPosition = stream.Position;

            writer.Write(0); // Size
            writer.Write(0); // Unknown
            writer.Write(1); // Inner count

            writer.WriteUTF8LengthedString(blueprint.Name.GetStringOrHexString());
            writer.WriteUTF8LengthedString(blueprint.Type.GetStringOrHexString());

            var propertyPosition = stream.Position;
            writer.Write(0); // Property count

            var propertyCount = blueprint.WriteProperties(writer);

            var bpEndPosition = stream.Position;

            writer.DoAtPosition(propertyPosition, _ => writer.Write(propertyCount));
            writer.DoAtPosition(bpStartPosition, _ => writer.Write((int)(bpEndPosition - bpStartPosition - 4)));
        }
    }

    public static List<Blueprint> DeserializeJSON(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            
        var blueprints = JsonConvert.DeserializeObject<List<Blueprint>>(reader.ReadToEnd(), new CrcConverter());
        if (blueprints is null)
            return new List<Blueprint>();

        return blueprints;
    }

    public static void SerializeJSON(List<Blueprint> blueprints, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(blueprints, new CrcConverter()));
    }
}
