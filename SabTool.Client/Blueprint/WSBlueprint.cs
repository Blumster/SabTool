using System.Threading;

namespace SabTool.Client.Blueprint
{
    using Pebble;
    using SabTool.Utils;
    using Streaming;

    public class WSBlueprint : PblCounted<WSBlueprint>
    {
        private int _useCount;

        public WSBlueprint Backup { get; set; }
        public WSStreamBlockNode StreamBlockNode { get; set; }
        public int DwordC
        {
            get
            {
                return _useCount;
            }
            set
            {
                _useCount = value;
            }
        }
        public byte Priority { get; set; }
        public byte Flags { get; set; }

        public WSBlueprint()
        {
            Backup = null;
            StreamBlockNode = null;
            DwordC = 0;
            Priority = 0;
            Flags = 0;
        }

        public bool Sub443880()
        {
            if (StreamBlockNode != null && ((StreamBlockNode.StreamBlock.Flags & 0xC0) == 0 || (StreamBlockNode.StreamBlock.Flags & 0x2000) == 0x2000))
                return false;

            return true;
        }

        public virtual object GetDamagableBlueprint()
        {
            // TODO: proper object
            return null;
        }

        public virtual void Sub452670() { }
        public virtual void Sub452680() { }

        public virtual object GetModelRenderableBlueprint()
        {
            // TODO: proper object
            return null;
        }

        public virtual void Sub4526A0() { }
        public virtual void Sub4526B0() { }
        public virtual void Sub4526C0() { }

        public virtual object GetTargetableBlueprint()
        {
            // TODO: proper object
            return null;
        }

        public virtual void Sub4526E0() { }
        public virtual void Sub4526F0() { }
        public virtual void Sub452700() { }
        public virtual void Sub452710() { }
        public virtual void Sub452720() { }
        public virtual void Sub452730() { }
        public virtual void Sub452740() { }
        public virtual void Sub452750() { }
        public virtual void Sub452760() { }
        public virtual void Sub452770() { }
        public virtual void Sub452780() { }
        public virtual void Sub452790() { }
        public virtual void Sub4527A0() { }
        public virtual void Sub4527B0() { }
        public virtual void Sub4527C0() { }
        public virtual void Sub4527D0() { }
        public virtual void Sub4527E0() { }

        public virtual object Sub4527F0()
        {
            // TODO: proper object
            return null;
        }

        public virtual object GetAIAttractionPtBlueprint()
        {
            // TODO: proper object
            return null;
        }

        public virtual void Sub452810() { }
        public virtual void Sub452820() { }
        public virtual void Sub452830() { }

        public virtual IWSControllableBlueprint GetControllableBlueprint()
        {
            return null;
        }

        public virtual void Sub452850() { }
        public virtual void Sub452860() { }
        public virtual void Sub452870() { }
        public virtual void Sub452880() { }
        public virtual void Sub452890() { }
        public virtual void Sub4528A0() { }
        public virtual void Sub4528B0() { }
        public virtual void Sub4528C0() { }
        public virtual void Sub4528D0() { }
        public virtual void Sub4528E0() { }
        public virtual void Sub4528F0() { }
        public virtual void Sub452900() { }
        public virtual void Sub452910() { }
        public virtual void Sub452920() { }
        public virtual void Sub452930() { }
        public virtual void Sub452940() { }
        public virtual void Sub452950() { }
        public virtual void Sub452960() { }
        public virtual void Sub452970() { }
        public virtual void Sub452980() { }
        public virtual void Sub452990() { }
        public virtual void Sub4529A0() { }
        public virtual void Sub4529B0() { }
        public virtual void Sub4529C0() { }
        public virtual void Sub4529D0() { }
        public virtual void Sub4529E0() { }
        public virtual void Sub4529F0() { }
        public virtual void Sub452A00() { }
        public virtual void Sub452A10() { }

