﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SabTool.Containers.GameTemplates
{
    using Client.Blueprint;
    using Dump;
    using Utils;

    public class GameTemplatesDump : GameTemplatesBase
    {
        private const int NamePad = -30;

        private TextWriter Output { get; }

        static GameTemplatesDump()
        {
            var toAdd = new HashSet<string>();

            while (true)
            {
                var didAnything = false;

                foreach (var entry in Parents)
                {
                    foreach (var parent in entry.Value)
                    {
                        if (!Parents.ContainsKey(parent))
                            continue;

                        foreach (var parentVal in Parents[parent])
                        {
                            toAdd.Add(parentVal);
                        }
                    }

                    var startCount = entry.Value.Count;

                    foreach (var newEntry in toAdd)
                    {
                        entry.Value.Add(newEntry);
                    }

                    toAdd.Clear();

                    didAnything = didAnything || startCount != entry.Value.Count;
                }

                if (!didAnything)
                    break;
            }
        }

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

            HashSet<string> parents = null;

            foreach (var entry in Parents)
            {
                if (entry.Key.ToLowerInvariant() == type.ToLowerInvariant())
                {
                    parents = entry.Value;
                    break;
                }
            }

            if (parents == null)
            {
                Output.WriteLine("Unknown blueprint type!");
            }

            while (reader.Offset < reader.Size)
            {
                var crc = reader.ReadUInt();
                var size = reader.ReadInt();

                // Store the starting position
                var startOff = reader.Offset;

                // Read the property
                ReadProperty(reader, type, crc, size, parents);

                // Assign the valid offset to the reader, even if the ReadProperty read too little or too much
                reader.Offset = startOff + size;
            }

            Output.WriteLine();
        }

        private void ReadProperty(BluaReader reader, string type, uint crc, int size, HashSet<string> parents)
        {
            var typeCrc = Hash.FNV32string(type);

            var name = Hash.HashToString(crc);

            TypeEntry entry;

            if (!PropertyTypes.TryGetValue(new(typeCrc, crc), out entry) && parents != null)
            {
                foreach (var parent in parents)
                {
                    if (PropertyTypes.TryGetValue(new(Hash.FNV32string(parent), crc), out entry))
                    {
                        type = parent;
                        break;
                    }
                }
            }

            if (entry != null)
            {
                var propName = entry.OverrideName;

                if (!string.IsNullOrEmpty(name))
                {
                    if (string.IsNullOrEmpty(propName))
                        propName = name;
                    else
                        propName = $"{propName} ({name})";
                }

                if (string.IsNullOrEmpty(propName))
                    propName = "UNKNOWN";

                Output.WriteLine($"[0x{crc:X8}][{type,NamePad}][{propName,NamePad}]: {ReadPropertyValue(reader, entry.Type, size)}");
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    name = "UNKNOWN";

                var bytes = reader.ReadBytes(size);

                Output.WriteLine($"[0x{crc:X8}][{type,NamePad}][{name,NamePad}]: {FormatEmptyType(bytes)}");
            }
        }

        private static string FormatEmptyType(byte[] bytes)
        {
            var bytesStr = BitConverter.ToString(bytes).Replace('-', ' ');

            string guessStr = "";
            if (bytes.Length >= 4)
            {
                var intStr = bytes.Length >= 4 ? BitConverter.ToInt32(bytes, 0).ToString() : "";
                var floatStr = bytes.Length >= 4 ? BitConverter.ToSingle(bytes, 0).ToString() : "";
                var stringVal = Encoding.UTF8.GetString(bytes);
                var crcVal = bytes.Length >= 4 ? new Crc(BitConverter.ToUInt32(bytes, 0)).ToString() : "";

                if (!stringVal.All(c => char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)))
                    stringVal = null;

                if (string.IsNullOrEmpty(stringVal))
                {
                    guessStr = $" (I: {intStr,-15} | F: {floatStr,-15} | Crc: {crcVal})";
                }
                else
                {
                    guessStr = $" (I: {intStr,-15} | F: {floatStr,-15} | S: {stringVal,-15} | Crc: {crcVal})";
                }
            }

            return $"{bytesStr,-30}{guessStr}";
        }

        private static object ReadPropertyValue(BluaReader reader, Type type, int size)
        {
            if (type == null)
                return FormatEmptyType(reader.ReadBytes(size));

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
                "FloatVector2" => new FloatVector2(reader.ReadFloat(), reader.ReadFloat()),
                "FloatVector3" => new FloatVector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                "FloatVector4" => new FloatVector4(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                "Color" => new Color(reader.ReadInt()),
                _ => null
            };
        }

        record TypeCrcKey(uint TypeCrc, uint Crc);
        record TypeEntry(string OverrideName, Type Type);
        record FloatVector2(float Val1, float Val2)
        {
            public override string ToString()
            {
                return $"({Val1}, {Val2})";
            }
        }
        record FloatVector3(float Val1, float Val2, float Val3);
        record FloatVector4(float Val1, float Val2, float Val3, float Val4)
        {
            public override string ToString()
            {
                return $"({Val1}, {Val2}, {Val3}, {Val4})";
            }
        }
        record Color(int Value)
        {
            public override string ToString()
            {
                return $"(R: {(Value >> 24) & 0xFF:X2}, G: {(Value >> 16) & 0xFF:X2}, B: {(Value >> 8) & 0xFF:X2}, A: {Value & 0xFF:X2})";
            }
        }

        // Damageable, Audible, ModelRenderable, Targetable, AIAttractionPt, Controllable
        private static Dictionary<string, HashSet<string>> Parents { get; } = new()
        {
            { "Weapon", new() { "Item", "Damageable", "Audible" } },
            { "Searcher", new() { "Item", "Damageable", "Audible" } },
            { "MeleeWeapon", new() { "Item" } },
            { "Ammo", new() { "Item" } },
            { "bullet", new() { "Ordnance" } },
            { "FlameOrdnance", new() { "Ordnance" } },
            { "PhysicalOrdnance", new() { "Ordnance", "Targetable", "Audible" } },
            { "Rocket", new() { "Ordnance", "Audible" } },
            { "HumanPhysics", new() { } },
            { "Player", new() { } },
            { "Car", new() { } },
            { "Truck", new() { } },
            { "APC", new() { } },
            { "Tank", new() { } },
            { "Human", new() { } },
            { "Spore", new() { } },
            { "VehicleCollision", new() { } },
            { "PlayerCollision", new() { } },
            { "Targetable", new() { } },
            { "Prop", new() { } },
            { "TrainList", new() { } },
            { "Train", new() { } },
            { "Foliage", new() { } },
            { "AIAttractionPt", new() { } },
            { "DamageSphere", new() { } },
            { "Explosion", new() { } },
            { "AIController", new() { } },
            { "ScriptController", new() { } },
            { "AISpawner", new() { } },
            { "AICombatParams", new() { } },
            { "AICrowdBlocker", new() { } },
            { "GlobalHumanParams", new() { } },
            { "Melee", new() { } },
            { "ParticleEffect", new() { } },
            { "RadialBlur", new() { } },
            { "Sound", new() { } },
            { "Turret", new() { } },
            { "SearchTurret", new() { } },
            { "TrainCarriage", new() { } },
            { "TrainEngine", new() { } },
            { "TrainItem", new() { } },
            { "SlowMotionCamera", new() { } },
            { "ElasticTransition", new() { } },
            { "AnimatedTransition", new() { } },
            { "GroupTransition", new() { } },
            { "ScopeTransition", new() { } },
            { "CameraSettings", new() { } },
            { "GroupCameraSettings", new() { } },
            { "CameraSettingsMisc", new() { } },
            { "ToneMapping", new() { } },
            { "LightSettings", new() { } },
            { "LightHalo", new() { } },
            { "LightVolume", new() { } },
            { "ClothObject", new() { } },
            { "ClothForce", new() { } },
            { "WillToFight", new() { } },
            { "WillToFightNode", new() { } },
            { "HealthEffectFilter", new() { } },
            { "ParticleEffectSpawner", new() { } },
            { "ButtonPrompt", new() { } },
            { "Item", new() { "LightAttachment", "LightHaloAttachment" } },
            { "FxBoneStateList", new() { } },
            { "FxHumanHead", new() { } },
            { "FxHumanBodyPart", new() { } },
            { "FxHumanBodySetup", new() { } },
            { "FaceExpression", new() { } },
            { "HumanBodyPart", new() { } },
            { "HumanBodySetup", new() { } },
            { "HumanSkeletonScale", new() { } },
            { "AnimatedObject", new() { } },
            { "RandomObj", new() { } },
            { "BridgeController", new() { } },
            { "Ricochet", new() { } },
            { "AIRoad", new() { } },
            { "Highlight", new() { } },
            { "MiniGame", new() { } },
            { "SabotageTarget", new() { } },
            { "BigMap", new() { } },
            { "FlashMovie", new() { } },
            { "ItemCache", new() { } },
            { "VirVehicleWheel", new() { } },
            { "VirVehicleTransmission", new() { } },
            { "VirVehicleEngine", new() { } },
            { "VirVehicleChassis", new() { } },
            { "VirVehicleSetup", new() { } },
            { "VehicleWheelFX", new() { } },
            { "Difficulty", new() { } },
            { "Shop", new() { } },
            { "Perks", new() { } },
            { "PerkFactors", new() { } },
            { "PhysicsParticleSet", new() { } },
            { "PhysicsParticle", new() { } },
            { "Decal", new() { } },
            { "DetailObject", new() { } },
            { "CivilianProp", new() { } },
            { "Bird", new() { } },
            { "Escalation", new() { } },
            { "EscHWTF", new() { } },
            { "AIChatterSet", new() { } },
            { "AIChatter", new() { } },
            { "SeatAnimations", new() { } },
            { "SeatAnimationsPassenger", new() { } },
            { "SeatAnimationsGunner", new() { } },
            { "SeatAnimationsSearcher", new() { } },
            { "SeatAnimationsDriver", new() { } },
            { "FoliageFx", new() { } },
            { "WaterController", new() { } },
            { "WaterTexture", new() { } },
            { "LeafSpawner", new() { } },
            { "VerletBoneObject", new() { } },
            { "WaterParticleFx", new() { } },
            { "Cinematics", new() { } },
            { "Rumble", new() { } },
            { "ImageFolder", new() { } },
            { "CreditName", new() { } },
            //{ "CommonUI_Persistent", new() { } },
            //{ "Common", new() { } },
            //{ "SingleImage", new() { } },
            //{ "InteriorImages", new() { } },
            //{ "AIPathPt", new() { } },

            { "Damageable", new() { "DamageableRoot" } }
        };

        private static Dictionary<TypeCrcKey, TypeEntry> PropertyTypes { get; } = new()
        {
            // Common
            { new(0x2447DFFA, 0x6302F1CC), new(null, typeof(bool)) },
            { new(0x2447DFFA, 0x8C9928FB), new(null, typeof(bool)) },
            { new(0x2447DFFA, 0x87519019), new(null, typeof(int)) },
            { new(0x2447DFFA, 0xBF83D3AF), new(null, typeof(Crc)) },
            { new(0x2447DFFA, 0x0C2DB3B4), new(null, typeof(bool)) },

            // Damageable

            // Audible
            { new(0xCC1EE14D, 0xCC1EE14D), new(null, typeof(Crc)) },
            { new(0xCC1EE14D, 0x42C6DA9D), new(null, typeof(int)) },
            { new(0xCC1EE14D, 0x246DB06C), new(null, typeof(Crc)) },
            { new(0xCC1EE14D, 0x3217CDF0), new(null, typeof(float)) },
            { new(0xCC1EE14D, 0x212A9DB0), new(null, typeof(bool)) },
            { new(0xCC1EE14D, 0x02D3EB9F), new(null, typeof(Crc)) },
            { new(0xCC1EE14D, 0x04FBB1EE), new(null, typeof(string)) },
            { new(0xCC1EE14D, 0x1E59E605), new(null, typeof(bool)) },
            { new(0xCC1EE14D, 0xD76D7747), new(null, typeof(Crc)) },
            { new(0xCC1EE14D, 0x85B86BD7), new(null, typeof(bool)) },
            { new(0xCC1EE14D, 0xAFCE4BA2), new(null, typeof(int)) },
            { new(0xCC1EE14D, 0xEC8964B3), new(null, typeof(int)) },
            { new(0xCC1EE14D, 0xE94E8608), new(null, typeof(int)) },

            // ModelRenderable
            { new(0x38D88BB8, 0x5B724250), new(null, typeof(Crc)) },
            { new(0x38D88BB8, 0xAE1ED17F), new(null, typeof(bool)) },

            // Targetable

            // AIAttractionPt

            // Controllable
            { new(0x2C70C910, 0xFB31F1EF), new(null, typeof(Crc)) },
            { new(0x2C70C910, 0x404D1343), new(null, typeof(Crc)) },
            { new(0x2C70C910, 0xDB0F705C), new(null, typeof(LuaParam)) },

            // 0x194 SetProperty
            //{ , new("", null, typeof()) },



            // AIAttractionPt
            { new(0x92473A24, 0x7EF2B668), new(null, typeof(bool)) },
            { new(0x92473A24, 0x7E4E77E0), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x7ED98262), new(null, typeof(float)) },
            { new(0x92473A24, 0xE773FC1F), new(null, typeof(bool)) },
            { new(0x92473A24, 0xE29D36B0), new(null, typeof(int)) },
            { new(0x92473A24, 0xE66CED59), new(null, typeof(bool)) },
            { new(0x92473A24, 0xE1291EAE), new(null, typeof(float)) },
            { new(0x92473A24, 0xD411C23C), new(null, typeof(bool)) },
            { new(0x92473A24, 0xDE634E2F), new(null, typeof(bool)) },
            { new(0x92473A24, 0xDF0B2054), new(null, typeof(float)) },
            { new(0x92473A24, 0xCF88DE36), new(null, typeof(float)) },
            { new(0x92473A24, 0xD235DA20), new(null, typeof(int)) },
            { new(0x92473A24, 0xCE1A4A79), new(null, typeof(int)) },
            { new(0x92473A24, 0xC6E0F627), new(null, typeof(double)) },
            { new(0x92473A24, 0xC8DDD177), new(null, typeof(float)) },
            { new(0x92473A24, 0xCBD8745E), new(null, typeof(float)) },
            { new(0x92473A24, 0xD27568AD), new(null, typeof(Crc)) },
            { new(0x92473A24, 0xF01F08A8), new(null, typeof(int)) },
            { new(0x92473A24, 0xEA73DB2F), new(null, typeof(bool)) },
            { new(0x92473A24, 0xECF2E7AE), new(null, typeof(float)) },
            { new(0x92473A24, 0xE9CACFF7), new(null, typeof(bool)) },
            { new(0x92473A24, 0xE7B1D609), new(null, typeof(Crc)) },
            { new(0x92473A24, 0xE94E8608), new(null, typeof(int)) },
            { new(0x92473A24, 0xE9A5BC9C), new(null, typeof(bool)) },
            { new(0x92473A24, 0xFB14F042), new(null, typeof(float)) },
            { new(0x92473A24, 0xFB31F1EF), new(null, typeof(int)) },
            { new(0x92473A24, 0xF8AFE581), new(null, typeof(int)) },
            { new(0x92473A24, 0xF0C7DA5F), new(null, typeof(string)) },
            { new(0x92473A24, 0xF10C031B), new(null, typeof(int)) },
            { new(0x92473A24, 0xF49F0182), new(null, typeof(float)) },
            { new(0x92473A24, 0xC54E8CAE), new(null, typeof(bool)) },
            { new(0x92473A24, 0x9EDB6BD9), new(null, typeof(int)) },
            { new(0x92473A24, 0x99A54CBF), new(null, typeof(bool)) },
            { new(0x92473A24, 0x9AAC5788), new(null, typeof(float)) },
            { new(0x92473A24, 0x982589A0), new(null, typeof(byte[])) },
            { new(0x92473A24, 0x923BA76B), new(null, typeof(byte[])) },
            { new(0x92473A24, 0x92D0A9BA), new(null, typeof(float)) },
            { new(0x92473A24, 0x97AC8632), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x8FD2512F), new(null, typeof(bool)) },
            { new(0x92473A24, 0x8C8D6435), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x8D89FAD9), new(null, typeof(bool)) },
            { new(0x92473A24, 0x8DB13A88), new(null, typeof(int)) },
            { new(0x92473A24, 0x888505C3), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x07F79C5D), new(null, typeof(float)) },
            { new(0x92473A24, 0x84C77E96), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x84D70C5E), new(null, typeof(int)) },
            { new(0x92473A24, 0xB3AF2A59), new(null, typeof(bool)) },
            { new(0x92473A24, 0xA8DB360C), new(null, typeof(int)) },
            { new(0x92473A24, 0xAEEA42F8), new(null, typeof(Crc)) },
            { new(0x92473A24, 0xB2E588D7), new(null, typeof(double)) },
            { new(0x92473A24, 0xB40462AD), new(null, typeof(int)) },
            { new(0x92473A24, 0xC1D0622B), new(null, typeof(float)) },
            { new(0x92473A24, 0xA8079DD8), new(null, typeof(double)) },
            { new(0x92473A24, 0xA5A71B8A), new(null, typeof(float)) },
            { new(0x92473A24, 0xA7E0D198), new(null, typeof(Crc)) },
            { new(0x92473A24, 0xA3FBC25C), new(null, typeof(bool)) },
            { new(0x92473A24, 0x9FDC0AE6), new(null, typeof(int)) },
            { new(0x92473A24, 0xA0A39AA4), new(null, typeof(float)) },
            { new(0x92473A24, 0xA2FF7739), new(null, typeof(float)) },
            { new(0x92473A24, 0x7E1D5456), new(null, typeof(bool)) },
            { new(0x92473A24, 0x658D48E2), new(null, typeof(int)) },
            { new(0x92473A24, 0x506DE042), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x4785F2A3), new(null, typeof(int)) },
            { new(0x92473A24, 0x4A67BCC0), new(null, typeof(int)) },
            { new(0x92473A24, 0x4D44EFE0), new(null, typeof(int)) },
            { new(0x92473A24, 0x47751B18), new(null, typeof(float)) },
            { new(0x92473A24, 0x4172D6FB), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x41B4ACE8), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x460E7F77), new(null, typeof(bool)) },
            { new(0x92473A24, 0x55D89223), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x507A477F), new(null, typeof(bool)) },
            { new(0x92473A24, 0x5253FC93), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x530D6414), new(null, typeof(bool)) },
            { new(0x92473A24, 0x5E58EAC8), new(null, typeof(int)) },
            { new(0x92473A24, 0x5E77F41F), new(null, typeof(float)) },
            { new(0x92473A24, 0x733ED3B2), new(null, typeof(int)) },
            { new(0x92473A24, 0x6C6A0DB8), new(null, typeof(int)) },
            { new(0x92473A24, 0x6E15FFFF), new(null, typeof(float)) },
            { new(0x92473A24, 0x726F6DCC), new(null, typeof(bool)) },
            { new(0x92473A24, 0x6BE83213), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x67277C2C), new(null, typeof(bool)) },
            { new(0x92473A24, 0x6935C0FD), new(null, typeof(float)) },
            { new(0x92473A24, 0x6A099303), new(null, typeof(float)) },
            { new(0x92473A24, 0x7C0D1F8E), new(null, typeof(bool)) },
            { new(0x92473A24, 0x7CD0DF87), new(null, typeof(string)) },
            { new(0x92473A24, 0x7A3E99B4), new(null, typeof(int)) },
            { new(0x92473A24, 0x7794DACB), new(null, typeof(bool)) },
            { new(0x92473A24, 0x787A47A5), new(null, typeof(float)) },
            { new(0x92473A24, 0x7A157568), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x4070B35E), new(null, typeof(bool)) },
            { new(0x92473A24, 0x239B1E1B), new(null, typeof(bool)) },
            { new(0x92473A24, 0x0F12604C), new(null, typeof(float)) },
            { new(0x92473A24, 0x06273164), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x07AD6EBF), new(null, typeof(float)) },
            { new(0x92473A24, 0x09E38146), new(null, typeof(float)) },
            { new(0x92473A24, 0x054A8FB1), new(null, typeof(float)) },
            { new(0x92473A24, 0x019EF97E), new(null, typeof(bool)) },
            { new(0x92473A24, 0x01D505A2), new(null, typeof(bool)) },
            { new(0x92473A24, 0x04FBB1EE), new(null, typeof(string)) },
            { new(0x92473A24, 0x1DE59C2F), new(null, typeof(bool)) },
            { new(0x92473A24, 0x16930AFE), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x192A3633), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x1B78F12E), new(null, typeof(int)) },
            { new(0x92473A24, 0x1DE5C824), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x23518B03), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x3DCC7355), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x3DFE6182), new(null, typeof(int)) },
            { new(0x92473A24, 0x397B6A20), new(null, typeof(bool)) },
            { new(0x92473A24, 0x331D2AA8), new(null, typeof(bool)) },
            { new(0x92473A24, 0x3566AE45), new(null, typeof(float)) },
            { new(0x92473A24, 0x365FB660), new(null, typeof(bool)) },
            { new(0x92473A24, 0x32D97474), new(null, typeof(float)) },
            { new(0x92473A24, 0x2D5B659B), new(null, typeof(bool)) },
            { new(0x92473A24, 0x2ED2EC21), new(null, typeof(bool)) },
            { new(0x92473A24, 0x3217CDF0), new(null, typeof(float)) },
            { new(0x92473A24, 0x2C652AF7), new(null, typeof(Crc)) },
            { new(0x92473A24, 0x2523B4EE), new(null, typeof(bool)) },
            { new(0x92473A24, 0x29EC9F57), new(null, typeof(bool)) },
            { new(0x92473A24, 0x2A4802C1), new(null, typeof(bool)) },

            // ScriptController
            { new(0x7A6E9D28, 0xE0B01D9F), new(null, typeof(string)) },
            { new(0x7A6E9D28, 0xCA6B9057), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0xC072E94A), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0xC4C412CD), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0xBF930289), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0xBCFE6314), new(null, typeof(string)) },
            { new(0x7A6E9D28, 0xBE5952B1), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0xAE350719), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0xA3DBF09D), new(null, typeof(bool)) },
            { new(0x7A6E9D28, 0xA77EDAAA), new(null, typeof(bool)) },
            { new(0x7A6E9D28, 0x97EE12C6), new(null, typeof(bool)) },
            { new(0x7A6E9D28, 0x7A2AA5EF), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0x85B5E29F), new(null, typeof(string)) },
            { new(0x7A6E9D28, 0x89A84EF5), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0xF4BADFDB), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0xE4E34549), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0xF6B4FF0E), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0xFE8BAE82), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0xDDF1EE0F), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0xD72AA401), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0xCACFD6AA), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0xCE629E7E), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0xD6F4D903), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0xD730D0E1), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0xD79C2BE9), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0x77776D5A), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0x7154DC63), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0x722C0953), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0x6A9AB174), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0x4177A478), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0x44885A78), new(null, typeof(bool)) },
            { new(0x7A6E9D28, 0x37FC008D), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0x2F1820CC), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0x3570F09D), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0x19BAB4C9), new(null, typeof(float)) },
            { new(0x7A6E9D28, 0x0424DD32), new(null, typeof(bool)) },
            { new(0x7A6E9D28, 0x07B13063), new(null, typeof(int)) },
            { new(0x7A6E9D28, 0x165C1FD7), new(null, typeof(int)) },

            // WSDamageable
            { new(0x6068DB46, 0xB37BF5B2), new(null, typeof(float)) },
            { new(0x6068DB46, 0x97E0DB0F), new(null, typeof(float)) },
            { new(0x6068DB46, 0xA1B82BA7), new(null, typeof(float)) },
            { new(0x6068DB46, 0xAC0F66B8), new(null, typeof(float)) },
            { new(0x6068DB46, 0x08ECB69D), new(null, typeof(float)) },
            { new(0x6068DB46, 0x8CBC442D), new(null, typeof(float)) },
            { new(0x6068DB46, 0x82E62211), new(null, typeof(Crc)) },
            { new(0x6068DB46, 0x7C203814), new(null, typeof(float)) },
            { new(0x6068DB46, 0x81CD9A01), new(null, typeof(float)) },
            { new(0x6068DB46, 0xDD7EAE2C), new(null, typeof(float)) },
            { new(0x6068DB46, 0xDA3A3781), new(null, typeof(float)) },
            { new(0x6068DB46, 0xD0E3A1A1), new(null, typeof(float)) },
            { new(0x6068DB46, 0xC11C102E), new(null, typeof(float)) },
            { new(0x6068DB46, 0xC86877FD), new(null, typeof(int)) },
            { new(0x6068DB46, 0xF7C72618), new(null, typeof(float)) },
            { new(0x6068DB46, 0xF9C458E7), new(null, typeof(float)) },
            { new(0x6068DB46, 0xFD4869CD), new(null, typeof(FloatVector2)) },
            { new(0x6068DB46, 0x75791974), new(null, typeof(float)) },
            { new(0x6068DB46, 0x55817592), new(null, typeof(int)) },
            { new(0x6068DB46, 0x6AC12B1E), new(null, typeof(float)) },
            { new(0x6068DB46, 0x73FF7621), new(null, typeof(float)) },
            { new(0x6068DB46, 0x54019561), new(null, typeof(float)) },
            { new(0x6068DB46, 0x4F18FA7A), new(null, typeof(float)) },
            { new(0x6068DB46, 0x496A25D2), new(null, typeof(byte)) },
            { new(0x6068DB46, 0x49C74990), new(null, typeof(int)) },
            { new(0x6068DB46, 0x4B186A63), new(null, null) },
            { new(0x6068DB46, 0x416E074F), new(null, typeof(float)) },
            { new(0x6068DB46, 0x2D96C4F2), new(null, typeof(float)) },
            { new(0x6068DB46, 0x2A3029F3), new(null, typeof(float)) },
            { new(0x6068DB46, 0x24E96E81), new(null, typeof(float)) },
            { new(0x6068DB46, 0x0E29263B), new(null, typeof(float)) },
            { new(0x6068DB46, 0x12066797), new(null, typeof(float)) },
            { new(0x6068DB46, 0x3121EECD), new(null, typeof(Crc)) },
            { new(0x6068DB46, 0x3D960EA8), new(null, typeof(float)) },
            { new(0x6068DB46, 0x414631B3), new(null, typeof(int)) },

            // DamageRegion
            { new(0xC87F672A, 0x31C25320), new(null, typeof(float)) },
            { new(0xC87F672A, 0x216E8465), new(null, typeof(float)) },
            { new(0xC87F672A, 0x28E10525), new(null, typeof(Crc)) },
            { new(0xC87F672A, 0x3F955DB8), new(null, typeof(float)) },
            { new(0xC87F672A, 0x848096A5), new(null, typeof(Crc)) },

            // DamageSphere
            { new(0x6898A77D, 0x354694DB), new(null, typeof(float)) },
            { new(0x6898A77D, 0x7617FD5B), new(null, typeof(float)) },

            // EventConversation

            // FxHumanBodyPart
            { new(0x08271F91, 0x679CD9A1), new(null, typeof(int)) },
            { new(0x08271F91, 0x1CBA8055), new(null, null) },
            { new(0x08271F91, 0x05FBAB16), new(null, typeof(Crc)) },
            { new(0x08271F91, 0x18281B3B), new(null, typeof(byte)) },
            { new(0x08271F91, 0x3FF4F976), new(null, null) },
            { new(0x08271F91, 0x62404569), new(null, typeof(Crc)) },
            { new(0x08271F91, 0xD9725C55), new(null, typeof(Crc)) },
            { new(0x08271F91, 0xCBA8D62C), new(null, null) },
            { new(0x08271F91, 0xD6A5096B), new(null, typeof(float)) },
            { new(0x08271F91, 0xE5955136), new(null, typeof(int)) },
            { new(0x08271F91, 0xF2F6EAEC), new(null, typeof(Crc)) },

            { new(0xCB248263, 0x725A3658), new(null, typeof(byte)) },

            // Light
            { new(0xC25641DF, 0x76308672), new(null, typeof(byte)) },
            { new(0xC25641DF, 0x456A2D80), new(null, typeof(float)) },
            { new(0xC25641DF, 0x3E6F5C10), new(null, typeof(Color)) },
            { new(0xC25641DF, 0x3FF4F976), new(null, typeof(Color)) },
            { new(0xC25641DF, 0x38D0AD05), new(null, typeof(float)) },
            { new(0xC25641DF, 0x32E060B4), new(null, typeof(Color)) },
            { new(0xC25641DF, 0x354694DB), new(null, typeof(float)) },
            { new(0xC25641DF, 0x35B6696C), new(null, typeof(float)) },
            { new(0xC25641DF, 0x2CFA7B6B), new(null, typeof(float)) },
            { new(0xC25641DF, 0x15667836), new(null, typeof(float)) },
            { new(0xC25641DF, 0x0033984F), new(null, typeof(Color)) },
            { new(0xC25641DF, 0x0EC38247), new(null, typeof(Color)) },
            { new(0xC25641DF, 0x0F2823AC), new(null, typeof(float)) },
            { new(0xC25641DF, 0x1C41B160), new(null, typeof(float)) },
            { new(0xC25641DF, 0x2BF2D35E), new(null, typeof(float)) },
            { new(0xC25641DF, 0x69282DF5), new(null, typeof(byte)) },
            { new(0xC25641DF, 0x5B89B397), new(null, typeof(float)) },
            { new(0xC25641DF, 0x63A27078), new(null, typeof(byte)) },
            { new(0xC25641DF, 0x586891F5), new(null, typeof(float)) },
            { new(0xC25641DF, 0x4BF5F9E3), new(null, typeof(Color)) },
            { new(0xC25641DF, 0x52FCF5D4), new(null, typeof(float)) },
            { new(0xC25641DF, 0x566B3F52), new(null, typeof(int)) },
            { new(0xC25641DF, 0x73E741E3), new(null, typeof(byte)) },
            { new(0xC25641DF, 0x762D7CD9), new(null, typeof(float)) },
            { new(0xC25641DF, 0x73514F39), new(null, typeof(float)) },
            { new(0xC25641DF, 0x6BED3D76), new(null, typeof(Crc)) },
            { new(0xC25641DF, 0x725EAA9B), new(null, typeof(float)) },
            { new(0xC25641DF, 0x726047C6), new(null, typeof(float)) },
            { new(0xC25641DF, 0x7A9D55F3), new(null, typeof(byte)) },
            { new(0xC25641DF, 0x76C3FCF2), new(null, typeof(Color)) },
            { new(0xC25641DF, 0x79AEA90A), new(null, typeof(byte)) },
            { new(0xC25641DF, 0xC61EDBD1), new(null, typeof(float)) },
            { new(0xC25641DF, 0x99327D6F), new(null, typeof(float)) },
            { new(0xC25641DF, 0x917335AD), new(null, typeof(float)) },
            { new(0xC25641DF, 0x976E00A1), new(null, typeof(float)) },
            { new(0xC25641DF, 0x8900B33F), new(null, typeof(float)) },
            { new(0xC25641DF, 0x7CBBCA09), new(null, typeof(Color)) },
            { new(0xC25641DF, 0x836D3DFC), new(null, typeof(float)) },
            { new(0xC25641DF, 0x85A5B4DD), new(null, typeof(float)) },
            { new(0xC25641DF, 0xACFFC219), new(null, typeof(float)) },
            { new(0xC25641DF, 0xA0716ED6), new(null, typeof(float)) },
            { new(0xC25641DF, 0xACDFC789), new(null, typeof(string)) },
            { new(0xC25641DF, 0xAF9F46EA), new(null, typeof(float)) },
            { new(0xC25641DF, 0xB7CFC8DD), new(null, typeof(float)) },
            { new(0xC25641DF, 0xFBABFD4C), new(null, typeof(float)) },
            { new(0xC25641DF, 0xF467D031), new(null, typeof(float)) },
            { new(0xC25641DF, 0xF7688793), new(null, typeof(float)) },
            { new(0xC25641DF, 0xFF244A07), new(null, typeof(Crc)) },
            { new(0xC25641DF, 0xFF27868E), new(null, typeof(bool)) },
            { new(0xC25641DF, 0xF0758D60), new(null, typeof(byte)) },
            { new(0xC25641DF, 0xD7DBF28C), new(null, typeof(byte)) },
            { new(0xC25641DF, 0xE1AE04B8), new(null, typeof(float)) },
            { new(0xC25641DF, 0xD1488AB4), new(null, typeof(int)) },
            { new(0xC25641DF, 0xC8AF0E75), new(null, typeof(Color)) },
            { new(0xC25641DF, 0xCB7BA603), new(null, typeof(float)) },

            // Item
            { new(0x71798A24, 0x953C60F5), new(null, typeof(float)) },
            { new(0x71798A24, 0x8303620E), new(null, typeof(int)) },
            { new(0x71798A24, 0x75060ED9), new(null, typeof(bool)) },
            { new(0x71798A24, 0x6ADE730A), new(null, typeof(Crc)) },
            { new(0x71798A24, 0x70785B2A), new(null, typeof(bool)) },
            { new(0x71798A24, 0x97AC8632), new(null, typeof(int)) },
            { new(0x71798A24, 0xC6676245), new(null, typeof(int)) },
            { new(0x71798A24, 0xF7CEEE0B), new(null, typeof(bool)) },
            { new(0x71798A24, 0x650A47ED), new(null, typeof(bool)) },
            { new(0x71798A24, 0x2D1BCD5A), new(null, typeof(Crc)) },
            { new(0x71798A24, 0x5B724250), new(null, typeof(int)) },
            { new(0x71798A24, 0x5D4608B5), new(null, typeof(bool)) },
            { new(0x71798A24, 0x1E757409), new(null, typeof(Crc)) },
            { new(0x71798A24, 0x13F47172), new(null, typeof(bool)) },
            { new(0x71798A24, 0x0F59CD06), new(null, typeof(Crc)) },
            { new(0x71798A24, 0x06DA8775), new(null, typeof(Crc)) },
            { new(0x71798A24, 0x0D111AB7), new(null, typeof(Crc)) },

            // LightAttachement
            { new(0xA3613F4D, 0xC25641DF), new(null, typeof(int)) },
            { new(0xA3613F4D, 0x9AB4F3ED), new(null, typeof(int)) },
            { new(0xA3613F4D, 0xA3613F4D), new(null, typeof(int)) },
            { new(0xA3613F4D, 0x8ED97E12), new(null, typeof(float)) },
            { new(0xA3613F4D, 0x6C9D982B), new(null, typeof(int)) },
            { new(0xA3613F4D, 0x81EB0E4F), new(null, typeof(float)) },
            { new(0xA3613F4D, 0xA873293A), new(null, typeof(int)) },

            // LightHaloAttachment
            { new(0x0B213D8E, 0x54F0B858), new(null, typeof(float)) },
            { new(0x0B213D8E, 0xB4A80855), new(null, typeof(int)) },
            { new(0x0B213D8E, 0xF80ABFB7), new(null, typeof(int)) },
            { new(0x0B213D8E, 0x438C44F5), new(null, typeof(int)) },
            { new(0x0B213D8E, 0x0B213D8E), new(null, typeof(int)) },
            { new(0x0B213D8E, 0x1C128785), new(null, typeof(float)) },
            { new(0x0B213D8E, 0x2A8F3B51), new(null, typeof(int)) },

            // PhysicsParticle
            { new(0xFC8B1DA2, 0x9C323CFD), new(null, typeof(float)) },
            { new(0xFC8B1DA2, 0x5B724250), new(null, typeof(Crc)) },
            { new(0xFC8B1DA2, 0x7BE5AA43), new(null, typeof(float)) },
            { new(0xFC8B1DA2, 0xADAE06C3), new(null, typeof(byte)) },
            { new(0xFC8B1DA2, 0x5608BD5A), new(null, typeof(Crc)) },
            { new(0xFC8B1DA2, 0x50F8BFFA), new(null, typeof(byte)) },
            { new(0xFC8B1DA2, 0x46EB60BF), new(null, typeof(float)) },
            { new(0xFC8B1DA2, 0x2E978372), new(null, typeof(Crc)) },
            { new(0xFC8B1DA2, 0x3904F4E4), new(null, typeof(int)) },

            // Weapon
            { new(0x787C0871, 0x7C0B86B9), new(null, typeof(bool)) },
            { new(0x787C0871, 0x334A7577), new(null, typeof(int)) },
            { new(0x787C0871, 0x1667D2C6), new(null, typeof(float)) },
            { new(0x787C0871, 0x0F3D263B), new(null, typeof(float)) },
            { new(0x787C0871, 0x142C5C7A), new(null, typeof(float)) },
            { new(0x787C0871, 0x1508D7DD), new(null, typeof(FloatVector4)) },
            { new(0x787C0871, 0x0B3830D6), new(null, typeof(float)) },
            { new(0x787C0871, 0x09C7D282), new(null, typeof(int)) },
            { new(0x787C0871, 0x079C2D5B), new(null, typeof(float)) },
            { new(0x787C0871, 0x06A053C3), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x074D0D0F), new(null, typeof(float)) },
            { new(0x787C0871, 0x05B6BA7C), new(null, typeof(float)) },
            { new(0x787C0871, 0x02057C8B), new(null, typeof(float)) },
            { new(0x787C0871, 0x0140B5C6), new(null, typeof(float)) },
            { new(0x787C0871, 0x00F23035), new(null, typeof(float)) },
            { new(0x787C0871, 0x0008DD0E), new(null, typeof(float)) },
            { new(0x787C0871, 0x001223FE), new(null, typeof(float)) },
            { new(0x787C0871, 0x04AD1998), new(null, typeof(bool)) },
            { new(0x787C0871, 0x0266D69F), new(null, typeof(float)) },
            { new(0x787C0871, 0x039589BC), new(null, typeof(bool)) },
            { new(0x787C0871, 0x04E3420D), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x2FD5EE93), new(null, typeof(float)) },
            { new(0x787C0871, 0x31E23AD4), new(null, typeof(int)) },
            { new(0x787C0871, 0x33202768), new(null, typeof(bool)) },
            { new(0x787C0871, 0x2FB64EFC), new(null, typeof(float)) },
            { new(0x787C0871, 0x2F5D8019), new(null, typeof(FloatVector2)) },
            { new(0x787C0871, 0x2E022594), new(null, typeof(float)) },
            { new(0x787C0871, 0x2CAA4ACE), new(null, typeof(float)) },
            { new(0x787C0871, 0x2DD68A03), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x2A22FDC1), new(null, typeof(float)) },
            { new(0x787C0871, 0x26AE7598), new(null, typeof(float)) },
            { new(0x787C0871, 0x2652FAE8), new(null, typeof(float)) },
            { new(0x787C0871, 0x20C14C6D), new(null, typeof(float)) },
            { new(0x787C0871, 0x1724437A), new(null, typeof(float)) },
            { new(0x787C0871, 0x1B4B7AF6), new(null, typeof(float)) },
            { new(0x787C0871, 0x277B8B55), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x279A36E1), new(null, typeof(bool)) },
            { new(0x787C0871, 0x29C2A39C), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x64F0F584), new(null, typeof(int)) },
            { new(0x787C0871, 0x5CD15632), new(null, typeof(float)) },
            { new(0x787C0871, 0x63468C4B), new(null, typeof(float)) },
            { new(0x787C0871, 0x63FF9380), new(null, typeof(bool)) },
            { new(0x787C0871, 0x56D6092E), new(null, typeof(float)) },
            { new(0x787C0871, 0x560A68C4), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x544D5A73), new(null, typeof(float)) },
            { new(0x787C0871, 0x4E13EA36), new(null, typeof(float)) },
            { new(0x787C0871, 0x52619D52), new(null, typeof(float)) },
            { new(0x787C0871, 0x498C371F), new(null, typeof(FloatVector2)) },
            { new(0x787C0871, 0x3BF1CDB6), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x379CD846), new(null, typeof(float)) },
            { new(0x787C0871, 0x375004CF), new(null, typeof(float)) },
            { new(0x787C0871, 0x36A350AD), new(null, typeof(float)) },
            { new(0x787C0871, 0x36F01F83), new(null, typeof(float)) },
            { new(0x787C0871, 0x491A1285), new(null, typeof(float)) },
            { new(0x787C0871, 0x44297E86), new(null, typeof(byte)) },
            { new(0x787C0871, 0x3C967A20), new(null, typeof(int)) },
            { new(0x787C0871, 0x3DE8A21D), new(null, typeof(string)) },
            { new(0x787C0871, 0x797A84EF), new(null, typeof(float)) },
            { new(0x787C0871, 0x7A909FD2), new(null, typeof(float)) },
            { new(0x787C0871, 0x7B7A641C), new(null, typeof(float)) },
            { new(0x787C0871, 0x78FA7E87), new(null, typeof(float)) },
            { new(0x787C0871, 0x76ECC8CD), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x74BEF49A), new(null, typeof(int)) },
            { new(0x787C0871, 0x761C3A09), new(null, typeof(float)) },
            { new(0x787C0871, 0x7777D77E), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x73A9FAE3), new(null, typeof(float)) },
            { new(0x787C0871, 0x6DB7B63D), new(null, typeof(int)) },
            { new(0x787C0871, 0x71DFBAEE), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x71E30E69), new(null, typeof(float)) },
            { new(0x787C0871, 0x6CA0E2FE), new(null, typeof(FloatVector4)) },
            { new(0x787C0871, 0x6C93DEA3), new(null, typeof(FloatVector2)) },
            { new(0x787C0871, 0x69059D08), new(null, typeof(bool)) },
            { new(0x787C0871, 0x69DCA2B6), new(null, typeof(int)) },
            { new(0x787C0871, 0x6B484076), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xBC58AA53), new(null, typeof(string)) },
            { new(0x787C0871, 0xA56EE87E), new(null, typeof(bool)) },
            { new(0x787C0871, 0xA3868095), new(null, typeof(float)) },
            { new(0x787C0871, 0xA3C83F69), new(null, typeof(bool)) },
            { new(0x787C0871, 0xA4CA8665), new(null, typeof(float)) },
            { new(0x787C0871, 0xA3416E0A), new(null, typeof(bool)) },
            { new(0x787C0871, 0xA1953CD0), new(null, typeof(float)) },
            { new(0x787C0871, 0xA0C571DD), new(null, typeof(float)) },
            { new(0x787C0871, 0x9737DC5B), new(null, typeof(FloatVector2)) },
            { new(0x787C0871, 0x9B889E4A), new(null, typeof(bool)) },
            { new(0x787C0871, 0x96DC09BD), new(null, typeof(float)) },
            { new(0x787C0871, 0x95F2D6BB), new(null, typeof(bool)) },
            { new(0x787C0871, 0x94FCF44A), new(null, typeof(Crc)) },
            { new(0x787C0871, 0x8FABBD6C), new(null, typeof(float)) },
            { new(0x787C0871, 0x94B5AED3), new(null, typeof(float)) },
            { new(0x787C0871, 0x8EB3B1A4), new(null, typeof(float)) },
            { new(0x787C0871, 0x89341658), new(null, typeof(float)) },
            { new(0x787C0871, 0x865EADCB), new(null, typeof(float)) },
            { new(0x787C0871, 0x7DB30B59), new(null, typeof(int)) },
            { new(0x787C0871, 0x85924A26), new(null, typeof(float)) },
            { new(0x787C0871, 0xB344CC74), new(null, typeof(float)) },
            { new(0x787C0871, 0xB182906E), new(null, typeof(float)) },
            { new(0x787C0871, 0xAE28D6E9), new(null, typeof(float)) },
            { new(0x787C0871, 0xAD451485), new(null, typeof(float)) },
            { new(0x787C0871, 0xADDEBDCA), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xAB3BE1A0), new(null, typeof(int)) },
            { new(0x787C0871, 0xA8AB10E2), new(null, typeof(float)) },
            { new(0x787C0871, 0xA75D5327), new(null, typeof(int)) },
            { new(0x787C0871, 0xA5BC45E2), new(null, typeof(float)) },
            { new(0x787C0871, 0xA6107C89), new(null, typeof(FloatVector2)) },
            { new(0x787C0871, 0xB838A7E9), new(null, typeof(FloatVector2)) },
            { new(0x787C0871, 0xB4D334D8), new(null, typeof(bool)) },
            { new(0x787C0871, 0xB4685717), new(null, typeof(int)) },
            { new(0x787C0871, 0xB4C6842E), new(null, typeof(float)) },
            { new(0x787C0871, 0xB5C71325), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xB8627450), new(null, typeof(float)) },
            { new(0x787C0871, 0xB91AF09E), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xBA157547), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xEC438E17), new(null, typeof(float)) },
            { new(0x787C0871, 0xEBB3F257), new(null, typeof(float)) },
            { new(0x787C0871, 0xE8E01A04), new(null, typeof(int)) },
            { new(0x787C0871, 0xE545FBF4), new(null, typeof(bool)) },
            { new(0x787C0871, 0xE79E08FE), new(null, typeof(float)) },
            { new(0x787C0871, 0xE3945C1D), new(null, typeof(FloatVector2)) },
            { new(0x787C0871, 0xE1F8DAFC), new(null, typeof(bool)) },
            { new(0x787C0871, 0xE0AC4345), new(null, typeof(float)) },
            { new(0x787C0871, 0xDDF78D47), new(null, typeof(float)) },
            { new(0x787C0871, 0xE0428E0D), new(null, typeof(float)) },
            { new(0x787C0871, 0xFA9AD969), new(null, typeof(float)) },
            { new(0x787C0871, 0xFE9C1EEB), new(null, typeof(float)) },
            { new(0x787C0871, 0xF8F8275D), new(null, typeof(float)) },
            { new(0x787C0871, 0xF766AA6F), new(null, typeof(float)) },
            { new(0x787C0871, 0xF535722A), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xEFED376D), new(null, typeof(float)) },
            { new(0x787C0871, 0xF4162380), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xF20499A6), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xDC9E428F), new(null, typeof(float)) },
            { new(0x787C0871, 0xD43A1FE1), new(null, typeof(float)) },
            { new(0x787C0871, 0xD850F531), new(null, typeof(int)) },
            { new(0x787C0871, 0xDB4B8095), new(null, typeof(float)) },
            { new(0x787C0871, 0xD3F2828B), new(null, typeof(bool)) },
            { new(0x787C0871, 0xCE9447BC), new(null, typeof(float)) },
            { new(0x787C0871, 0xD01F6168), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xD1B71AE8), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xD1DD6599), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xCE4384BF), new(null, typeof(FloatVector2)) },
            { new(0x787C0871, 0xCA55EEC9), new(null, typeof(byte)) },
            { new(0x787C0871, 0xC882AF1A), new(null, typeof(float)) },
            { new(0x787C0871, 0xC8E09549), new(null, typeof(int)) },
            { new(0x787C0871, 0xCAF505EA), new(null, typeof(Crc)) },
            { new(0x787C0871, 0xC6300246), new(null, typeof(float)) },
            { new(0x787C0871, 0xC451AB8D), new(null, typeof(FloatVector2)) },
            { new(0x787C0871, 0xBD8F9C00), new(null, typeof(float)) },
            { new(0x787C0871, 0xBFA35CB2), new(null, typeof(FloatVector2)) },
            { new(0x787C0871, 0xC40ED65B), new(null, typeof(int)) },

            // sub_7FB4D0
            { new(0xD0C6D015, 0xE813FE45), new(null, typeof(int)) },
            { new(0xD0C6D015, 0x772B0B2F), new(null, typeof(FloatVector3)) },
            { new(0xD0C6D015, 0x982589A0), new(null, typeof(FloatVector3)) },
            { new(0xD0C6D015, 0xD3DFD699), new(null, typeof(int)) },
            { new(0xD0C6D015, 0xF2CCAF79), new(null, typeof(int)) },
            { new(0xD0C6D015, 0xFD4C5C69), new(null, typeof(Crc)) },
        };
    }
}