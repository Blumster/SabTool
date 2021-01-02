using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabTool.Containers.GameTemplates
{
    using Client;
    using Client.Blueprint;
    using SabTool.Client.Streaming;
    using Utils;
    using Utils.Extensions;

    public class GameTemplates
    {
        public static string UnkStrGlobalCont;
        public int Unknown { get; set; }

        public void Read(Stream stream)
        {
            var blua = new BluaStruct();
            var innerBlua = new BluaStruct();

            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (!br.CheckHeaderString("BLUA", reversed: true))
                    throw new Exception("Invalid header found!");

                blua.Size = (int)br.BaseStream.Length;
                blua.Data = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
            }

            blua.Count = blua.ReadInt();

            if (blua.Count > 0)
            {
                var off = blua.Offset;

                do
                {
                    var offset = blua.ReadInt(off);
                    var v7 = off + 4;
                    var v8 = off + 8;
                    var innerCount = blua.ReadInt(v8);

                    for (off = v8 + 4; innerCount != 0; off = offset + v7)
                    {
                        var strLen1 = blua.ReadInt(off);

                        blua.Offset = off + 4;

                        var str1 = blua.ReadString(strLen1);

                        var strLen2 = blua.ReadInt(blua.Offset);

                        blua.Offset += 4;

                        var str2 = blua.ReadString(strLen2);

                        innerBlua.Size = offset + v7 - (blua.Offset + 4);
                        innerBlua.Data = blua.Data;
                        innerBlua.BaseOff = blua.Offset + 4;
                        innerBlua.Offset = 0;
                        innerBlua.Count = 0;

                        Sub461590(str2, str1, innerBlua);

                        --innerCount;
                    }

                    --blua.Count;
                }
                while (blua.Count != 0);
            }

            /*using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (!br.CheckHeaderString("BLUA", reversed: true))
                    throw new Exception("Invalid header found!");

                var count = br.ReadInt32();
                
                for (var i = 0; i < count; ++i)
                {
                    var off = br.ReadUInt32();
                    var off2 = br.ReadUInt32();
                    var innerCount = br.ReadInt32();

                    for (var j = 0; j < innerCount; ++j) // TODO: 
                    {
                        var len1 = br.ReadInt32();
                        var str1 = br.ReadStringFromCharArray(len1);
                        var len2 = br.ReadInt32();
                        var str2 = br.ReadStringFromCharArray(len2);

                        Console.WriteLine($"Str1: {str1} | Str2: {str2}");

                        SubRead(str2, str1, br);
                    }
                }
            }*/
        }

        public void Write(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                
            }
        }

        private void Sub461590(string categoryStr, string unkStr, BluaStruct blua)
        {
            var categoryHash = Hash.StringToHash(categoryStr);
            var unkStrHash = Hash.StringToHash(unkStr);

            
            WSBlueprint blueprint = null;
            // TODO: get blueprint from cache

            var found = blueprint != null;
            if (found)
            {
                blueprint.Sub452CC0(blua, categoryStr, unkStr);
            }

            UnkStrGlobalCont = unkStr;

            if (!found)
            {
                switch (categoryStr)
                {
                    case "Weapon":
                        break;

                    case "Searcher":
                        break;

                    case "MeleeWeapon":
                        break;

                    case "Ammo":
                        break;

                    case "bullet":
                        break;

                    case "FlameOrdnance":
                        break;

                    case "PhysicalOrdnance":
                        break;

                    case "Rocket":
                        break;

                    case "HumanPhysics":
                        break;

                    case "Player":
                        break;

                    case "Car":
                        break;

                    case "Truck":
                        break;

                    case "APC":
                        break;

                    case "Tank":
                        break;

                    case "Human":
                        break;

                    case "Spore":
                        break;

                    case "VehicleCollision":
                        break;

                    case "PlayerCollision":
                        break;

                    case "Targetable":
                        break;

                    case "Prop":
                        break;

                    case "TrainList":
                        break;

                    case "Train":
                        break;

                    case "Foliage":
                        break;

                    case "AIAttractionPt":
                        break;

                    case "DamageSphere":
                        break;

                    case "Explosion":
                        break;

                    case "AIController":
                        break;

                    case "ScriptController":
                        blueprint = new WSAIScriptControllerBlueprint();
                        break;

                    case "AISpawner":
                        break;

                    case "AICombatParams":
                        break;

                    case "AICrowdBlocker":
                        break;

                    case "GlobalHumanParams":
                        break;

                    case "Melee":
                        break;

                    case "ParticleEffect":
                        break;

                    case "RadialBlur":
                        break;

                    case "Sound":
                        break;

                    case "Turret":
                        break;

                    case "SearchTurret":
                        break;

                    case "TrainCarriage":
                        break;

                    case "TrainEngine":
                        break;

                    case "TrainItem":
                        break;

                    case "SlowMotionCamera":
                        break;

                    case "ElasticTransition":
                        break;

                    case "AnimatedTransition":
                        break;

                    case "GroupTransition":
                        break;

                    case "ScopeTransition":
                        break;

                    case "CameraSettings":
                        break;

                    case "GroupCameraSettings":
                        break;

                    case "CameraSettingsMisc":
                        break;

                    case "ToneMapping":
                        break;

                    case "LightSettings":
                        break;

                    case "LightHalo":
                        break;

                    case "LightVolume":
                        break;

                    case "ClothObject":
                        break;

                    case "ClothForce":
                        break;

                    case "WillToFight":
                        break;

                    case "WillToFightNode":
                        break;

                    case "HealthEffectFilter":
                        break;

                    case "ParticleEffectSpawner":
                        break;

                    case "ButtonPrompt":
                        break;

                    case "Item":
                        break;

                    case "FxBoneStateList":
                        break;

                    case "FxHumanHead":
                        break;

                    case "FxHumanBodyPart":
                        break;

                    case "FxHumanBodySetup":
                        break;

                    case "FaceExpression":
                        break;

                    case "HumanBodyPart":
                        break;

                    case "HumanBodySetup":
                        break;

                    case "HumanSkeletonScale":
                        break;

                    case "AnimatedObject":
                        break;

                    case "RandomObj":
                        break;

                    case "BridgeController":
                        break;

                    case "Ricochet":
                        break;

                    case "AIRoad":
                        break;

                    case "Highlight":
                        break;

                    case "MiniGame":
                        break;

                    case "SabotageTarget":
                        break;

                    case "BigMap":
                        break;

                    case "FlashMovie":
                        break;

                    case "ItemCache":
                        break;

                    case "VirVehicleWheel":
                        break;

                    case "VirVehicleTransmission":
                        break;

                    case "VirVehicleEngine":
                        break;

                    case "VirVehicleChassis":
                        break;

                    case "VirVehicleSetup":
                        break;

                    case "VehicleWheelFX":
                        break;

                    case "Difficulty":
                        break;

                    case "Shop":
                        break;

                    case "Perks":
                        break;

                    case "PerkFactors":
                        break;

                    case "PhysicsParticleSet":
                        break;

                    case "PhysicsParticle":
                        break;

                    case "Decal":
                        break;

                    case "DetailObject":
                        break;

                    case "CivilianProp":
                        break;

                    case "Bird":
                        break;

                    case "Escalation":
                        break;

                    case "EscHWTF":
                        break;

                    case "AIChatterSet":
                        break;

                    case "AIChatter":
                        break;

                    case "SeatAnimations":
                        break;

                    case "SeatAnimationsPassenger":
                        break;

                    case "SeatAnimationsGunner":
                        break;

                    case "SeatAnimationsSearcher":
                        break;

                    case "SeatAnimationsDriver":
                        break;

                    case "FoliageFx":
                        break;

                    case "WaterController":
                        break;

                    case "WaterTexture":
                        break;

                    case "LeafSpawner":
                        break;

                    case "VerletBoneObject":
                        break;

                    case "WaterParticleFx":
                        break;

                    case "Cinematics":
                        break;

                    case "Rumble":
                        break;

                    case "ImageFolder":
                        break;

                    case "CreditName":
                        break;
                }
                
                if (categoryStr == "CommonUI_Persistent" || categoryStr == "Common" || categoryStr == "SingleImage" || categoryStr == "InteriorImages" || categoryStr == "AIPathPt" || blueprint == null)
                    return;
            }

            blueprint.Priority = 0;

            if (!found)
                blueprint.Sub452C90(unkStr);

            blueprint.Sub4614A0(blua, categoryStr, unkStr);

            if (found)
                blueprint.Sub452CD0();

            if (blueprint.Sub452D10())
            {
                blueprint.Sub460920(WSStreamingManager.Instance.Sub9F1860(unkStrHash));
            }
        }
    }
}
