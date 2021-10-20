using System.Collections.Generic;
using System.Linq;

namespace SabTool.Data.Lua
{
    using Blueprint;
    using Structures;

    public class LuaParam
    {
        public const int NamePad = -20;
        public const int TypePad = -10;

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
                LuaStringCrc => $"[{Name,NamePad}, {typeStr,TypePad}, {StringVal}]",
                LuaIntCrc => $"[{Name,NamePad}, {typeStr,TypePad}, {IntVal}]",
                LuaCrcListCrc => $"[{Name,NamePad}, {typeStr,TypePad}, {CrcCount}, {(CrcList.Count > 0 ? CrcList.Aggregate((a, b) => $"{a}, {b}") : "")}]",
                LuaCrcCrc => $"[{Name,NamePad}, {typeStr,TypePad}, {CrcVal}]",
                LuaBoolCrc => $"[{Name,NamePad}, {typeStr,TypePad}, {BoolVal}]",
                LuaFloatCrc => $"[{Name,NamePad}, {typeStr,TypePad}, {FloatVal}]",
                _ => $"[{Name,NamePad}, {typeStr,TypePad}, UNKNOWN LUA PARAM]",
            };
        }
    }

}