        public virtual object GetAudibleBlueprint()
        {
            // TODO: proper object
            return null;
        }

        public virtual void Sub452A30() { }
        public virtual void Sub452A40() { }
        public virtual void Sub452A50() { }
        public virtual void Sub452A60() { }
        public virtual void Sub452A70() { }
        public virtual void Sub452A80() { }
        public virtual void Sub452A90() { }
        public virtual void Sub452AA0() { }
        public virtual void Sub452AB0() { }
        public virtual void Sub452AC0() { }
        public virtual void Sub452AD0() { }
        public virtual void Sub452AE0() { }
        public virtual void Sub452AF0() { }
        public virtual void Sub452B00() { }
        public virtual void Sub452B10() { }
        public virtual void Sub452B20() { }
        public virtual void Sub452B30() { }
        public virtual void Sub452B40() { }
        public virtual void Sub452B50() { }
        public virtual void Sub452B60() { }
        public virtual void Sub452B70() { }
        public virtual void Sub452B80() { }
        public virtual void Sub452B90() { }
        public virtual void Sub452BA0() { }
        public virtual void Sub452BB0() { }
        public virtual void Sub452BC0() { }
        public virtual void Sub452BD0() { }
        public virtual void Sub452BE0() { }
        public virtual void Sub452BF0() { }
        public virtual void Sub452C00() { }
        public virtual void Sub452C10() { }
        public virtual void Sub452C20() { }
        public virtual void Sub452C30() { }
        public virtual void Sub452C40() { }
        public virtual void Sub452C50() { }
        public virtual void Sub452C60() { }
        public virtual void Sub452C70() { }
        public virtual void Sub452C80() { }

        public virtual void Init(string blueprintName)
        {
        }

        public virtual void Preload()
        {
        }

        public virtual bool SetProperty(uint crc, BluaReader blua)
        {
            return false;
        }

        public virtual void PostLoad()
        {
        }

        public virtual void Sub452CC0(BluaReader a, string category, string unk)
        {
        }

        public virtual void EndLiveUpdate()
        {
        }

        public virtual int Sub460830()
        {
            if (StreamBlockNode != null)
                return StreamBlockNode.StreamBlock.Sub65A5E0();

            return 0;
        }

        public virtual int Sub460850()
        {
            if (StreamBlockNode != null)
                return (int)(StreamBlockNode.StreamBlock.Sub65C0A0() * 1.0f);

            return 0;
        }

        public virtual int Sub4608C0(int a2, int a3)
        {
            if (StreamBlockNode != null)
                return StreamBlockNode.StreamBlock.Sub65BDF0(a2, a3);

            return 0;
        }

        public virtual void Sub452CE0() { }
        public virtual void Sub452CF0() { }
        public virtual void Sub452D00() { }

        public virtual bool Sub460960()
        {
            return Sub443880();
        }

        public virtual bool IsDynamic()
        {
            return (Flags & 1) != 0;
        }

        public virtual void Sub4608E0()
        {
            Interlocked.Increment(ref _useCount);
        }

        public virtual void Sub4608F0()
        {
            if (Interlocked.Decrement(ref _useCount) == 0 && (Flags & 1) == 1)
            {
                Flags |= 8;
                // TODO: Sub988780(dword_1494190, this);
            }
        }

        public void Sub41DF90(WSStreamBlockNode node)
        {
            StreamBlockNode = node;

            if (StreamBlockNode != null)
            {
                StreamBlockNode.StreamBlock.Blueprint = this;
                StreamBlockNode.StreamBlock.Flags ^= (uint)((StreamBlockNode.StreamBlock.Flags ^ (Flags << 17)) & 0x40000);
            }
        }

        public void SetStreamBlock(WSStreamBlockNode node)
        {
            Sub41DF90(node);
        }

