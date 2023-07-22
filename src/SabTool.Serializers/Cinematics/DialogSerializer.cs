using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Cinematics;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Cinematics;
public static class DialogSerializer
{
    public const int Version = 5;

    public static Dialog DeserialzieRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        Dialog dialog = new();

        int version = reader.ReadInt32();
        if (version != Version)
            throw new Exception("Invalid GameText.dlg version found!");

        dialog.Texts = ReadTextArray(reader);

        int subCount = reader.ReadInt32();

        dialog.SubTexts = new(subCount);

        for (int i = 0; i < subCount; ++i)
        {
            Crc id = new(reader.ReadUInt32());
            int offset = reader.ReadInt32();

            long startPosition = stream.Position;

            stream.Position = offset;

            dialog.SubTexts.Add(id, ReadTextArray(reader));

            stream.Position = startPosition;
        }

        return dialog;
    }

    private static List<DialogText> ReadTextArray(BinaryReader reader)
    {
        int entryCount = reader.ReadInt32();
        _ = reader.ReadInt32(); // total character count

        List<DialogText> texts = new(entryCount);

        for (int i = 0; i < entryCount; ++i)
        {
            if (!reader.CheckHeaderString("DTXT", reversed: true))
                throw new Exception("Invalid GameText start tag!");

            texts.Add(new DialogText
            {
                Id = new Crc(reader.ReadUInt32()),
                VoiceOver = reader.ReadUTF8StringOn(reader.ReadInt16()),
                Text = reader.ReadUTF16StringOn(reader.ReadInt16())
            });
        }

        return !reader.CheckHeaderString("CEND", reversed: true) ? throw new Exception("Invalid GameText block end tag!") : texts;
    }

    public static void SerializeRaw(Dialog dialog, Stream stream)
    {

    }

    public static Dialog? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Dialog dialog, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(dialog, Formatting.Indented, new CrcConverter()));
    }
}
