using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Lua
{
    using Data.Lua;

    public static class LuapSerializer
    {
        public static LuaPackage DeserializeRaw(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            var luaPackage = new LuaPackage();
            
            var count = reader.ReadInt32();
            for (var i = 0; i < count; ++i)
            {
                var entry = new LuaPackage.Entry
                {
                    PathCRC = new(reader.ReadUInt32()),
                    NameCRC = new(reader.ReadUInt32()),
                    Offset = reader.ReadInt32(),
                    Size = reader.ReadInt32(),
                    Size2 = reader.ReadInt32(),
                    IsModule = reader.ReadBoolean()
                };

                luaPackage.Entries.Add(entry);

                if (entry.Size != entry.Size2)
                    System.Diagnostics.Debugger.Break();

                var currentPosition = reader.BaseStream.Position;

                reader.BaseStream.Position = entry.Offset;

                entry.Data = reader.ReadBytes(entry.Size);

                reader.BaseStream.Position = currentPosition;
            }

            return luaPackage;
        }

        public static void SerializeRaw(LuaPackage luaPackage, Stream stream)
        {
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

            writer.Write(luaPackage.Entries.Count);

            var offset = luaPackage.Entries.Count * 21;

            for (var i = 0; i < luaPackage.Entries.Count; ++i)
            {
                writer.Write(luaPackage.Entries[i].PathCRC.Value);
                writer.Write(luaPackage.Entries[i].NameCRC.Value);
                writer.Write(offset); // todo: store later?
                writer.Write(luaPackage.Entries[i].Data.Length);
                writer.Write(luaPackage.Entries[i].Data.Length);
                writer.Write(luaPackage.Entries[i].IsModule);

                offset += luaPackage.Entries[i].Data.Length;
            }

            for (var i = 0; i < luaPackage.Entries.Count; ++i)
            {
                writer.Write(luaPackage.Entries[i].Data);
            }
        }

        public static LuaPackage DeserializeJSON(Stream stream)
        {
            return null;
        }

        public static void SerializeJSON(LuaPackage luaPackage, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            writer.Write(JsonConvert.SerializeObject(luaPackage, Formatting.Indented));
        }
    }
}
