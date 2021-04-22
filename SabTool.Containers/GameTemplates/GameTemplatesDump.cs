using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabTool.Containers.GameTemplates
{
    using Client.Blueprint;
    using Dump;

    public class GameTemplatesDump : GameTemplatesBase
    {
        private TextWriter Output { get; }

        public GameTemplatesDump(TextWriter output = null)
        {
            Output = output;

            if (Output == null)
            {
                Output = Console.Out;
            }
        }

        protected override void ReadBlueprint(string type, string name, BluaReader reader)
        {
            Output.WriteLine($"Blueprint({type}, {name}):");

            while (reader.Offset < reader.Size)
            {
                var crc = reader.ReadUInt();
                var size = reader.ReadInt();

                // Store the starting position
                var startOff = reader.Offset;

                // Read the property
                ReadProperty(reader, crc, size);

                // Assign the valid offset to the reader, even if the ReadProperty read too little or too much
                reader.Offset = startOff + size;
            }

            Output.WriteLine();
        }

        private void ReadProperty(BluaReader reader, uint crc, int size)
        {
            if (PropertyTypes.TryGetValue(crc, out var entry))
            {
                Output.WriteLine($"[0x{crc:X8}][{entry.Category}][{entry.Name}]: {ReadPropertyValue(reader, entry.Type, size)}");
            }
            else
            {
                var bytes = reader.ReadBytes(size);

                Output.WriteLine($"[0x{crc:X8}][UNKNOWN][UNKNOWN]: {BitConverter.ToString(bytes).Replace('-', ' ')} ({(size >= 4 ? BitConverter.ToInt32(bytes, 0) : "")}, {(size >= 4 ? BitConverter.ToSingle(bytes, 0) : "")}, {Encoding.UTF8.GetString(bytes)})");
            }
        }

        private static object ReadPropertyValue(BluaReader reader, Type type, int size)
        {
            return type.Name switch
            {
                "Int32" => reader.ReadInt(),
                "Boolean" => reader.ReadBool() != 0,
                "Byte" => reader.ReadByte(),
                "String" => reader.ReadString(size),
                "Crc" => new Crc(reader.ReadUInt()),
                "LuaParam" => new LuaParam(reader),
                "Single" => reader.ReadFloat(),
                "Double" => reader.ReadDouble(),
                "Byte[]" => BitConverter.ToString(reader.ReadBytes(size)).Replace('-', ' '),
                _ => null
            };
        }

        record TypeEntry(string Category, string Name, Type Type);

        private static Dictionary<uint, TypeEntry> PropertyTypes { get; } = new()
        {
            // Common properties
            { 0x6302F1CC, new("Common", "dynamic", typeof(bool)) },
            { 0x8C9928FB, new("Common", "UseDynamicPool", typeof(bool)) },
            { 0x87519019, new("Common", "priority", typeof(int)) },
            { 0xBF83D3AF, new("Common", "backup", typeof(Crc)) },
            { 0x0C2DB3B4, new("Common", "managed", typeof(bool)) },

            // Damagable properties

            // Audible properties
            { 0x42C6DA9D, new("Audible", "", typeof(int)) },
            { 0x246DB06C, new("Audible", "", typeof(int)) },
            { 0x3217CDF0, new("Audible", "", typeof(float)) },
            { 0x212A9DB0, new("Audible", "", typeof(bool)) },
            { 0x02D3EB9F, new("Audible", "", typeof(int)) },
            { 0x04FBB1EE, new("Audible", "", typeof(string)) },
            { 0x1E59E605, new("Audible", "Looping", typeof(bool)) },
            { 0xD76D7747, new("Audible", "", typeof(int)) },
            { 0x85B86BD7, new("Audible", "", typeof(bool)) },
            { 0xAFCE4BA2, new("Audible", "", typeof(int)) },
            { 0xEC8964B3, new("Audible", "", typeof(int)) },
            { 0xE94E8608, new("Audible", "", typeof(int)) },

            // ModelRenderable properties
            { 0x5B724250, new("ModelRenderable", "Model", typeof(Crc)) },
            { 0xAE1ED17F, new("ModelRenderable", "HasWillToFight?", typeof(bool)) },

            // Targetable properties

            // AIAttraction properties

            // Controllable properties
            { 0xFB31F1EF, new("Controllable", "", typeof(Crc)) },

            // 0x194 SetProperty
            //{ , new("", "", typeof()) },

            // Lua
            { 0x404D1343, new("Lua", "LuaTable", typeof(Crc)) },
            { 0xDB0F705C, new("Lua", "LuaParam", typeof(LuaParam)) },

            // WSAIAttractionPt
            { 0x7EF2B668, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x7E4E77E0, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x7ED98262, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xE773FC1F, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0xE29D36B0, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xE66CED59, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0xE1291EAE, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xD411C23C, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0xDE634E2F, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0xDF0B2054, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xCF88DE36, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xD235DA20, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xCE1A4A79, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xC6E0F627, new("WSAIAttractionPt", "", typeof(double)) },
            { 0xC8DDD177, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xCBD8745E, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xD27568AD, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0xF01F08A8, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xEA73DB2F, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0xECF2E7AE, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xE9CACFF7, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0xE7B1D609, new("WSAIAttractionPt", "", typeof(Crc)) },
            //{ 0xE94E8608, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xE9A5BC9C, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0xFB14F042, new("WSAIAttractionPt", "", typeof(float)) },
            //{ 0xFB31F1EF, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xF8AFE581, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xF0C7DA5F, new("WSAIAttractionPt", "", typeof(string)) },
            { 0xF10C031B, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xF49F0182, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xC54E8CAE, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x9EDB6BD9, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x99A54CBF, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x9AAC5788, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x982589A0, new("WSAIAttractionPt", "", typeof(byte[])) },
            { 0x923BA76B, new("WSAIAttractionPt", "", typeof(byte[])) },
            { 0x92D0A9BA, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x97AC8632, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x8FD2512F, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x8C8D6435, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x8D89FAD9, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x8DB13A88, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x888505C3, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x07F79C5D, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x84C77E96, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x84D70C5E, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xB3AF2A59, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0xA8DB360C, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xAEEA42F8, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0xB2E588D7, new("WSAIAttractionPt", "", typeof(double)) },
            { 0xB40462AD, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xC1D0622B, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xA8079DD8, new("WSAIAttractionPt", "", typeof(double)) },
            { 0xA5A71B8A, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xA7E0D198, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0xA3FBC25C, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x9FDC0AE6, new("WSAIAttractionPt", "", typeof(int)) },
            { 0xA0A39AA4, new("WSAIAttractionPt", "", typeof(float)) },
            { 0xA2FF7739, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x7E1D5456, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x658D48E2, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x506DE042, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x4785F2A3, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x4A67BCC0, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x4D44EFE0, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x47751B18, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x4172D6FB, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x41B4ACE8, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x460E7F77, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x55D89223, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x507A477F, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x5253FC93, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x530D6414, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x5E58EAC8, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x5E77F41F, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x733ED3B2, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x6C6A0DB8, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x6E15FFFF, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x726F6DCC, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x6BE83213, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x67277C2C, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x6935C0FD, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x6A099303, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x7C0D1F8E, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x7CD0DF87, new("WSAIAttractionPt", "", typeof(string)) },
            { 0x7A3E99B4, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x7794DACB, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x787A47A5, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x7A157568, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x4070B35E, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x239B1E1B, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x0F12604C, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x06273164, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x07AD6EBF, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x09E38146, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x054A8FB1, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x019EF97E, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x01D505A2, new("WSAIAttractionPt", "", typeof(bool)) },
            //{ 0x04FBB1EE, new("WSAIAttractionPt", "", typeof(string)) },
            { 0x1DE59C2F, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x16930AFE, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x192A3633, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x1B78F12E, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x1DE5C824, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x23518B03, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x3DCC7355, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x3DFE6182, new("WSAIAttractionPt", "", typeof(int)) },
            { 0x397B6A20, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x331D2AA8, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x3566AE45, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x365FB660, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x32D97474, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x2D5B659B, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x2ED2EC21, new("WSAIAttractionPt", "", typeof(bool)) },
            //{ 0x3217CDF0, new("WSAIAttractionPt", "", typeof(float)) },
            { 0x2C652AF7, new("WSAIAttractionPt", "", typeof(Crc)) },
            { 0x2523B4EE, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x29EC9F57, new("WSAIAttractionPt", "", typeof(bool)) },
            { 0x2A4802C1, new("WSAIAttractionPt", "", typeof(bool)) },

            // WSAIScriptController
            { 0xE0B01D9F, new("WSAIScriptController", "InitialModule", typeof(string)) },
            { 0xCA6B9057, new("WSAIScriptController", "", typeof(int)) },
            { 0xC072E94A, new("WSAIScriptController", "", typeof(float)) },
            { 0xC4C412CD, new("WSAIScriptController", "RainStartDist", typeof(float)) },
            { 0xBF930289, new("WSAIScriptController", "", typeof(int)) },
            { 0xBCFE6314, new("WSAIScriptController", "Path", typeof(string)) },
            { 0xBE5952B1, new("WSAIScriptController", "", typeof(float)) },
            { 0xAE350719, new("WSAIScriptController", "LightningStopDist", typeof(float)) },
            { 0xA3DBF09D, new("WSAIScriptController", "", typeof(bool)) },
            { 0xA77EDAAA, new("WSAIScriptController", "", typeof(bool)) },
            { 0x97EE12C6, new("WSAIScriptController", "", typeof(bool)) },
            { 0x7A2AA5EF, new("WSAIScriptController", "LightningStartDist", typeof(float)) },
            { 0x85B5E29F, new("WSAIScriptController", "", typeof(string)) },
            { 0x89A84EF5, new("WSAIScriptController", "", typeof(int)) },
            { 0xF4BADFDB, new("WSAIScriptController", "RainDelta", typeof(float)) },
            { 0xE4E34549, new("WSAIScriptController", "", typeof(int)) },
            { 0xF6B4FF0E, new("WSAIScriptController", "", typeof(int)) },
            { 0xFE8BAE82, new("WSAIScriptController", "", typeof(int)) },
            { 0xDDF1EE0F, new("WSAIScriptController", "", typeof(float)) },
            { 0xD72AA401, new("WSAIScriptController", "", typeof(int)) },
            { 0xCACFD6AA, new("WSAIScriptController", "", typeof(int)) },
            { 0xCE629E7E, new("WSAIScriptController", "", typeof(int)) },
            { 0xD6F4D903, new("WSAIScriptController", "ActivationRadius", typeof(float)) },
            { 0xD730D0E1, new("WSAIScriptController", "InvincibleRange", typeof(float)) },
            { 0xD79C2BE9, new("WSAIScriptController", "", typeof(int)) },
            { 0x77776D5A, new("WSAIScriptController", "", typeof(int)) },
            { 0x7154DC63, new("WSAIScriptController", "RainStopDist", typeof(float)) },
            { 0x722C0953, new("WSAIScriptController", "", typeof(int)) },
            { 0x6A9AB174, new("WSAIScriptController", "", typeof(float)) },
            { 0x4177A478, new("WSAIScriptController", "", typeof(int)) },
            { 0x44885A78, new("WSAIScriptController", "Is3D", typeof(bool)) },
            { 0x37FC008D, new("WSAIScriptController", "", typeof(float)) },
            { 0x2F1820CC, new("WSAIScriptController", "", typeof(float)) },
            { 0x3570F09D, new("WSAIScriptController", "", typeof(int)) },
            { 0x19BAB4C9, new("WSAIScriptController", "ActivationChance", typeof(float)) },
            { 0x0424DD32, new("WSAIScriptController", "", typeof(bool)) },
            { 0x07B13063, new("WSAIScriptController", "", typeof(int)) },
            { 0x165C1FD7, new("WSAIScriptController", "", typeof(int)) },

            // WSEventConversation

            // WSPhysicsParticle
            { 0x9C323CFD, new("WSPhysicsParticle", "EffectRandomizeNumber?", typeof(float)) },
            //{ 0x5B724250, new("WSPhysicsParticle", "", typeof(Crc)) }, // already as shared
            { 0x7BE5AA43, new("WSPhysicsParticle", "", typeof(float)) },
            { 0xADAE06C3, new("WSPhysicsParticle", "", typeof(byte)) },
            { 0x5608BD5A, new("WSPhysicsParticle", "", typeof(Crc)) },
            { 0x50F8BFFA, new("WSPhysicsParticle", "", typeof(byte)) },
            { 0x46EB60BF, new("WSPhysicsParticle", "", typeof(float)) },
            { 0x2E978372, new("WSPhysicsParticle", "", typeof(Crc)) },
            { 0x3904F4E4, new("WSPhysicsParticle", "", typeof(int)) },
        };
    }
}
