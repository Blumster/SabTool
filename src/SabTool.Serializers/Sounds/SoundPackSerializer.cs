using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Sounds;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Sounds;
public static class SoundPackSerializer
{
    public static SoundPack DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        SoundPack soundPack = new()
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

        List<SoundPack.Entry> banks = new();
        List<SoundPack.Entry> streams = new();

        if (stream.Position != soundPack.BankEntriesOffset)
            throw new Exception("Invalid position for SoundBank entries!");

        for (int i = 0; i < soundPack.NumBanks; ++i)
            banks.Add(new SoundPack.Entry(new(reader.ReadUInt32()), reader.ReadInt32(), reader.ReadInt32()));

        if (stream.Position != soundPack.StreamEntriesOffset)
            throw new Exception("Invalid position for SoundStream entries!");

        for (int i = 0; i < soundPack.NumStreams; ++i)
            streams.Add(new SoundPack.Entry(new(reader.ReadUInt32()), reader.ReadInt32(), reader.ReadInt32()));

        foreach (SoundPack.Entry entry in banks)
        {
            stream.Position = entry.Offset;

            SoundBank bank = DeserializeBank(reader, entry.Size);
            bank.Id = entry.Id;

            soundPack.SoundBanks.Add(bank);

            if (stream.Position != entry.Offset + entry.Size)
                throw new Exception("Under or overreading SoundBank!");
        }

        foreach (SoundPack.Entry entry in streams)
        {
            stream.Position = entry.Offset;

            SoundStream soundStream = DeserializeStream(reader, entry.Size);
            soundStream.Id = entry.Id;

            soundPack.SoundStreams.Add(soundStream);

            if (stream.Position != entry.Offset + entry.Size)
                throw new Exception("Under or overreading SoundStream!");
        }

        return soundPack;
    }

    private static SoundBank DeserializeBank(BinaryReader reader, int size)
    {
        SoundBank bank = new()
        {
            Data = reader.ReadBytes(size)
        };

        // TODO?

        return bank;
    }

    private static SoundStream DeserializeStream(BinaryReader reader, int size)
    {
        SoundStream stream = new()
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
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(soundPack, Formatting.Indented, new CrcConverter()));
    }
}
