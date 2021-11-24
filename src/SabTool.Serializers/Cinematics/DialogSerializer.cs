using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Cinematics
{
    using Data.Cinematics;
    using Json.Converters;
    using Utils;
    using Utils.Extensions;

    public static class DialogSerializer
    {
        public const int Version = 5;

        public static Dialog DeserialzieRaw(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            var dialog = new Dialog();

            var version = reader.ReadInt32();
            if (version != Version)
                throw new Exception("Invalid GameText.dlg version found!");

            dialog.Texts = ReadTextArray(reader);

            var subCount = reader.ReadInt32();

            dialog.SubTexts = new(subCount);

            for (var i = 0; i < subCount; ++i)
            {
                var id = new Crc(reader.ReadUInt32());
                var offset = reader.ReadInt32();

                var startPosition = stream.Position;

                stream.Position = offset;

                dialog.SubTexts.Add(id, ReadTextArray(reader));

                stream.Position = startPosition;
            }

            return dialog;
        }

        private static List<DialogText> ReadTextArray(BinaryReader reader)
        {
            var entryCount = reader.ReadInt32();
            _ = reader.ReadInt32(); // total character count

            var texts = new List<DialogText>(entryCount);

            for (var i = 0; i < entryCount; ++i)
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

            if (!reader.CheckHeaderString("CEND", reversed: true))
                throw new Exception("Invalid GameText block end tag!");

            return texts;
        }

        public static void SerializeRaw(Dialog dialog, Stream stream)
        {

        }

        public static Dialog DeserializeJSON(Stream stream)
        {
            return null;
        }

        public static void SerializeJSON(Dialog dialog, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            writer.Write(JsonConvert.SerializeObject(dialog, Formatting.Indented, new CrcConverter()));
        }
    }
}
