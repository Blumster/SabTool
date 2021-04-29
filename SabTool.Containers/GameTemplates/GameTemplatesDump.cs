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
            var name = Utils.Hash.HashToString(crc);

            if (PropertyTypes.TryGetValue(crc, out var entry))
            {
                Output.WriteLine($"[0x{crc:X8}][{entry.Category}][{(string.IsNullOrEmpty(entry.Name) && !string.IsNullOrEmpty(name) ? name : entry.Name)}]: {ReadPropertyValue(reader, entry.Type, size)}");
            }
            else
            {
                var bytes = reader.ReadBytes(size);

                Output.WriteLine($"[0x{crc:X8}][UNKNOWN][{(string.IsNullOrEmpty(name) ? "UNKNOWN" : name)}]: {BitConverter.ToString(bytes).Replace('-', ' ')} ({(size >= 4 ? BitConverter.ToInt32(bytes, 0) : "")}, {(size >= 4 ? BitConverter.ToSingle(bytes, 0) : "")}, {Encoding.UTF8.GetString(bytes)})");
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
                "FloatRange2" => new FloatRange2(reader.ReadFloat(), reader.ReadFloat()),
                "FloatRange4" => new FloatRange4(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                _ => null
            };
        }

        record TypeEntry(string Category, string Name, Type Type);
        record FloatRange2(float Val1, float Val2)
        {
            public override string ToString()
            {
                return $"({Val1}, {Val2})";
            }
        }
        record FloatRange4(float Val1, float Val2, float Val3, float Val4)
        {
            public override string ToString()
            {
                return $"({Val1}, {Val2}, {Val3}, {Val4})";
            }
        }

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
            { 0x42C6DA9D, new("Audible", "Sound Events", typeof(int)) },
            { 0x246DB06C, new("Audible", "", typeof(Crc)) },
            { 0x3217CDF0, new("Audible", "", typeof(float)) },
            { 0x212A9DB0, new("Audible", "", typeof(bool)) },
            { 0x02D3EB9F, new("Audible", "", typeof(Crc)) },
            { 0x04FBB1EE, new("Audible", "", typeof(string)) },
            { 0x1E59E605, new("Audible", "Looping", typeof(bool)) },
            { 0xD76D7747, new("Audible", "", typeof(Crc)) },
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
            { 0x16930AFE, new("WSAIAttractionPt", "Explosion", typeof(Crc)) },
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

            // WSDamageableBlueprint
            { 0xB37BF5B2, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x97E0DB0F, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0xA1B82BA7, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0xAC0F66B8, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x08ECB69D, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x8CBC442D, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x82E62211, new("WSDamageableBlueprint", "", typeof(Crc)) },
            { 0x7C203814, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x81CD9A01, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0xDD7EAE2C, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0xDA3A3781, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0xD0E3A1A1, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0xC11C102E, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0xC86877FD, new("WSDamageableBlueprint", "", typeof(int)) },
            { 0xF7C72618, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0xF9C458E7, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0xFD4869CD, new("WSDamageableBlueprint", "", typeof(FloatRange2)) },
            { 0x75791974, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x55817592, new("WSDamageableBlueprint", "", typeof(int)) },
            { 0x6AC12B1E, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x73FF7621, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x54019561, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x4F18FA7A, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x496A25D2, new("WSDamageableBlueprint", "", typeof(byte)) },
            { 0x49C74990, new("WSDamageableBlueprint", "", typeof(int)) },
            //{ 0x4B186A63, new("WSDamageableBlueprint", "", typeof()) },
            { 0x416E074F, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x2D96C4F2, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x2A3029F3, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x24E96E81, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x0E29263B, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x12066797, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x3121EECD, new("WSDamageableBlueprint", "", typeof(Crc)) },
            { 0x3D960EA8, new("WSDamageableBlueprint", "", typeof(float)) },
            { 0x414631B3, new("WSDamageableBlueprint", "", typeof(int)) },

            // WSEventConversation

            // WSItem
            { 0x953C60F5, new("WSItem", "", typeof(float)) },
            { 0x8303620E, new("WSItem", "", typeof(int)) },
            { 0x75060ED9, new("WSItem", "", typeof(bool)) },
            { 0x6ADE730A, new("WSItem", "", typeof(Crc)) },
            { 0x70785B2A, new("WSItem", "", typeof(bool)) },
            //{ 0x97AC8632, new("WSItem", "", typeof(int)) },
            { 0xC6676245, new("WSItem", "", typeof(int)) },
            { 0xF7CEEE0B, new("WSItem", "", typeof(bool)) },
            { 0x650A47ED, new("WSItem", "", typeof(bool)) },
            { 0x2D1BCD5A, new("WSItem", "", typeof(Crc)) },
            //{ 0x5B724250, new("WSItem", "", typeof(int)) },
            { 0x5D4608B5, new("WSItem", "", typeof(bool)) },
            { 0x1E757409, new("WSItem", "", typeof(Crc)) },
            { 0x13F47172, new("WSItem", "", typeof(bool)) },
            { 0x0F59CD06, new("WSItem", "", typeof(Crc)) },
            { 0x06DA8775, new("WSItem", "", typeof(Crc)) },
            { 0x0D111AB7, new("WSItem", "PickupHighlight", typeof(Crc)) },

            // WSLightAttachement
            { 0xC25641DF, new("WSLightAttachement", "", typeof(int)) },
            { 0x9AB4F3ED, new("WSLightAttachement", "", typeof(int)) },
            { 0xA3613F4D, new("WSLightAttachement", "", typeof(int)) },
            { 0x8ED97E12, new("WSLightAttachement", "", typeof(float)) },
            { 0x6C9D982B, new("WSLightAttachement", "", typeof(int)) },
            { 0x81EB0E4F, new("WSLightAttachement", "", typeof(float)) },
            { 0xA873293A, new("WSLightAttachement", "", typeof(int)) },

            // WSLightHaloAttachment
            { 0x54F0B858, new("WSLightHaloAttachment", "", typeof(float)) },
            { 0xB4A80855, new("WSLightHaloAttachment", "", typeof(int)) },
            { 0xF80ABFB7, new("WSLightHaloAttachment", "", typeof(int)) },
            { 0x438C44F5, new("WSLightHaloAttachment", "", typeof(int)) },
            { 0x0B213D8E, new("WSLightHaloAttachment", "", typeof(int)) },
            { 0x1C128785, new("WSLightHaloAttachment", "", typeof(float)) },
            { 0x2A8F3B51, new("WSLightHaloAttachment", "", typeof(int)) },

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

            // WSWeapon
            { 0x7C0B86B9, new("WSWeapon", "", typeof(bool)) },
            { 0x334A7577, new("WSWeapon", "", typeof(int)) },
            { 0x1667D2C6, new("WSWeapon", "MaxAIMissAngle?", typeof(float)) },
            { 0x0F3D263B, new("WSWeapon", "", typeof(float)) },
            { 0x142C5C7A, new("WSWeapon", "NormalSpread", typeof(float)) },
            { 0x1508D7DD, new("WSWeapon", "", typeof(FloatRange4)) },
            { 0x0B3830D6, new("WSWeapon", "", typeof(float)) },
            { 0x09C7D282, new("WSWeapon", "RoundsPerClip?", typeof(int)) },
            { 0x079C2D5B, new("WSWeapon", "", typeof(float)) },
            { 0x06A053C3, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0x074D0D0F, new("WSWeapon", "AICloseAdjustTime?", typeof(float)) },
            { 0x05B6BA7C, new("WSWeapon", "AIBurstCooldown?", typeof(float)) },
            { 0x02057C8B, new("WSWeapon", "MaxDPSMultiplier?", typeof(float)) },
            { 0x0140B5C6, new("WSWeapon", "", typeof(float)) },
            { 0x00F23035, new("WSWeapon", "", typeof(float)) },
            { 0x0008DD0E, new("WSWeapon", "", typeof(float)) },
            { 0x001223FE, new("WSWeapon", "", typeof(float)) },
            { 0x04AD1998, new("WSWeapon", "", typeof(bool)) },
            { 0x0266D69F, new("WSWeapon", "", typeof(float)) },
            { 0x039589BC, new("WSWeapon", "DisableAIGrenades?", typeof(bool)) },
            { 0x04E3420D, new("WSWeapon", "", typeof(Crc)) },
            { 0x2FD5EE93, new("WSWeapon", "", typeof(float)) },
            { 0x31E23AD4, new("WSWeapon", "SoundVarCount?", typeof(int)) },
            { 0x33202768, new("WSWeapon", "IsSightedArtillery?", typeof(bool)) },
            { 0x2FB64EFC, new("WSWeapon", "WindDownTime?", typeof(float)) },
            { 0x2F5D8019, new("WSWeapon", "", typeof(FloatRange2)) },
            { 0x2E022594, new("WSWeapon", "", typeof(float)) },
            { 0x2CAA4ACE, new("WSWeapon", "", typeof(float)) },
            { 0x2DD68A03, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0x2A22FDC1, new("WSWeapon", "", typeof(float)) },
            { 0x26AE7598, new("WSWeapon", "", typeof(float)) },
            { 0x2652FAE8, new("WSWeapon", "", typeof(float)) },
            { 0x20C14C6D, new("WSWeapon", "AIMedAdjustTime", typeof(float)) },
            { 0x1724437A, new("WSWeapon", "", typeof(float)) },
            { 0x1B4B7AF6, new("WSWeapon", "", typeof(float)) },
            { 0x277B8B55, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0x279A36E1, new("WSWeapon", "HideAfterFire?", typeof(bool)) },
            { 0x29C2A39C, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0x64F0F584, new("WSWeapon", "", typeof(int)) },
            { 0x5CD15632, new("WSWeapon", "BashForce?", typeof(float)) },
            { 0x63468C4B, new("WSWeapon", "", typeof(float)) },
            { 0x63FF9380, new("WSWeapon", "RemoveWhenEmpty?", typeof(bool)) },
            { 0x56D6092E, new("WSWeapon", "", typeof(float)) },
            { 0x560A68C4, new("WSWeapon", "", typeof(int)) },
            { 0x544D5A73, new("WSWeapon", "SpreadIncreasePerShot?", typeof(float)) },
            { 0x4E13EA36, new("WSWeapon", "SightedMultiplier?", typeof(float)) },
            { 0x52619D52, new("WSWeapon", "", typeof(float)) },
            { 0x498C371F, new("WSWeapon", "AICloseInaccuracyMin/Max?", typeof(FloatRange2)) },
            { 0x3BF1CDB6, new("WSWeapon", "", typeof(int)) },
            { 0x379CD846, new("WSWeapon", "", typeof(float)) },
            { 0x375004CF, new("WSWeapon", "HorzAimPreference?", typeof(float)) },
            { 0x36A350AD, new("WSWeapon", "VertAimPreference?", typeof(float)) },
            { 0x36F01F83, new("WSWeapon", "", typeof(float)) },
            { 0x491A1285, new("WSWeapon", "AudibleRadius?", typeof(float)) },
            { 0x44297E86, new("WSWeapon", "", typeof(byte)) },
            { 0x3C967A20, new("WSWeapon", "", typeof(int)) },
            { 0x3DE8A21D, new("WSWeapon", "LowResSoundBank?", typeof(string)) },
            { 0x797A84EF, new("WSWeapon", "", typeof(float)) },
            { 0x7A909FD2, new("WSWeapon", "RateOfFire?", typeof(float)) },
            { 0x7B7A641C, new("WSWeapon", "", typeof(float)) },
            { 0x78FA7E87, new("WSWeapon", "AimBoxWidth?", typeof(float)) },
            { 0x76ECC8CD, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0x74BEF49A, new("WSWeapon", "", typeof(int)) },
            { 0x761C3A09, new("WSWeapon", "WindUpTime?", typeof(float)) },
            { 0x7777D77E, new("WSWeapon", "", typeof(Crc)) },
            { 0x73A9FAE3, new("WSWeapon", "TargetSeekSpeed?", typeof(float)) },
            { 0x6DB7B63D, new("WSWeapon", "", typeof(int)) },
            { 0x71DFBAEE, new("WSWeapon", "CrossHairs?", typeof(Crc)) },
            { 0x71E30E69, new("WSWeapon", "", typeof(float)) },
            { 0x6CA0E2FE, new("WSWeapon", "", typeof(FloatRange4)) },
            { 0x6C93DEA3, new("WSWeapon", "vertRecoilRange?", typeof(FloatRange2)) },
            { 0x69059D08, new("WSWeapon", "", typeof(bool)) },
            { 0x69DCA2B6, new("WSWeapon", "", typeof(int)) },
            { 0x6B484076, new("WSWeapon", "", typeof(Crc)) },
            { 0xBC58AA53, new("WSWeapon", "HighResSoundBank?", typeof(string)) },
            { 0xA56EE87E, new("WSWeapon", "DisableAICover?", typeof(bool)) },
            { 0xA3868095, new("WSWeapon", "", typeof(float)) },
            { 0xA3C83F69, new("WSWeapon", "", typeof(bool)) },
            { 0xA4CA8665, new("WSWeapon", "CrossHairScale?", typeof(float)) },
            { 0xA3416E0A, new("WSWeapon", "IsArtillery?", typeof(bool)) },
            { 0xA1953CD0, new("WSWeapon", "AimBoxHeight?", typeof(float)) },
            { 0xA0C571DD, new("WSWeapon", "JogSpread?", typeof(float)) },
            { 0x9737DC5B, new("WSWeapon", "", typeof(FloatRange2)) },
            { 0x9B889E4A, new("WSWeapon", "Automatic", typeof(bool)) },
            { 0x96DC09BD, new("WSWeapon", "ReloadTime?", typeof(float)) },
            { 0x95F2D6BB, new("WSWeapon", "", typeof(bool)) },
            { 0x94FCF44A, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0x8FABBD6C, new("WSWeapon", "", typeof(float)) },
            { 0x94B5AED3, new("WSWeapon", "CameraFireMovementScale?", typeof(float)) },
            { 0x8EB3B1A4, new("WSWeapon", "", typeof(float)) },
            { 0x89341658, new("WSWeapon", "MinRange?", typeof(float)) },
            { 0x865EADCB, new("WSWeapon", "", typeof(float)) },
            { 0x7DB30B59, new("WSWeapon", "", typeof(int)) },
            { 0x85924A26, new("WSWeapon", "OptimalRange", typeof(float)) },
            { 0xB344CC74, new("WSWeapon", "AIMedRangeMax?", typeof(float)) },
            { 0xB182906E, new("WSWeapon", "DistAimPreference?", typeof(float)) },
            { 0xAE28D6E9, new("WSWeapon", "", typeof(float)) },
            { 0xAD451485, new("WSWeapon", "MaxBloomSpread?", typeof(float)) },
            { 0xADDEBDCA, new("WSWeapon", "", typeof(Crc)) },
            { 0xAB3BE1A0, new("WSWeapon", "StartingClips?", typeof(int)) },
            { 0xA8AB10E2, new("WSWeapon", "AICloseRangeMax?", typeof(float)) },
            { 0xA75D5327, new("WSWeapon", "", typeof(int)) },
            { 0xA5BC45E2, new("WSWeapon", "Presence?", typeof(float)) },
            { 0xA6107C89, new("WSWeapon", "horzRecoilRange", typeof(FloatRange2)) },
            { 0xB838A7E9, new("WSWeapon", "", typeof(FloatRange2)) },
            { 0xB4D334D8, new("WSWeapon", "", typeof(bool)) },
            { 0xB4685717, new("WSWeapon", "Audible:unk?", typeof(int)) },
            { 0xB4C6842E, new("WSWeapon", "MaxRange?", typeof(float)) },
            { 0xB5C71325, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0xB8627450, new("WSWeapon", "AIFarAdjustTime?", typeof(float)) },
            { 0xB91AF09E, new("WSWeapon", "", typeof(Crc)) },
            { 0xBA157547, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0xEC438E17, new("WSWeapon", "MuzzleFlashTime", typeof(float)) },
            { 0xEBB3F257, new("WSWeapon", "", typeof(float)) },
            { 0xE8E01A04, new("WSWeapon", "MuzzleFlashIndex?", typeof(int)) },
            { 0xE545FBF4, new("WSWeapon", "", typeof(bool)) },
            { 0xE79E08FE, new("WSWeapon", "", typeof(float)) },
            { 0xE3945C1D, new("WSWeapon", "AIMedInaccuracyMin/Max?", typeof(FloatRange2)) },
            { 0xE1F8DAFC, new("WSWeapon", "", typeof(bool)) },
            { 0xE0AC4345, new("WSWeapon", "", typeof(float)) },
            { 0xDDF78D47, new("WSWeapon", "", typeof(float)) },
            { 0xE0428E0D, new("WSWeapon", "", typeof(float)) },
            { 0xFA9AD969, new("WSWeapon", "", typeof(float)) },
            { 0xFE9C1EEB, new("WSWeapon", "SpreadDecreasePerSec?", typeof(float)) },
            { 0xF8F8275D, new("WSWeapon", "BaseAIAccuracy?", typeof(float)) },
            { 0xF766AA6F, new("WSWeapon", "AIBurstDuration?", typeof(float)) },
            { 0xF535722A, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0xEFED376D, new("WSWeapon", "", typeof(float)) },
            { 0xF4162380, new("WSWeapon", "", typeof(Crc)) },
            { 0xF20499A6, new("WSWeapon", "SightedCrossHairs?", typeof(Crc)) },
            { 0xDC9E428F, new("WSWeapon", "WalkSpread?", typeof(float)) },
            { 0xD43A1FE1, new("WSWeapon", "AIFarRangeMax?", typeof(float)) },
            { 0xD850F531, new("WSWeapon", "", typeof(int)) },
            { 0xDB4B8095, new("WSWeapon", "", typeof(float)) },
            { 0xD3F2828B, new("WSWeapon", "", typeof(bool)) },
            { 0xCE9447BC, new("WSWeapon", "", typeof(float)) },
            { 0xD01F6168, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0xD1B71AE8, new("WSWeapon", "BuddyItemName?", typeof(Crc)) },
            { 0xD1DD6599, new("WSWeapon", "Audible:unk?", typeof(Crc)) },
            { 0xCE4384BF, new("WSWeapon", "", typeof(FloatRange2)) },
            { 0xCA55EEC9, new("WSWeapon", "", typeof(byte)) },
            { 0xC882AF1A, new("WSWeapon", "", typeof(float)) },
            { 0xC8E09549, new("WSWeapon", "PelletsPerShot?", typeof(int)) },
            { 0xCAF505EA, new("WSWeapon", "", typeof(Crc)) },
            { 0xC6300246, new("WSWeapon", "", typeof(float)) },
            { 0xC451AB8D, new("WSWeapon", "SpreadMultRange?", typeof(FloatRange2)) },
            { 0xBD8F9C00, new("WSWeapon", "", typeof(float)) },
            { 0xBFA35CB2, new("WSWeapon", "AIFarInaccuracyMin/Max?", typeof(FloatRange2)) },
            { 0xC40ED65B, new("WSWeapon", "", typeof(int)) },
        };
    }
}
