using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Sounds;

using SabTool.Data.Sounds;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils.Extensions;

public static class WWiseIDTableSerializer
{
    public static WWiseIDTable DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var table = new WWiseIDTable();

        var version = reader.ReadUInt32();
        if (version != WWiseIDTable.Version)
            throw new Exception($"Invalid WWiseIDTable version: {version}!");

        var entry1Count = reader.ReadUInt32();
        var entry2Count = reader.ReadUInt32();
        var entry3Count = reader.ReadUInt32();
        var entry4Count = reader.ReadUInt32();
        var entry5Count = reader.ReadUInt32();
        var entry6Count = reader.ReadUInt32();
        var entry7Count = reader.ReadUInt32();
        var entry8Count = reader.ReadUInt32();
        var entry9Count = reader.ReadUInt32();
        var entry10Count = reader.ReadUInt32();

        for (var i = 0u; i < entry1Count; ++i)
            table.Entry1s.Add(DeserializeEntry1Raw(reader));

        for (var i = 0u; i < entry2Count; ++i)
            table.Entry2s.Add(DeserializeEntry2Raw(reader));

        for (var i = 0u; i < entry3Count; ++i)
            table.Entry3s.Add(DeserializeEntry3Raw(reader));

        for (var i = 0u; i < entry4Count; ++i)
            table.Entry4s.Add(DeserializeEntry3Raw(reader));

        for (var i = 0u; i < entry5Count; ++i)
            table.Entry4s.Add(DeserializeEntry3Raw(reader));

        for (var i = 0u; i < entry6Count; ++i)
            table.Entry5s.Add(DeserializeEntry3Raw(reader));

        for (var i = 0u; i < entry7Count; ++i)
            table.Entry6s.Add(DeserializeEntry3Raw(reader));

        for (var i = 0u; i < entry8Count; ++i)
            table.Entry7s.Add(DeserializeEntry3Raw(reader));

        for (var i = 0u; i < entry9Count; ++i)
            table.Entry8s.Add(DeserializeEntry3Raw(reader));

        for (var i = 0u; i < entry10Count; ++i)
            table.Entry10s.Add(DeserializeEntry3Raw(reader));

        var count = reader.ReadUInt32();
        for (var i = 0u; i < count; ++i)
            table.Params.Add(reader.ReadUTF8StringOn(reader.ReadInt32()));

        if (stream.Position != stream.Length)
            throw new Exception($"Reading WWiseIDTable failed!");

        return table;
    }

    private static WWiseIDTable.Entry1 DeserializeEntry1Raw(BinaryReader reader)
    {
        return new WWiseIDTable.Entry1
        {
            Name = reader.ReadUTF8StringOn(reader.ReadInt32()),
            Unk1 = reader.ReadUInt32(),
            Unk2 = reader.ReadUInt32(),
            Names = reader.ReadConstArray(reader.ReadInt32(), () => reader.ReadUTF8StringOn(reader.ReadInt32()))
        };
    }

    private static WWiseIDTable.Entry2 DeserializeEntry2Raw(BinaryReader reader)
    {
        return new WWiseIDTable.Entry2
        {
            Name = new(reader.ReadUInt32()),
            Unk1 = reader.ReadInt32(),
            Unk2 = reader.ReadInt32()
        };
    }

    private static WWiseIDTable.Entry3 DeserializeEntry3Raw(BinaryReader reader)
    {
        return new WWiseIDTable.Entry3
        {
            Unk1 = reader.ReadUInt32(),
            Unk2 = reader.ReadUInt32()
        };
    }

    public static void SerializeJSON(WWiseIDTable table, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(table, Formatting.Indented, new CrcConverter()));
    }
}
