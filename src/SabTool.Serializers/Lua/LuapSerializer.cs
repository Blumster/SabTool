using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Lua;

namespace SabTool.Serializers.Lua;
public static class LuapSerializer
{
    public static LuaPackage DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        LuaPackage luaPackage = new();

        int count = reader.ReadInt32();
        for (int i = 0; i < count; ++i)
        {
            LuaPackage.Entry entry = new()
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

            long currentPosition = reader.BaseStream.Position;

            reader.BaseStream.Position = entry.Offset;

            entry.Data = reader.ReadBytes(entry.Size);

            reader.BaseStream.Position = currentPosition;
        }

        return luaPackage;
    }

    public static void SerializeRaw(LuaPackage luaPackage, Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        writer.Write(luaPackage.Entries.Count);

        int offset = luaPackage.Entries.Count * 21;

        for (int i = 0; i < luaPackage.Entries.Count; ++i)
        {
            writer.Write(luaPackage.Entries[i].PathCRC.Value);
            writer.Write(luaPackage.Entries[i].NameCRC.Value);
            writer.Write(offset); // todo: store later?
            writer.Write(luaPackage.Entries[i].Data.Length);
            writer.Write(luaPackage.Entries[i].Data.Length);
            writer.Write(luaPackage.Entries[i].IsModule);

            offset += luaPackage.Entries[i].Data.Length;
        }

        for (int i = 0; i < luaPackage.Entries.Count; ++i)
        {
            writer.Write(luaPackage.Entries[i].Data);
        }
    }

    public static LuaPackage? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(LuaPackage luaPackage, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(luaPackage, Formatting.Indented));
    }
}
