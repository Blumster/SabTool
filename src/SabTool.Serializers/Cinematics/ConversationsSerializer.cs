using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Cinematics;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Cinematics;
public static class ConversationsSerializer
{
    private const int Version = 10;

    public static List<ConversationStructure> DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        int version = reader.ReadInt32();
        if (version != Version)
            throw new Exception($"Invalid version {version} for Conversations!");

        int count = reader.ReadInt32();

        List<ConversationStructure> structures = new(count);

        for (int i = 0; i < count; ++i)
        {
            int size = reader.ReadInt32();
            long startPosition = stream.Position;

            short ver2 = reader.ReadInt16();
            if (ver2 != 10)
            {
                stream.Position = startPosition + size;
                continue;
            }

            ConversationStructure conversation = new()
            {
                Name = new(reader.ReadUInt32())
            };

            if (!DeserializeConversation(reader, conversation))
            {
                stream.Position = startPosition + size;
                continue;
            }

            structures.Add(conversation);

            if (stream.Position != startPosition + size)
                throw new Exception($"Over or under reading Conversation! Pos: {stream.Position} != Expected: {startPosition + size}!");
        }

        return structures;
    }

    private static bool DeserializeConversation(BinaryReader reader, ConversationStructure conversation)
    {
        short numLines = reader.ReadInt16();
        if (numLines == 0)
            return false;

        conversation.Lines = new ConversationLineStructure[numLines];

        short numNextLines = reader.ReadInt16();
        conversation.NextLines = new(numNextLines);

        while (true)
        {
            uint tag = reader.ReadUInt32();
            if (tag == 0x43454E44) // CEND
                break;

            switch (tag)
            {
                case 0x53554254: // SUBT
                    conversation.Flags |= ConversationFlags.Subtitle;
                    break;

                case 0x534E3344: // SN3D
                    if (reader.ReadInt16() != 0)
                    {
                        conversation.Flags |= ConversationFlags.Sound3D;
                    }
                    else
                    {
                        conversation.Flags &= ~ConversationFlags.Sound3D;
                    }
                    break;

                case 0x53445354: // SDST
                    conversation.SoundDistance = reader.ReadSingle();
                    break;

                case 0x534C4F43: // SLOC
                    conversation.SoundLocation = reader.ReadInt32();
                    break;

                case 0x53435231: // SCR1
                    conversation.Script[1] = reader.ReadStringWithMaxLength(reader.ReadInt16());

                    // Store hash if missing
                    _ = Hash.FNV32string(conversation.Script[1]);
                    break;

                case 0x53424E4B: // SBNK
                    conversation.SoundBank = reader.ReadStringWithMaxLength(reader.ReadInt16());

                    // Store hash if missing
                    _ = Hash.FNV32string(conversation.SoundBank);
                    break;

                case 0x53435230: // SCR0
                    conversation.Script[0] = reader.ReadStringWithMaxLength(reader.ReadInt16());

                    // Store hash if missing
                    _ = Hash.FNV32string(conversation.Script[0]);
                    break;

                case 0x524E444C: // RNDL
                    conversation.Flags |= ConversationFlags.RandomLine;
                    break;

                case 0x4E455854: // NEXT
                    conversation.NextLines.Add(reader.ReadUInt16());
                    break;

                case 0x4E4F4645: // NOFE
                    conversation.Flags &= ~ConversationFlags.Unk20;
                    break;

                case 0x4C494E45: // LINE
                    short idx = reader.ReadInt16();

                    if (conversation.Lines[idx] != null)
                        throw new Exception("Line is being reloaded?");

                    conversation.Lines[idx] = new ConversationLineStructure
                    {
                        Idx = idx
                    };

                    if (!DeserializeConversationLine(reader, conversation.Lines[idx], conversation))
                        throw new Exception("Unable to deserialize ConversationLineStructure!");

                    break;

                case 0x48545854: // HTXT
                    Crc humanCrc = new(reader.ReadUInt32());

                    if (conversation.Humans.TryGetValue(humanCrc, out ConversationStructure.Human? human))
                        human.Float1 = reader.ReadSingle();

                    break;

                case 0x4A524E4C: // JRNL
                    conversation.Flags |= ConversationFlags.Journal;
                    break;

                case 0x484D4135: // HMA5
                    Crc human5Key = new(reader.ReadUInt32());

                    conversation.Humans.Add(human5Key, new ConversationStructure.Human("HMA5")
                    {
                        Crc1 = new(reader.ReadUInt32()),
                        Crc2 = new(reader.ReadUInt32()),
                        Bool1 = reader.ReadByte() != 0
                    });
                    break;

                case 0x484D4133: // HMA3
                    Crc human3Key = new(reader.ReadUInt32());

                    conversation.Humans.Add(human3Key, new ConversationStructure.Human("HMA3")
                    {
                        Crc1 = new(reader.ReadUInt32()),
                        Float1 = reader.ReadSingle(), // TODO: unused?
                        Crc2 = new(reader.ReadUInt32()),
                        Bool1 = true
                    });
                    break;

                case 0x484D4134: // HMA4
                    Crc human4Key = new(reader.ReadUInt32());

                    conversation.Humans.Add(human4Key, new ConversationStructure.Human("HMA4")
                    {
                        Crc1 = new(reader.ReadUInt32()),
                        Crc2 = new(reader.ReadUInt32()),
                        Bool1 = true
                    });
                    break;

                case 0x484D4132: // HMA2
                    Crc human2Key = new(reader.ReadUInt32());

                    conversation.Humans.Add(human2Key, new ConversationStructure.Human("HMA2")
                    {
                        Crc1 = new(reader.ReadUInt32()),
                        Float1 = reader.ReadSingle(), // TODO: unused?
                        Bool1 = true
                    });
                    break;

                case 0x44495354: // DIST
                    conversation.DistanceSqr = reader.ReadSingle();
                    break;

                case 0x46494C45: // FILE
                    conversation.File = new(reader.ReadUInt32());
                    break;

                case 0x43414D31: // CAM1
                    conversation.Camera = new(reader.ReadUInt32());
                    break;

                case 0x43415230: // CAR0
                    conversation.Flags |= ConversationFlags.Car;
                    break;
            }
        }

        return true;
    }

    private static bool DeserializeConversationLine(BinaryReader reader, ConversationLineStructure line, ConversationStructure conversation)
    {
        line.NextLines = new(reader.ReadInt16());

        while (true)
        {
            uint tag = reader.ReadUInt32();
            if (tag == 0x43454E44) // CEND
                break;

            switch (tag)
            {
                case 0x54524754: // TRGT
                    line.Target = new(reader.ReadUInt32());
                    break;

                case 0x54585432: // TXT2
                    line.Texts.Add(new(reader.ReadUInt32()), new(reader.ReadUInt32()));
                    break;

                case 0x54585443: // TXTC
                    line.ConditionCrc = new(reader.ReadUInt32());
                    break;

                case 0x53504B52: // SPKR
                    line.Speaker = new(reader.ReadUInt32());
                    break;

                case 0x53435231: // SCR1
                    line.Script[1] = reader.ReadStringWithMaxLength(reader.ReadInt16());

                    // Store hash if missing
                    _ = Hash.FNV32string(line.Script[1]);
                    break;

                case 0x53435230: // SCR0
                    line.Script[0] = reader.ReadStringWithMaxLength(reader.ReadInt16());

                    // Store hash if missing
                    _ = Hash.FNV32string(line.Script[0]);
                    break;

                case 0x4E414D45: // NAME
                    line.Name = new(reader.ReadUInt32());
                    break;

                case 0x4E455854: // NEXT
                    line.NextLines.Add(reader.ReadUInt16());
                    break;

                case 0x4C494E45: // LINE
                    short idx = reader.ReadInt16();

                    if (conversation.Lines[idx] != null)
                        throw new Exception("Line is being reloaded?");

                    conversation.Lines[idx] = new ConversationLineStructure
                    {
                        Idx = idx
                    };

                    if (!DeserializeConversationLine(reader, conversation.Lines[idx], conversation))
                        throw new Exception("Unable to deserialize ConversationLineStructure!");

                    break;

                case 0x494E5452: // INTR
                    line.Flags |= ConversationLineFlags.Interrupt;
                    break;

                case 0x494E464F: // INFO
                    Crc infoKey = new(reader.ReadUInt32());

                    line.Humans.Add(infoKey, new ConversationLineStructure.Human("INFO")
                    {
                        Crc1 = new(reader.ReadUInt32()),
                        Crc2 = new(reader.ReadUInt32()),
                        Float1 = reader.ReadSingle() // TODO: unused?
                    });
                    break;

                case 0x494E4633: // INF3
                    Crc inf3Key = new(reader.ReadUInt32());

                    ConversationLineStructure.Human inf3Human = new("INF3")
                    {
                        Crc1 = new(reader.ReadUInt32()),
                        Crc2 = new(reader.ReadUInt32()),
                        Float1 = reader.ReadSingle()
                    };

                    if (inf3Key == line.Speaker)
                        line.Target = inf3Human.Crc1;
                    else if (inf3Human.Crc2.Value == 0 && inf3Human.Crc1.Value != 0)
                        inf3Human.Crc2 = new(0xDA5398F4);

                    line.Humans.Add(inf3Key, inf3Human);
                    break;

                case 0x494E4634: // INF4
                    Crc inf4Key = new(reader.ReadUInt32());

                    ConversationLineStructure.Human inf4Human = new("INF4")
                    {
                        Crc1 = new(reader.ReadUInt32()),
                        Crc2 = new(reader.ReadUInt32()),
                        Float1 = reader.ReadSingle(),
                        Bool1 = reader.ReadByte() != 0
                    };

                    if (inf4Key == line.Speaker)
                        line.Target = inf4Human.Crc1;
                    else if (inf4Human.Crc2.Value == 0 && inf4Human.Crc1.Value != 0)
                        inf4Human.Crc2 = new(0xDA5398F4);

                    break;

                case 0x494E4632: // INF2
                    Crc inf2Key = new(reader.ReadUInt32());

                    ConversationLineStructure.Human inf2Human = new("INF2")
                    {
                        Crc1 = new(reader.ReadUInt32()),
                        Crc2 = new(reader.ReadUInt32()),
                        Float1 = reader.ReadSingle(),
                        Float2 = reader.ReadInt32() // TODO: unused?
                    };

                    if (inf2Key == line.Speaker)
                        line.Target = inf2Human.Crc1;
                    else if (inf2Human.Crc2.Value == 0 && inf2Human.Crc1.Value != 0)
                        inf2Human.Crc2 = new(0xDA5398F4);

                    line.Humans.Add(inf2Key, inf2Human);
                    break;

                case 0x44454C41: // DELA
                    line.Delay = reader.ReadSingle();
                    break;

                case 0x434E4454: // CNDT
                    line.Condition = reader.ReadInt16();

                    if (line.Condition == 6)
                    {
                        line.ConditionText = reader.ReadStringWithMaxLength(reader.ReadInt16());

                        // Store hash if missing
                        _ = Hash.FNV32string(line.ConditionText);
                    }

                    break;

                case 0x43494E54: // CINT
                    line.Flags |= ConversationLineFlags.CInt;
                    break;
            }
        }

        return true;
    }

    public static void SerialzieRaw(List<ConversationStructure> structures, Stream stream)
    {

    }

    public static List<ConversationStructure>? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(List<ConversationStructure> structures, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(structures, Formatting.Indented, new CrcConverter()));
    }
}
