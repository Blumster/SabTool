using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Sounds;

using SabTool.Data.Sounds;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils.Extensions;

public static class SoundPackSerializer
{
    public static SoundPack DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var soundPack = new SoundPack
        {
            Tag = reader.ReadHeaderString(4, reversed: true),
            Align = reader.ReadInt32(),
            Unk1 = reader.ReadInt32(),
            NumBanks = reader.ReadInt32(),
            BankEntriesOffset = reader.ReadInt32(),
            NumStreams = reader.ReadInt32(),
            StreamEntriesOffset = reader.ReadInt32()
        };

        if (soundPack.Tag != "PCK1")
            throw new Exception($"Invalid Sound Bank tag {soundPack.Tag}!");

        var banks = new List<SoundPack.Entry>();
        var streams = new List<SoundPack.Entry>();

        if (stream.Position != soundPack.BankEntriesOffset)
            throw new Exception("Invalid position for SoundBank entries!");

        for (var i = 0; i < soundPack.NumBanks; ++i)
            banks.Add(new SoundPack.Entry(reader.ReadUInt32(), reader.ReadInt32(), reader.ReadInt32()));

        if (stream.Position != soundPack.StreamEntriesOffset)
            throw new Exception("Invalid position for SoundStream entries!");

        for (var i = 0; i < soundPack.NumStreams; ++i)
            streams.Add(new SoundPack.Entry(reader.ReadUInt32(), reader.ReadInt32(), reader.ReadInt32()));

        foreach (var entry in banks)
        {
            stream.Position = entry.Offset;

            var bank = DeserializeBank(reader, entry.Size);
            bank.Id = entry.Id;

            soundPack.SoundBanks.Add(bank);

            if (stream.Position != entry.Offset + entry.Size)
                throw new Exception("Under or overreading SoundBank!");
        }

        foreach (var entry in streams)
        {
            stream.Position = entry.Offset;

            var soundStream = DeserializeStream(reader, entry.Size);
            soundStream.Id = entry.Id;

            soundPack.SoundStreams.Add(soundStream);

            if (stream.Position != entry.Offset + entry.Size)
                throw new Exception("Under or overreading SoundStream!");
        }

        return soundPack;
    }

    private static SoundBank DeserializeBank(BinaryReader reader, int size)
    {
        var bank = new SoundBank
        {
            Data = reader.ReadBytes(size)
        };

        // TODO?

        return bank;
    }

    private static SoundStream DeserializeStream(BinaryReader reader, int size)
    {
        var stream = new SoundStream
        {
            Data = reader.ReadBytes(size)
        };

        return stream;
    }

    public static void SerialzieRaw(SoundPack soundPack, Stream stream)
    {

    }

    public static SoundPack? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(SoundPack soundPack, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(soundPack, Formatting.Indented, new CrcConverter()));
    }
}
