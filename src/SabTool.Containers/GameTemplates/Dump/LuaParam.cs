using System.Collections.Generic;
using System.Linq;

namespace SabTool.Containers.GameTemplates.Dump
{
    using Client.Blueprint;

    public class LuaParam
    {
        public const uint LuaIntCrc = 0xF0676C78;
        public const uint LuaStringCrc = 0xF5B799C2;
        public const uint LuaBoolCrc = 0x1B444F31;
        public const uint LuaFloatCrc = 0x2B93BDA9;
        public const uint LuaTableCrc = 0x404D1343;
        public const uint LuaCrcCrc = 0x3290BA65;
        public const uint LuaCrcListCrc = 0xB9059DF9;

        public string Name { get; }
        public uint TypeCrc { get; }
        public string StringVal { get; }
        public int IntVal { get; }
        public int CrcCount { get; }
        public List<string> CrcList { get; } = new();
        public Crc CrcVal { get; }
        public bool BoolVal { get; }
        public float FloatVal { get; }

        public LuaParam(BluaReader reader)
        {
            Name = reader.ReadString(128);
            TypeCrc = reader.ReadUInt();

            switch (TypeCrc)
            {
                case LuaStringCrc:
                    StringVal = reader.ReadString(128);
                    break;

                case LuaIntCrc:
                    IntVal = reader.ReadInt();
                    break;

                case LuaCrcListCrc:
                    CrcCount = reader.ReadInt();

                    for (var i = 0; i < CrcCount; ++i)
                    {
                        CrcList.Add(reader.ReadString(128));
                    }
                    break;

                case LuaCrcCrc:
                    CrcVal = new(reader.ReadUInt());
                    break;

                case LuaBoolCrc:
                    BoolVal = reader.ReadBool() != 0;
                    break;

                case LuaFloatCrc:
                    FloatVal = reader.ReadFloat();
                    break;
            }
        }

        public override string ToString()
        {
            var typeStr = TypeCrc switch
            {
                LuaStringCrc => "LuaString",
                LuaIntCrc => "LuaInt",
                LuaBoolCrc => "LuaBool",
                LuaFloatCrc => "LuaFloat",
                LuaCrcListCrc => "LuaCrcList",
                LuaCrcCrc => "LuaCrc",
                _ => "UNKNOWN TYPE"
            };

            return TypeCrc switch
            {
                LuaStringCrc => $"[{Name}, {typeStr}, {StringVal}]",
                LuaIntCrc => $"[{Name}, {typeStr}, {IntVal}]",
                LuaCrcListCrc => $"[{Name}, {typeStr}, {CrcCount}, {(CrcList.Count > 0 ? CrcList.Aggregate((a, b) => $"{a}, {b}") : "")}]",
                LuaCrcCrc => $"[{Name}, {typeStr}, {CrcVal}]",
                LuaBoolCrc => $"[{Name}, {typeStr}, {BoolVal}]",
                LuaFloatCrc => $"[{Name}, {typeStr}, {FloatVal}]",
                _ => $"[{Name}, {typeStr}, UNKNOWN LUA PARAM CRC]",
            };
        }
    }

}