        public bool SetCommonProperty(uint crc, BluaReader blua)
        {
            bool v4 = false;

            var v5 = GetDamagableBlueprint();
            if (v5 != null && false) // TODO: struct is returned in the above call
                v4 = true;

            var v7 = GetAudibleBlueprint();
            if (v7 != null && false) // TODO: struct is returned in the above call
                v4 = true;

            var v8 = GetModelRenderableBlueprint();
            if (v8 != null && false) // TODO: struct is returned in the above call
                v4 = true;

            var v9 = GetTargetableBlueprint();
            if (v9 != null && false) // TODO: struct is returned in the above call
                v4 = true;

            var v10 = GetAIAttractionPtBlueprint();
            if (v10 != null && false) // TODO: struct is returned in the above call
                v4 = true;

            var v11 = GetControllableBlueprint();
            if (v11 != null && v11.SetControllableProperty(crc, blua))
                v4 = true;

            if (Hash.StringToHash("dynamic") == crc)
            {
                Flags ^= (byte)((blua.ReadByte() ^ Flags) & 1);

                return true;
            }

            if (Hash.StringToHash("UseDynamicPool") == crc)
            {
                Flags ^= (byte)(((2 * blua.ReadByte()) ^ Flags) & 2);

                return true;
            }

            if (Hash.StringToHash("priority") == crc)
            {
                var prio = blua.ReadInt();
                if (prio >= 0)
                {
                    if (prio > 255)
                        prio = 255;

                    Priority = (byte)prio;
                }
                else
                {
                    Priority = 0;
                }

                return true;
            }

            if (Hash.StringToHash("backup") == crc)
            {
                var crc2 = blua.ReadInt();
                if (crc2 != Hash.StringToHash("NONE") && crc2 != 0)
                {
                    Backup = null; // TODO
                }
                else
                {
                    Backup = null;
                }

                return true;
            }

            if (Hash.StringToHash("managed") == crc)
            {
                Flags ^= (byte)(((4 * blua.ReadByte()) ^ Flags) & 4);

                return true;
            }

            return v4;
        }

        public void SetProperties(BluaReader blua, string bluprintType, string blueprintName)
        {
            Preload();

            while (blua.Offset < blua.Size)
            {
                var crc = blua.ReadUInt();
                var propertyValueLength = blua.ReadInt();

                if (!SetProperty(crc, blua) && !SetCommonProperty(crc, blua))
                {
                    // Skip over this property, if it wasn't read properly
                    blua.Offset += propertyValueLength;
                }
            }

            PostLoad();
        }


        public static string UnkStrGlobalCont;
        public static void Create(string blueprintType, string blueprintName, BluaReader blua)
        {
            var typeCrc = Hash.StringToHash(blueprintType);
            var nameCrc = Hash.StringToHash(blueprintName);


            WSBlueprint blueprint = null;
            // TODO: get blueprint from cache

            var found = blueprint != null;
            if (found)
            {
                blueprint.Sub452CC0(blua, blueprintType, blueprintName);
            }

            UnkStrGlobalCont = blueprintName;

            if (!found)
            {
                switch (blueprintType)
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

                if (blueprintType == "CommonUI_Persistent" || blueprintType == "Common" || blueprintType == "SingleImage" || blueprintType == "InteriorImages" || blueprintType == "AIPathPt" || blueprint == null)
                    return;
            }

            blueprint.Priority = 0;

            if (!found)
                blueprint.Init(blueprintName);

            blueprint.SetProperties(blua, blueprintType, blueprintName);

            if (found)
                blueprint.EndLiveUpdate();

            if (blueprint.IsDynamic())
            {
                blueprint.SetStreamBlock(WSStreamingManager.Instance.FindInDynamicBlocks(nameCrc));
            }
        }


        public virtual int Sub452D20()
        {
            return DwordC;
        }

        public virtual void Sub452D30() { }
        public virtual void Sub452D40() { }

        public virtual byte Sub452D50()
        {
            return Priority;
        }
    }
}
