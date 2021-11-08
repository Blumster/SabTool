using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Data
{
    using Lua;
    using Utils;
    using Utils.Extensions;

    public partial class Blueprint
    {
        private static Dictionary<(string Group, string Name), Type> PropertyEntries { get; } = new();
        private static Dictionary<string, HashSet<string>> HierarchyEntries { get; } = new()
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
            { "Player", new() { "Human", "CameraSet" } },
            { "Car", new() { "Vehicle" } },
            { "Truck", new() { "Car" } },
            { "APC", new() { "Car" } },
            { "Tank", new() { "Vehicle" } },
            { "Human", new() { "Aimer", "Damageable", "HumanPhysics", "Targetable", "Controllable", "Audible" } },
            { "Spore", new() { "Controllable" } },
            { "VehicleCollision", new() { } },
            { "PlayerCollision", new() { } },
            { "Targetable", new() { } },
            { "Prop", new() { "ModelRenderable", "Damageable", "LightAttachement", "LightHaloAttachment", "AttrPtAttachable", "Audible" } },
            { "TrainList", new() { } },
            { "Train", new() { } },
            { "Foliage", new() { } },
            { "AIAttractionPt", new() { "Controllable", "Trappable" } },
            { "DamageSphere", new() { "DamageRegion" } },
            { "Explosion", new() { "Audible" } },
            { "AIController", new() { } },
            { "ScriptController", new() { "Controllable" } },
            { "AISpawner", new() { "Controllable" } },
            { "AICombatParams", new() { } },
            { "AICrowdBlocker", new() { } },
            { "GlobalHumanParams", new() { } },
            { "Melee", new() { } },
            { "ParticleEffect", new() { } },
            { "RadialBlur", new() { } },
            { "Sound", new() { "Audible" } },
            { "Turret", new() { "Prop", "GunnerSeat" } },
            { "SearchTurret", new() { "Prop", "SearcherSeat" } },
            { "TrainCarriage", new() { "Vehicle" } },
            { "TrainEngine", new() { "TrainCarriage" } },
            { "TrainItem", new() { "Prop" } },
            { "SlowMotionCamera", new() { } },
            { "ElasticTransition", new() { } },
            { "AnimatedTransition", new() { "ElasticTransition" } },
            { "GroupTransition", new() { "ElasticTransition" } },
            { "ScopeTransition", new() { "ElasticTransition" } },
            { "CameraSettings", new() { } },
            { "GroupCameraSettings", new() { "CameraSettings" } },
            { "CameraSettingsMisc", new() { } },
            { "ToneMapping", new() { } },
            { "LightSettings", new() { "LightAttachement", "LightHaloAttachment", "sub_7FB4D0" } },
            { "LightHalo", new() { } },
            { "LightVolume", new() { } },
            { "ClothObject", new() { "ModelRenderable" } },
            { "ClothForce", new() { } },
            { "WillToFight", new() { } },
            { "WillToFightNode", new() { } },
            { "HealthEffectFilter", new() { } },
            { "ParticleEffectSpawner", new() { "Audible" } },
            { "ButtonPrompt", new() { } },
            { "Item", new() { "LightAttachement", "LightHaloAttachment" } },
            { "FxBoneStateList", new() { } },
            { "FxHumanHead", new() { } },
            { "FxHumanBodyPart", new() { } },
            { "FxHumanBodySetup", new() { } },
            { "FaceExpression", new() { } },
            { "HumanBodyPart", new() { } },
            { "HumanBodySetup", new() { } },
            { "HumanSkeletonScale", new() { } },
            { "AnimatedObject", new() { "BonePair", "AttrPtAttachable", "Damageable", "Audible", "Trappable" } },
            { "RandomObj", new() { } },
            { "BridgeController", new() { } },
            { "Ricochet", new() { } },
            { "AIRoad", new() { } },
            { "Highlight", new() { } },
            { "MiniGame", new() { } },
            { "SabotageTarget", new() { } },
            { "BigMap", new() { } },
            { "FlashMovie", new() { } },
            { "ItemCache", new() { "ModelRenderable" } },
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
            { "PhysicsParticleSet", new() { "ModelRenderable" } },
            { "PhysicsParticle", new() { "ModelRenderable" } },
            { "Decal", new() { } },
            { "DetailObject", new() { } },
            { "CivilianProp", new() { } },
            { "Bird", new() { "Audible", "BirdSpawner" } },
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
            { "WaterFlow", new() { } },

            { "Damageable", new() { "DamageableRoot" } },
            { "DamageableRoot", new() { "DamageablePart" } },
            { "Vehicle", new() { "ModelRenderable", "Damageable", "LightAttachement", "LightHaloAttachment", "Audible", "PhysicsVehicle", "Trappable", "GunnerSeat", "AttrPtAttachable", "TweakableColors", "Seat", "SearcherSeat" } },
            { "PhysicsVehicle", new() { "PhysicsVehicleWheel" } },
            { "GunnerSeat", new() { "SeatWithMount" } },
            { "SearcherSeat", new() { "SeatWithMount" } },
            { "SeatWithMount", new() { "Seat", "Aimer" } },
            { "Seat", new() { "Controllable" } },
        };

        public static void Setup()
        {
            PropertyEntries.Add(("AIAttractionPt", "UseInLowWTF"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "Labels"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "UseRadiusMax"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "0xE773FC1F"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0xE29D36B0"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "0xE66CED59"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "SabotageDamage"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "MGDegradeToZero"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0xDE634E2F"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0xDF0B2054"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "MGDegradeSpeed"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "MaxSlots"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "Description"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "0xC6E0F627"), typeof(double));
            PropertyEntries.Add(("AIAttractionPt", "UseAngleMax"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "SocialRate"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "0xD27568AD"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0xF01F08A8"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "0xEA73DB2F"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0xECF2E7AE"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "SpotLight"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "CivilianProp"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0xE94E8608"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "UseInHighWTF"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "XOffset"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "AI"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "VisibleAtEscalation"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "LabelFilter"), typeof(char[]));
            PropertyEntries.Add(("AIAttractionPt", "BlueprintToStartWith"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "FunRate"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "0xC54E8CAE"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x9EDB6BD9"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "StartDisabled"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x9AAC5788"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "Offset"), typeof(byte[]));
            PropertyEntries.Add(("AIAttractionPt", "Explosion Offset"), typeof(byte[]));
            PropertyEntries.Add(("AIAttractionPt", "0x92D0A9BA"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "Display Name"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "IsDoorTrigger"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x8C8D6435"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0x8D89FAD9"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x8DB13A88"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "Prompt"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0x07F79C5D"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "InventoryItem"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0x84D70C5E"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "Usable"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0xA8DB360C"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "0xAEEA42F8"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "IdleDuration"), typeof(double));
            PropertyEntries.Add(("AIAttractionPt", "AttrPtScript"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "0xC1D0622B"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "IdleAltDuration"), typeof(double));
            PropertyEntries.Add(("AIAttractionPt", "0xA5A71B8A"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "0xA7E0D198"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "ShedSuspicion"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "Num Items Needed"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "Time to Explosion"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "YRot"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "EmptyIdle"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x658D48E2"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "0x506DE042"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0x4785F2A3"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "AnimationOutOf"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "0x4D44EFE0"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "ZOffset"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "MiniGameBlueprint"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0x41B4ACE8"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0x460E7F77"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x55D89223"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0x507A477F"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "Prompt2"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "AutoDetonate"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "ParentObject"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "YOffset"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "0x733ED3B2"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "QueueMaxLength"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "IdleDurationMax"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "StartUseDisabled"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "AnimationInto"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "Rooftop"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "PreferredDistance"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "FoodRate"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "MGLockOnComplete"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "DLCMiniGame"), typeof(char[]));
            PropertyEntries.Add(("AIAttractionPt", "0x7A3E99B4"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "0x7794DACB"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "SexRate"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "VisualItem"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0x4070B35E"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "Timer Enabled"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x0F12604C"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "AnimationIdleAlt"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "UseHeightMax"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "AlarmRate"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "0x054A8FB1"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "DoNotRayCast"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "RunHere"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x04FBB1EE"), typeof(char[]));
            PropertyEntries.Add(("AIAttractionPt", "0x1DE59C2F"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "Explosion"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "AnimationIdle"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "AffectedGroup"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "Name"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0x23518B03"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "SabotageType"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "NumUseStates"), typeof(int));
            PropertyEntries.Add(("AIAttractionPt", "AttrPtPlus"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "EnableAlts"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "SafetyRate"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "0x365FB660"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "HuntRate"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "DisableAfterSabotage"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x2ED2EC21"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x3217CDF0"), typeof(float));
            PropertyEntries.Add(("AIAttractionPt", "0x2C652AF7"), typeof(Crc));
            PropertyEntries.Add(("AIAttractionPt", "0x2523B4EE"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "0x29EC9F57"), typeof(bool));
            PropertyEntries.Add(("AIAttractionPt", "SabotagePt"), typeof(bool));

            PropertyEntries.Add(("AIChatter", "DISTANCE"), typeof(float));
            PropertyEntries.Add(("AIChatter", "0xBDB6564E"), typeof(float));
            PropertyEntries.Add(("AIChatter", "0xC187EFD3"), typeof(bool));
            PropertyEntries.Add(("AIChatter", "0xDC83F68F"), typeof(Vector2));
            PropertyEntries.Add(("AIChatter", "priority"), typeof(int));
            PropertyEntries.Add(("AIChatter", "0x70C454D5"), typeof(Vector2));
            PropertyEntries.Add(("AIChatter", "0x7213E378"), typeof(int));
            PropertyEntries.Add(("AIChatter", "Conversation"), typeof(Crc));
            PropertyEntries.Add(("AIChatter", "0x076B2507"), typeof(Vector2));
            PropertyEntries.Add(("AIChatter", "0x22CC5267"), typeof(bool));

            PropertyEntries.Add(("AIChatterSet", "0xDF9C6FEF"), typeof(Crc));
            PropertyEntries.Add(("AIChatterSet", "0x8D9F39B1"), typeof(Crc));
            PropertyEntries.Add(("AIChatterSet", "0xBBA6EBA6"), typeof(Crc));
            PropertyEntries.Add(("AIChatterSet", "0xD5638D90"), typeof(int));

            PropertyEntries.Add(("AICombatParams", "Presence"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "ChargeDist"), typeof(Vector2));
            PropertyEntries.Add(("AICombatParams", "0x4EBED1B7"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "Fear"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "MoveSpeedMultiplier"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "ChargeRate"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "UseGrenades"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "Aggression"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "WeaponPickup"), typeof(bool));
            PropertyEntries.Add(("AICombatParams", "CoverStyle"), typeof(Crc));
            PropertyEntries.Add(("AICombatParams", "EnableCharge"), typeof(byte));
            PropertyEntries.Add(("AICombatParams", "ImmediateHostile"), typeof(bool));
            PropertyEntries.Add(("AICombatParams", "0x8BB6AD21"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "0x91DEEDFA"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "CoverRate"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "ImmediateGunplay"), typeof(bool));
            PropertyEntries.Add(("AICombatParams", "AccuracyModifier"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "Intelligence"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "WeaponBashWhenClose"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "StandTime"), typeof(float));
            PropertyEntries.Add(("AICombatParams", "0xFA03EB28"), typeof(float));

            PropertyEntries.Add(("AIController", "AlarmDecay"), typeof(float));
            PropertyEntries.Add(("AIController", "InitialModule"), typeof(char[]));
            PropertyEntries.Add(("AIController", "RandomInitialNeeds"), typeof(byte));
            PropertyEntries.Add(("AIController", "FunDecay"), typeof(float));
            PropertyEntries.Add(("AIController", "SafetyDecay"), typeof(float));
            PropertyEntries.Add(("AIController", "FoodDecay"), typeof(float));
            PropertyEntries.Add(("AIController", "Labels"), typeof(Crc));
            PropertyEntries.Add(("AIController", "HuntDecay"), typeof(float));
            PropertyEntries.Add(("AIController", "SocialDecay"), typeof(float));
            PropertyEntries.Add(("AIController", "SexDecay"), typeof(float));

            PropertyEntries.Add(("AICrowdBlocker", "0x888F7A10"), typeof(bool));

            PropertyEntries.Add(("Aimer", "0x6DA3DB23"), typeof(float));
            PropertyEntries.Add(("Aimer", "YawSpeed"), typeof(float));
            PropertyEntries.Add(("Aimer", "FireBone"), typeof(Crc));
            PropertyEntries.Add(("Aimer", "0x63EC5E50"), typeof(float));
            PropertyEntries.Add(("Aimer", "PitchSpeed"), typeof(float));
            PropertyEntries.Add(("Aimer", "0x1379EBE2"), typeof(float));
            PropertyEntries.Add(("Aimer", "0x28506E5C"), typeof(float));
            PropertyEntries.Add(("Aimer", "FireOffset"), typeof(Vector3));
            PropertyEntries.Add(("Aimer", "PitchLimits"), typeof(Vector2));
            PropertyEntries.Add(("Aimer", "PitchBone"), typeof(Crc));
            PropertyEntries.Add(("Aimer", "YawBone"), typeof(Crc));
            PropertyEntries.Add(("Aimer", "0xE88FD118"), typeof(float));
            PropertyEntries.Add(("Aimer", "YawLimits"), typeof(Vector2));
            PropertyEntries.Add(("Aimer", "AimerYawOffset"), typeof(float));

            PropertyEntries.Add(("AIRoad", "SpawnSeat"), typeof(Crc));
            PropertyEntries.Add(("AIRoad", "Passengers"), typeof(int));
            PropertyEntries.Add(("AIRoad", "AIRoad_Vehicle"), typeof(Crc));
            PropertyEntries.Add(("AIRoad", "0xA1C6D3A0"), typeof(bool));
            PropertyEntries.Add(("AIRoad", "Train"), typeof(bool));
            PropertyEntries.Add(("AIRoad", "SpawnObject"), typeof(Crc));
            PropertyEntries.Add(("AIRoad", "Frequency"), typeof(float));
            PropertyEntries.Add(("AIRoad", "0x0181869C"), typeof(bool));
            PropertyEntries.Add(("AIRoad", "Activity"), typeof(float));
            PropertyEntries.Add(("AIRoad", "Vehicle"), typeof(Crc));
            PropertyEntries.Add(("AIRoad", "AIRoad_Passenger"), typeof(Crc));
            PropertyEntries.Add(("AIRoad", "Vehicles"), typeof(int));

            PropertyEntries.Add(("AISpawner", "MaxNumAlive"), typeof(int));
            PropertyEntries.Add(("AISpawner", "AutoSpawn"), typeof(bool));
            PropertyEntries.Add(("AISpawner", "ParentObject"), typeof(Crc));
            PropertyEntries.Add(("AISpawner", "CheckVisibility"), typeof(bool));
            PropertyEntries.Add(("AISpawner", "SpawnBlueprint"), typeof(Crc));
            PropertyEntries.Add(("AISpawner", "MinTimeBetweenSpawns"), typeof(float));
            PropertyEntries.Add(("AISpawner", "MaxNumSpawns"), typeof(int));
            PropertyEntries.Add(("AISpawner", "SpawnOneWhenDead"), typeof(bool));

            PropertyEntries.Add(("Ammo", "0x6596A232"), typeof(int));
            PropertyEntries.Add(("Ammo", "Amount"), typeof(int));
            PropertyEntries.Add(("Ammo", "Highlight"), typeof(Crc));
            PropertyEntries.Add(("Ammo", "0x524FE4FC"), typeof(int));
            PropertyEntries.Add(("Ammo", "MaximumAmount"), typeof(int));
            PropertyEntries.Add(("Ammo", "0xC84E7BEF"), typeof(int));
            PropertyEntries.Add(("Ammo", "0xDE11CA90"), typeof(int));
            PropertyEntries.Add(("Ammo", "Item"), typeof(Crc));

            PropertyEntries.Add(("AnimatedObject", "0x6BEC5CB4"), typeof(Crc));
            PropertyEntries.Add(("AnimatedObject", "EaseIn3"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "0x5C6BB173"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "0x5AAB9E8A"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "Model"), typeof(Crc));
            PropertyEntries.Add(("AnimatedObject", "EaseIn4"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "0x504894BF"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "AnimNode4Rot"), typeof(Vector3));
            PropertyEntries.Add(("AnimatedObject", "AnimNode2Rot"), typeof(Vector3));
            PropertyEntries.Add(("AnimatedObject", "0x3E495387"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "AnimTime4"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "AnimTime2"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "CloseDelay"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "EaseOut4"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "0x2CE48D05"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "AnimNode3Pos"), typeof(Vector3));
            PropertyEntries.Add(("AnimatedObject", "EaseIn2"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "LoopingAnim"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "EaseOut2"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "AnimNode4Pos"), typeof(Vector3));
            PropertyEntries.Add(("AnimatedObject", "0x1EE86C0E"), typeof(Crc));
            PropertyEntries.Add(("AnimatedObject", "EaseOut3"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "StartOpened"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "CloseAfterOpen"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "0xB77F637A"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "AnimTime3"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "AnimTime1"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "0xAEFF60B8"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "0xAF87143A"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "AnimateAtStart"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "0x92D3DD4C"), typeof(Crc));
            PropertyEntries.Add(("AnimatedObject", "0x6D36B289"), typeof(Crc));
            PropertyEntries.Add(("AnimatedObject", "EaseOut1"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "ActivationDelay"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "0xA42DAB74"), typeof(Crc));
            PropertyEntries.Add(("AnimatedObject", "AnimNode1Rot"), typeof(Vector3));
            PropertyEntries.Add(("AnimatedObject", "AnimNode2Pos"), typeof(Vector3));
            PropertyEntries.Add(("AnimatedObject", "StopHere4"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "0xDF77EDBB"), typeof(int));
            PropertyEntries.Add(("AnimatedObject", "StopHere2"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "0xF5572421"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "StopHere1"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "AttachedObject"), typeof(Crc));
            PropertyEntries.Add(("AnimatedObject", "AnimNode1Pos"), typeof(Vector3));
            PropertyEntries.Add(("AnimatedObject", "0xD19D14C4"), typeof(Crc));
            PropertyEntries.Add(("AnimatedObject", "AnimNode3Rot"), typeof(Vector3));
            PropertyEntries.Add(("AnimatedObject", "EaseIn1"), typeof(float));
            PropertyEntries.Add(("AnimatedObject", "StopHere3"), typeof(bool));
            PropertyEntries.Add(("AnimatedObject", "0x2D07DA6F"), typeof(Crc));
            PropertyEntries.Add(("AnimatedObject", "0xE6023240"), typeof(Crc));
            PropertyEntries.Add(("AnimatedObject", "0x6ADE730A"), typeof(Crc));

            PropertyEntries.Add(("AnimatedTransition", "0x1EDFEE31"), typeof(Vector3));
            PropertyEntries.Add(("AnimatedTransition", "0x5570EF5A"), typeof(float));
            PropertyEntries.Add(("AnimatedTransition", "0xEB4DFB24"), typeof(Crc));

            PropertyEntries.Add(("AttrPtAttachable", "AIAttractionPt"), typeof(int));
            PropertyEntries.Add(("AttrPtAttachable", "AttrPtRotation"), typeof(float));
            PropertyEntries.Add(("AttrPtAttachable", "AttrPtAttachment"), typeof(Crc));
            PropertyEntries.Add(("AttrPtAttachable", "AttrPtOffset"), typeof(Vector3));
            PropertyEntries.Add(("AttrPtAttachable", "AttrPt"), typeof(Crc));

            PropertyEntries.Add(("Audible", "Audible"), typeof(Crc));
            PropertyEntries.Add(("Audible", "Sound Events"), typeof(int));
            PropertyEntries.Add(("Audible", "Game Event Name"), typeof(Crc));
            PropertyEntries.Add(("Audible", "0x3217CDF0"), typeof(float));
            PropertyEntries.Add(("Audible", "Send Distance"), typeof(bool));
            PropertyEntries.Add(("Audible", "AudibleEvent"), typeof(Crc));
            PropertyEntries.Add(("Audible", "0x04FBB1EE"), typeof(char[]));
            PropertyEntries.Add(("Audible", "Looping"), typeof(bool));
            PropertyEntries.Add(("Audible", "Sound Event Name"), typeof(Crc));
            PropertyEntries.Add(("Audible", "Send Doppler"), typeof(bool));
            PropertyEntries.Add(("Audible", "0xAFCE4BA2"), typeof(int));
            PropertyEntries.Add(("Audible", "0xEC8964B3"), typeof(int));
            PropertyEntries.Add(("Audible", "0xE94E8608"), typeof(int));

            PropertyEntries.Add(("BigMap", "BigMapTexture"), typeof(Crc));

            PropertyEntries.Add(("Bird", "0x7741D08C"), typeof(float));
            PropertyEntries.Add(("Bird", "0x1B8ABD66"), typeof(float));
            PropertyEntries.Add(("Bird", "0x285807C8"), typeof(Crc));
            PropertyEntries.Add(("Bird", "Model"), typeof(Crc));
            PropertyEntries.Add(("Bird", "0xE8FBAB1F"), typeof(float));
            PropertyEntries.Add(("Bird", "0xBD4A934E"), typeof(float));
            PropertyEntries.Add(("Bird", "0xCD922F16"), typeof(float));
            PropertyEntries.Add(("Bird", "Scale"), typeof(float));
            PropertyEntries.Add(("Bird", "DeathFX"), typeof(Crc));
            PropertyEntries.Add(("Bird", "MaxSpeed"), typeof(float));
            PropertyEntries.Add(("Bird", "0xAAD8CCD8"), typeof(Vector2));
            PropertyEntries.Add(("Bird", "FX"), typeof(Crc));
            PropertyEntries.Add(("Bird", "0x7D09FBF0"), typeof(float));
            PropertyEntries.Add(("Bird", "0x8415C708"), typeof(Vector2));

            PropertyEntries.Add(("BirdSpawner", "0x3CDEDBED"), typeof(bool));
            PropertyEntries.Add(("BirdSpawner", "ClassName"), typeof(string));
            PropertyEntries.Add(("BirdSpawner", "0x9FC92EBA"), typeof(bool));

            PropertyEntries.Add(("BonePair", "BonePair"), typeof(Crc));
            PropertyEntries.Add(("BonePair", "BonePairList"), typeof(int));
            PropertyEntries.Add(("BonePair", "StartBone"), typeof(Crc));
            PropertyEntries.Add(("BonePair", "EndBone"), typeof(Crc));

            PropertyEntries.Add(("BridgeController", "0x6A582E8F"), typeof(float));
            PropertyEntries.Add(("BridgeController", "0x30057699"), typeof(char[]));
            PropertyEntries.Add(("BridgeController", "0x2BF29A61"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0x2B81C6D2"), typeof(int));
            PropertyEntries.Add(("BridgeController", "FadeOutTime"), typeof(float));
            PropertyEntries.Add(("BridgeController", "0x20BDCCE5"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0x537F205A"), typeof(float));
            PropertyEntries.Add(("BridgeController", "0x51C72ACE"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0x41754A65"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0x45C52D5C"), typeof(byte));
            PropertyEntries.Add(("BridgeController", "0xF15282D0"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0xDE72B5FA"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0xD81FD9CA"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0xDCDC8BCA"), typeof(float));
            PropertyEntries.Add(("BridgeController", "0xA6E31CA5"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0x9DC09A87"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0xA3454BFF"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0x9BDA60CA"), typeof(float));
            PropertyEntries.Add(("BridgeController", "0x6D32B59F"), typeof(Crc));
            PropertyEntries.Add(("BridgeController", "0x9AD419A5"), typeof(Crc));

            PropertyEntries.Add(("Bullet", "0x69DCA2B6"), typeof(Crc));
            PropertyEntries.Add(("Bullet", "0x5E6A93DB"), typeof(bool));
            PropertyEntries.Add(("Bullet", "0x4F7CB322"), typeof(int));
            PropertyEntries.Add(("Bullet", "Color"), typeof(Color));
            PropertyEntries.Add(("Bullet", "0x4C2B3699"), typeof(float));
            PropertyEntries.Add(("Bullet", "0x32FDED30"), typeof(bool));
            PropertyEntries.Add(("Bullet", "0x232BC103"), typeof(float));
            PropertyEntries.Add(("Bullet", "AI DmgFallOff Range"), typeof(Vector2));
            PropertyEntries.Add(("Bullet", "0x08775F0D"), typeof(float));
            PropertyEntries.Add(("Bullet", "SPEED"), typeof(float));
            PropertyEntries.Add(("Bullet", "MaxDistance"), typeof(float));
            PropertyEntries.Add(("Bullet", "0xBA52C3D5"), typeof(float));
            PropertyEntries.Add(("Bullet", "DmgFallOff Range"), typeof(Vector2));
            PropertyEntries.Add(("Bullet", "Ricochet"), typeof(Crc));
            PropertyEntries.Add(("Bullet", "AI DmgFallOff"), typeof(Vector2));
            PropertyEntries.Add(("Bullet", "0xE47A9E34"), typeof(float));
            PropertyEntries.Add(("Bullet", "0xD4F11960"), typeof(float));
            PropertyEntries.Add(("Bullet", "0xD850F531"), typeof(Crc));
            PropertyEntries.Add(("Bullet", "DmgFallOff"), typeof(Vector2));
            PropertyEntries.Add(("Bullet", "Tracer Width"), typeof(float)); // unused?
            PropertyEntries.Add(("Bullet", "Tracer Interval"), typeof(float)); // unused?

            PropertyEntries.Add(("ButtonPrompt", "Delay"), typeof(float));
            PropertyEntries.Add(("ButtonPrompt", "Duration"), typeof(float));
            PropertyEntries.Add(("ButtonPrompt", "Prompt"), typeof(Crc));

            PropertyEntries.Add(("CameraSet", "RightWallCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "LowCoverCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "LadderCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "Melee Blur In"), typeof(float));
            PropertyEntries.Add(("CameraSet", "Melee Tweak Dist"), typeof(Vector2));
            PropertyEntries.Add(("CameraSet", "LeftCornerCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "BareHandsStealthKillCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "BareHandsMeleeCamera1"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "RightCornerCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "BareHandsCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "Use Group Camera"), typeof(bool));
            PropertyEntries.Add(("CameraSet", "LeftWallCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "VerticalBeamCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "BareHandsSprintCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "ShootingCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "HangCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "RoofTopCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "SwimmingCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "JumpCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "FlyCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "WallCoverCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "0x63784F61"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "BareHandsMeleeCamera4"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "TightShootingCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "HideCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "SightedInCamera"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "Melee Blur Out"), typeof(float));
            PropertyEntries.Add(("CameraSet", "BareHandsMeleeCamera3"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "Melee Blur Timeout"), typeof(float));
            PropertyEntries.Add(("CameraSet", "BareHandsMeleeCamera2"), typeof(Crc));
            PropertyEntries.Add(("CameraSet", "ConversationCamera"), typeof(Crc));

            PropertyEntries.Add(("CameraSettings", "CameraSettings"), typeof(Crc));
            PropertyEntries.Add(("CameraSettings", "0x70092C52"), typeof(Crc));
            PropertyEntries.Add(("CameraSettings", "CameraTrackCenter"), typeof(Vector3));
            PropertyEntries.Add(("CameraSettings", "0x1B457A75"), typeof(bool));
            PropertyEntries.Add(("CameraSettings", "FocusRotSpeed"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "WallPushLength"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "LookOffset2"), typeof(Vector3));
            PropertyEntries.Add(("CameraSettings", "RotSpeed"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "Offset1"), typeof(Vector3));
            PropertyEntries.Add(("CameraSettings", "Back Far Plane"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "CameraAimTension"), typeof(Vector3));
            PropertyEntries.Add(("CameraSettings", "FocusableCamera"), typeof(bool));
            PropertyEntries.Add(("CameraSettings", "Front Far Plane"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "Front Near Plane"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "Opacity"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "PitchSpeed"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "Pitch1"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "Pitch3"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "0x5832E0D8"), typeof(Crc));
            PropertyEntries.Add(("CameraSettings", "RestPitchWhenStill"), typeof(bool));
            PropertyEntries.Add(("CameraSettings", "Pitch2"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "YawRange"), typeof(Vector2));
            PropertyEntries.Add(("CameraSettings", "FocusPitchSpeed"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "PitchRange"), typeof(Vector2));
            PropertyEntries.Add(("CameraSettings", "0xA4F21228"), typeof(bool));
            PropertyEntries.Add(("CameraSettings", "Offset2"), typeof(Vector3));
            PropertyEntries.Add(("CameraSettings", "0x7D41B349"), typeof(Crc));
            PropertyEntries.Add(("CameraSettings", "FOV"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "RestingPitch"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "CameraMoveTension"), typeof(Vector3));
            PropertyEntries.Add(("CameraSettings", "Offset3"), typeof(Vector3));
            PropertyEntries.Add(("CameraSettings", "Back Near Plane"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "CameraFOVTension"), typeof(float));
            PropertyEntries.Add(("CameraSettings", "LookOffset1"), typeof(Vector3));
            PropertyEntries.Add(("CameraSettings", "LookOffset3"), typeof(Vector3));
            PropertyEntries.Add(("CameraSettings", "SafePointOffset"), typeof(Vector3));
            PropertyEntries.Add(("CameraSettings", "Empty"), typeof(Crc)); // unused
            PropertyEntries.Add(("CameraSettings", "0x58BCB46C"), typeof(int)); // unused?
            PropertyEntries.Add(("CameraSettings", "0xEACA428D"), typeof(Crc)); // unused?
            PropertyEntries.Add(("CameraSettings", "DepthBlurFilter"), typeof(Crc)); // unused?
            PropertyEntries.Add(("CameraSettings", "CameraTrackOffset"), typeof(Vector3)); // unused?
            PropertyEntries.Add(("CameraSettings", "FadeInTime"), typeof(float)); // unused?
            PropertyEntries.Add(("CameraSettings", "BlendInExp"), typeof(float)); // unused?
            PropertyEntries.Add(("CameraSettings", "BlendOutExp"), typeof(float)); // unused?

            PropertyEntries.Add(("CameraSettingsMisc", "Offset Gain"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "Descend Pitch Per Distance"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "Ascend Pitch"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "0x069650EE"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "0x2387D0AD"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "Descend Pitch"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "0x930F51B6"), typeof(Vector2));
            PropertyEntries.Add(("CameraSettingsMisc", "0x88E6A6CA"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "0x75DED125"), typeof(Vector2));
            PropertyEntries.Add(("CameraSettingsMisc", "Offset Max"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "Ascend Pitch Per Distance"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "0xB99955C4"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "0xCDD597DD"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "0xF4219F95"), typeof(float));
            PropertyEntries.Add(("CameraSettingsMisc", "Empty"), typeof(Crc)); // unused

            PropertyEntries.Add(("Car", "RaceCar"), typeof(bool));

            PropertyEntries.Add(("Cinematics", "Actor"), typeof(Crc));
            PropertyEntries.Add(("Cinematics", "0x6018F238"), typeof(byte));
            PropertyEntries.Add(("Cinematics", "0xC79442F1"), typeof(byte));
            PropertyEntries.Add(("Cinematics", "0xE5746957"), typeof(byte));
            PropertyEntries.Add(("Cinematics", "0xF21424F2"), typeof(byte));
            PropertyEntries.Add(("Cinematics", "Cinematic_HiRes"), typeof(Crc)); // unused?
            PropertyEntries.Add(("Cinematics", "0x9965E3D4"), typeof(int)); // unused?

            PropertyEntries.Add(("CivilianProp", "0x5E93C871"), typeof(bool));
            PropertyEntries.Add(("CivilianProp", "0x8C4CDBA1"), typeof(Crc));
            PropertyEntries.Add(("CivilianProp", "Model"), typeof(Crc));
            PropertyEntries.Add(("CivilianProp", "Label"), typeof(Crc));
            PropertyEntries.Add(("CivilianProp", "0x19B0426A"), typeof(Crc));
            PropertyEntries.Add(("CivilianProp", "0x2D0720BF"), typeof(Crc));

            PropertyEntries.Add(("ClothForce", "0x40359952"), typeof(Crc));
            PropertyEntries.Add(("ClothForce", "0xE0AA99D6"), typeof(float));
            PropertyEntries.Add(("ClothForce", "0xD88D5899"), typeof(float));
            PropertyEntries.Add(("ClothForce", "0xCEDABF19"), typeof(float));
            PropertyEntries.Add(("ClothForce", "0xCFF89665"), typeof(float));
            PropertyEntries.Add(("ClothForce", "0x810318DB"), typeof(float));
            PropertyEntries.Add(("ClothForce", "0x6DCDF8F7"), typeof(float));
            PropertyEntries.Add(("ClothForce", "0x50C3B0B7"), typeof(float));
            PropertyEntries.Add(("ClothForce", "0x08D78FD0"), typeof(float));
            PropertyEntries.Add(("ClothForce", "0x47A03D28"), typeof(float));

            PropertyEntries.Add(("ClothObject", "Model"), typeof(Crc));
            PropertyEntries.Add(("ClothObject", "0x2FBC51C3"), typeof(Crc));
            PropertyEntries.Add(("ClothObject", "0x5700AB72"), typeof(int));
            PropertyEntries.Add(("ClothObject", "0x523AFE65"), typeof(Crc));
            PropertyEntries.Add(("ClothObject", "ClothForce"), typeof(Crc));
            PropertyEntries.Add(("ClothObject", "0x8A7EA61A"), typeof(byte));
            PropertyEntries.Add(("ClothObject", "0xE0B51584"), typeof(Crc));
            PropertyEntries.Add(("ClothObject", "0xE3AEC3CD"), typeof(int));
            PropertyEntries.Add(("ClothObject", "0x3EEACE1A"), typeof(int));
            PropertyEntries.Add(("ClothObject", "0x283C9626"), typeof(int));
            PropertyEntries.Add(("ClothObject", "0x23A31F18"), typeof(int));
            PropertyEntries.Add(("ClothObject", "0x6C89C416"), typeof(int));
            PropertyEntries.Add(("ClothObject", "0xE11B7999"), typeof(Crc)); // unused?
            PropertyEntries.Add(("ClothObject", "0x1D679AEB"), typeof(float));
            PropertyEntries.Add(("ClothObject", "0x305856DF"), typeof(float));
            PropertyEntries.Add(("ClothObject", "0xEFE5D990"), typeof(Crc));
            PropertyEntries.Add(("ClothObject", "0xEAFC3A7D"), typeof(Crc));
            PropertyEntries.Add(("ClothObject", "0x69A86828"), typeof(int));
            PropertyEntries.Add(("ClothObject", "0x5AC2612B"), typeof(Vector3));
            PropertyEntries.Add(("ClothObject", "0xA0D456CB"), typeof(int));
            PropertyEntries.Add(("ClothObject", "MASS"), typeof(float));
            PropertyEntries.Add(("ClothObject", "DISTANCE"), typeof(float));
            PropertyEntries.Add(("ClothObject", "Offset"), typeof(float));
            PropertyEntries.Add(("ClothObject", "0xB9C2B47A"), typeof(Crc));

            PropertyEntries.Add(("Common", "dynamic"), typeof(bool));
            PropertyEntries.Add(("Common", "UseDynamicPool"), typeof(bool));
            PropertyEntries.Add(("Common", "priority"), typeof(int));
            PropertyEntries.Add(("Common", "backup"), typeof(Crc));
            PropertyEntries.Add(("Common", "managed"), typeof(bool));

            PropertyEntries.Add(("Controllable", "AI"), typeof(Crc));
            PropertyEntries.Add(("Controllable", "LuaTable"), typeof(Crc));
            PropertyEntries.Add(("Controllable", "LuaParam"), typeof(LuaParam));

            PropertyEntries.Add(("CreditName", "0x6C72334B"), typeof(int));
            PropertyEntries.Add(("CreditName", "Length"), typeof(float));
            PropertyEntries.Add(("CreditName", "Height"), typeof(float));

            PropertyEntries.Add(("Damageable", "DamageSettings"), typeof(Crc));
            PropertyEntries.Add(("Damageable", "0xB37BF5B2"), typeof(float));
            PropertyEntries.Add(("Damageable", "PartImpulse"), typeof(float));
            PropertyEntries.Add(("Damageable", "0xA1B82BA7"), typeof(Crc));
            PropertyEntries.Add(("Damageable", "0xAC0F66B8"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x08ECB69D"), typeof(float));
            PropertyEntries.Add(("Damageable", "InitialHealth"), typeof(float));
            PropertyEntries.Add(("Damageable", "ExplosionOnDeath"), typeof(Crc));
            PropertyEntries.Add(("Damageable", "0x7C203814"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x81CD9A01"), typeof(float));
            PropertyEntries.Add(("Damageable", "Weakness"), typeof(float));
            PropertyEntries.Add(("Damageable", "0xDA3A3781"), typeof(float));
            PropertyEntries.Add(("Damageable", "0xD0E3A1A1"), typeof(float));
            PropertyEntries.Add(("Damageable", "0xC11C102E"), typeof(float));
            PropertyEntries.Add(("Damageable", "0xC86877FD"), typeof(int));
            PropertyEntries.Add(("Damageable", "0xF7C72618"), typeof(float));
            PropertyEntries.Add(("Damageable", "0xF9C458E7"), typeof(float));
            PropertyEntries.Add(("Damageable", "0xFD4869CD"), typeof(Vector2));
            PropertyEntries.Add(("Damageable", "0x75791974"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x55817592"), typeof(Crc));
            PropertyEntries.Add(("Damageable", "0x6AC12B1E"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x73FF7621"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x54019561"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x4F18FA7A"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x496A25D2"), typeof(byte));
            PropertyEntries.Add(("Damageable", "0x49C74990"), typeof(Crc));
            PropertyEntries.Add(("Damageable", "0x4B186A63"), null);
            PropertyEntries.Add(("Damageable", "0x416E074F"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x2D96C4F2"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x2A3029F3"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x24E96E81"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x0E29263B"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x12066797"), typeof(float));
            PropertyEntries.Add(("Damageable", "AreaDamage"), typeof(Crc));
            PropertyEntries.Add(("Damageable", "0x3D960EA8"), typeof(float));
            PropertyEntries.Add(("Damageable", "0x414631B3"), typeof(int));
            PropertyEntries.Add(("Damageable", "0x8ECB69DB"), typeof(float));

            PropertyEntries.Add(("DamageablePart", "0x6DD070B1"), typeof(float));
            PropertyEntries.Add(("DamageablePart", "0x30812467"), typeof(char[]));
            PropertyEntries.Add(("DamageablePart", "0x3885F292"), typeof(bool));
            PropertyEntries.Add(("DamageablePart", "0x558C91D7"), typeof(char[]));
            PropertyEntries.Add(("DamageablePart", "0x22650220"), typeof(bool));
            PropertyEntries.Add(("DamageablePart", "0x03E19511"), typeof(float));
            PropertyEntries.Add(("DamageablePart", "0x187B88F6"), typeof(char[]));
            PropertyEntries.Add(("DamageablePart", "BoneName"), typeof(Crc));
            PropertyEntries.Add(("DamageablePart", "DamageWeakness"), typeof(float));
            PropertyEntries.Add(("DamageablePart", "0x7DB13D1F"), typeof(char[]));
            PropertyEntries.Add(("DamageablePart", "Pristine"), typeof(bool));
            PropertyEntries.Add(("DamageablePart", "DamageInitialHealth"), typeof(float));
            PropertyEntries.Add(("DamageablePart", "PartTimer"), typeof(float));
            PropertyEntries.Add(("DamageablePart", "DamageGroup"), typeof(Crc));

            PropertyEntries.Add(("DamageableRoot", "List"), typeof(Crc));
            PropertyEntries.Add(("DamageableRoot", "PartList"), typeof(Crc));
            PropertyEntries.Add(("DamageableRoot", "0xE3EB3242"), typeof(int));
            PropertyEntries.Add(("DamageableRoot", "GroupName"), typeof(Crc));
            PropertyEntries.Add(("DamageableRoot", "0x3C56B891"), typeof(Crc));
            PropertyEntries.Add(("DamageableRoot", "DamageGroups"), typeof(Crc));

            PropertyEntries.Add(("DamageRegion", "DamageIncreaseRate"), typeof(float));
            PropertyEntries.Add(("DamageRegion", "Duration"), typeof(float));
            PropertyEntries.Add(("DamageRegion", "Type"), typeof(Crc));
            PropertyEntries.Add(("DamageRegion", "DamageRate"), typeof(float));
            PropertyEntries.Add(("DamageRegion", "Emitter"), typeof(Crc));

            PropertyEntries.Add(("DamageSphere", "Radius"), typeof(float));
            PropertyEntries.Add(("DamageSphere", "ExpandRate"), typeof(float));

            PropertyEntries.Add(("Decal", "0xD3485F14"), typeof(float));
            PropertyEntries.Add(("Decal", "0xC6EC1E86"), typeof(float));
            PropertyEntries.Add(("Decal", "0x9DB73D8F"), typeof(float));
            PropertyEntries.Add(("Decal", "Normal Map"), typeof(Crc));
            PropertyEntries.Add(("Decal", "Size"), typeof(float));
            PropertyEntries.Add(("Decal", "Texture"), typeof(Crc));
            PropertyEntries.Add(("Decal", "0xD7D8D464"), typeof(float));
            PropertyEntries.Add(("Decal", "0xDF3262F2"), typeof(Vector3));
            PropertyEntries.Add(("Decal", "ST"), typeof(Vector4));

            PropertyEntries.Add(("DetailObject", "0xA90C7E48"), typeof(float));
            PropertyEntries.Add(("DetailObject", "0x440E11D7"), typeof(float));
            PropertyEntries.Add(("DetailObject", "Model"), typeof(Crc));
            PropertyEntries.Add(("DetailObject", "0x6DA91CF2"), typeof(float));
            PropertyEntries.Add(("DetailObject", "0xB7A38021"), typeof(float));
            PropertyEntries.Add(("DetailObject", "0xE0193B96"), typeof(bool));
            PropertyEntries.Add(("DetailObject", "0xF0FA376C"), typeof(float));
            PropertyEntries.Add(("DetailObject", "0x87DD9796"), typeof(bool)); // unused?
            PropertyEntries.Add(("DetailObject", "0x8A09514E"), typeof(bool)); // unused?

            PropertyEntries.Add(("Difficulty", "0x7F1A3641"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0x3208D523"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0x54586368"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0x6CA5A28E"), typeof(float));
            PropertyEntries.Add(("Difficulty", "Difficulty Level List"), typeof(int));
            PropertyEntries.Add(("Difficulty", "0x224AD76D"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0x1C985C43"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0x01EB8596"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0x1045459A"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0xB0D67841"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0x80235C30"), typeof(int));
            PropertyEntries.Add(("Difficulty", "0x860E70A3"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0xA892B18B"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0xCC919414"), typeof(float));
            PropertyEntries.Add(("Difficulty", "0xD7C38E8D"), typeof(float));
            PropertyEntries.Add(("Difficulty", "DifficultyLevel"), typeof(int));

            PropertyEntries.Add(("ElasticTransition", "ElasticTransition"), typeof(Crc));
            PropertyEntries.Add(("ElasticTransition", "0x6F6C645F"), typeof(Vector3));
            PropertyEntries.Add(("ElasticTransition", "0x4EF5360B"), typeof(float));
            PropertyEntries.Add(("ElasticTransition", "0x55DD23A8"), typeof(Vector3));
            PropertyEntries.Add(("ElasticTransition", "0x2DD64D12"), typeof(Vector3));
            PropertyEntries.Add(("ElasticTransition", "0x04D112AC"), typeof(float));
            PropertyEntries.Add(("ElasticTransition", "0x0E3ABED0"), typeof(float));
            PropertyEntries.Add(("ElasticTransition", "0xF73B0955"), typeof(Vector3));
            PropertyEntries.Add(("ElasticTransition", "0x8197FA7F"), typeof(byte));
            PropertyEntries.Add(("ElasticTransition", "0xD6F1AB54"), typeof(Vector3));
            PropertyEntries.Add(("ElasticTransition", "0xF741FF8D"), typeof(float));
            PropertyEntries.Add(("ElasticTransition", "0xF823CD96"), typeof(Vector3));

            PropertyEntries.Add(("Escalation", "0x7305C643"), typeof(float));
            PropertyEntries.Add(("Escalation", "Vehicle"), typeof(Crc));
            PropertyEntries.Add(("Escalation", "0x18A92078"), typeof(bool));
            PropertyEntries.Add(("Escalation", "0x07E2752A"), typeof(int));
            PropertyEntries.Add(("Escalation", "0x06F0936C"), typeof(float));
            PropertyEntries.Add(("Escalation", "0x1D5DD1C0"), typeof(int));
            PropertyEntries.Add(("Escalation", "0x67740C82"), typeof(float));
            PropertyEntries.Add(("Escalation", "0x62780BA3"), typeof(int));
            PropertyEntries.Add(("Escalation", "0x3204FD62"), typeof(float));
            PropertyEntries.Add(("Escalation", "0x3A527ED5"), typeof(Crc));
            PropertyEntries.Add(("Escalation", "0xE04DAEEB"), typeof(bool));
            PropertyEntries.Add(("Escalation", "0xE8020177"), typeof(Crc));
            PropertyEntries.Add(("Escalation", "0xEA355018"), typeof(Crc));
            PropertyEntries.Add(("Escalation", "0xE00E1D57"), typeof(Crc));
            PropertyEntries.Add(("Escalation", "0xB4F87903"), typeof(Crc));
            PropertyEntries.Add(("Escalation", "0xBD40121F"), typeof(bool));
            PropertyEntries.Add(("Escalation", "0xBE01F853"), typeof(float));
            PropertyEntries.Add(("Escalation", "0xAAADE1A8"), typeof(bool));
            PropertyEntries.Add(("Escalation", "0x99F65298"), typeof(int));
            PropertyEntries.Add(("Escalation", "0x9CF94678"), typeof(float));
            PropertyEntries.Add(("Escalation", "0xAA7CEDE2"), typeof(Crc));
            PropertyEntries.Add(("Escalation", "0x97303F8D"), typeof(float));
            PropertyEntries.Add(("Escalation", "0x7A0C1851"), typeof(Crc));
            PropertyEntries.Add(("Escalation", "0x89BCD2AA"), typeof(int));
            PropertyEntries.Add(("Escalation", "0x8C1C4A5B"), typeof(bool));

            PropertyEntries.Add(("EscHWTF", "0xF661BD0B"), typeof(Vector2));
            PropertyEntries.Add(("EscHWTF", "0xF705A3AA"), typeof(float));
            PropertyEntries.Add(("EscHWTF", "0xDA6B4D87"), typeof(int));
            PropertyEntries.Add(("EscHWTF", "0x89FAEAC2"), typeof(Crc));
            PropertyEntries.Add(("EscHWTF", "0xC37246F2"), typeof(Crc));
            PropertyEntries.Add(("EscHWTF", "0xD9A213BF"), typeof(float));
            PropertyEntries.Add(("EscHWTF", "0x7C0CF3AA"), typeof(Crc));
            PropertyEntries.Add(("EscHWTF", "0x49212E5A"), typeof(int));
            PropertyEntries.Add(("EscHWTF", "0x56D60972"), typeof(Crc));

            PropertyEntries.Add(("Explosion", "0xEE0E25BA"), typeof(float));
            PropertyEntries.Add(("Explosion", "0xEBB0A63E"), typeof(float));
            PropertyEntries.Add(("Explosion", "0xD8727836"), typeof(bool));
            PropertyEntries.Add(("Explosion", "Sound"), typeof(char[]));
            PropertyEntries.Add(("Explosion", "0xA1ED7451"), typeof(float));
            PropertyEntries.Add(("Explosion", "ExplosionForce"), typeof(float));
            PropertyEntries.Add(("Explosion", "Emitter"), typeof(Crc));
            PropertyEntries.Add(("Explosion", "0x79BA2FD0"), typeof(float));
            PropertyEntries.Add(("Explosion", "0x82EDC825"), typeof(bool));
            PropertyEntries.Add(("Explosion", "0x74F54615"), typeof(float));
            PropertyEntries.Add(("Explosion", "0x47E338FE"), typeof(float));
            PropertyEntries.Add(("Explosion", "0x5E244327"), typeof(float));
            PropertyEntries.Add(("Explosion", "0x62E0AFDE"), typeof(bool));
            PropertyEntries.Add(("Explosion", "0x349BF324"), typeof(float));
            PropertyEntries.Add(("Explosion", "0x2EBA9DF8"), typeof(float));
            PropertyEntries.Add(("Explosion", "Debris"), typeof(int));
            PropertyEntries.Add(("Explosion", "0x0FCE73F0"), typeof(float));
            PropertyEntries.Add(("Explosion", "ExplosionDamage"), typeof(float));

            PropertyEntries.Add(("FaceExpression", "Duration"), typeof(float));
            PropertyEntries.Add(("FaceExpression", "0xC023ACD3"), typeof(Crc));
            PropertyEntries.Add(("FaceExpression", "0xC2161E01"), typeof(float));

            PropertyEntries.Add(("FlameOrdnance", "FlameEffect"), typeof(Crc));
            PropertyEntries.Add(("FlameOrdnance", "Velocity"), typeof(float));
            PropertyEntries.Add(("FlameOrdnance", "DeAcceleration"), typeof(float));

            PropertyEntries.Add(("FlashMovie", "FlashFile"), typeof(Crc));
            PropertyEntries.Add(("FlashMovie", "Version"), typeof(int));

            PropertyEntries.Add(("Foliage", "OcclusionLight"), typeof(float));
            PropertyEntries.Add(("Foliage", "MaxSizeY"), typeof(float));
            PropertyEntries.Add(("Foliage", "MinRenderDist"), typeof(float));
            PropertyEntries.Add(("Foliage", "Type"), typeof(Crc));
            PropertyEntries.Add(("Foliage", "FoliageVolumeHP"), typeof(Crc));
            PropertyEntries.Add(("Foliage", "List"), typeof(Crc));
            PropertyEntries.Add(("Foliage", "0x1C0AEE5E"), typeof(Vector3));
            PropertyEntries.Add(("Foliage", "MaxColorBlue"), typeof(float));
            PropertyEntries.Add(("Foliage", "FED_Color"), typeof(Color));
            PropertyEntries.Add(("Foliage", "SpawnLimits"), typeof(Vector2));
            PropertyEntries.Add(("Foliage", "MinColorRed"), typeof(float));
            PropertyEntries.Add(("Foliage", "0x442FED33"), typeof(float));
            PropertyEntries.Add(("Foliage", "UseModel"), typeof(bool));
            PropertyEntries.Add(("Foliage", "Color"), typeof(Vector3[]));
            PropertyEntries.Add(("Foliage", "StartV"), typeof(float));
            PropertyEntries.Add(("Foliage", "Radius"), typeof(float));
            PropertyEntries.Add(("Foliage", "0x366C9D39"), typeof(float));
            PropertyEntries.Add(("Foliage", "MinRotation"), typeof(float));
            PropertyEntries.Add(("Foliage", "Model"), typeof(Crc));
            PropertyEntries.Add(("Foliage", "MaxColorRed"), typeof(float));
            PropertyEntries.Add(("Foliage", "MinColorBlue"), typeof(float));
            PropertyEntries.Add(("Foliage", "EndV"), typeof(float));
            PropertyEntries.Add(("Foliage", "0x58CDFFFA"), typeof(float));
            PropertyEntries.Add(("Foliage", "StartU"), typeof(float));
            PropertyEntries.Add(("Foliage", "Rotation"), typeof(Vector2));
            PropertyEntries.Add(("Foliage", "FoliageCluster"), typeof(Crc));
            PropertyEntries.Add(("Foliage", "MaxSpawnSphere"), typeof(float));
            PropertyEntries.Add(("Foliage", "MaxSizeX"), typeof(float));
            PropertyEntries.Add(("Foliage", "MaxColorAlpha"), typeof(float));
            PropertyEntries.Add(("Foliage", "LightVariation"), typeof(float));
            PropertyEntries.Add(("Foliage", "MaxParticleRotation"), typeof(float));
            PropertyEntries.Add(("Foliage", "EndU"), typeof(float));
            PropertyEntries.Add(("Foliage", "0xBB4D8134"), typeof(Crc));
            PropertyEntries.Add(("Foliage", "FoliageVolumeHPCount"), typeof(int));
            PropertyEntries.Add(("Foliage", "ParticleCount"), typeof(int));
            PropertyEntries.Add(("Foliage", "MinParticleRotation"), typeof(float));
            PropertyEntries.Add(("Foliage", "ParticleSkew"), typeof(Vector3));
            PropertyEntries.Add(("Foliage", "LOD"), typeof(int));
            PropertyEntries.Add(("Foliage", "MinSizeY"), typeof(float));
            PropertyEntries.Add(("Foliage", "Gradient"), typeof(float));
            PropertyEntries.Add(("Foliage", "0x98508662"), typeof(bool));
            PropertyEntries.Add(("Foliage", "CenterLight"), typeof(float));
            PropertyEntries.Add(("Foliage", "0x8B266E12"), typeof(bool));
            PropertyEntries.Add(("Foliage", "Size"), typeof(Vector4));
            PropertyEntries.Add(("Foliage", "MinSpawnBox"), typeof(Vector3));
            PropertyEntries.Add(("Foliage", "MinSpawnSphere"), typeof(float));
            PropertyEntries.Add(("Foliage", "MinSizeX"), typeof(float));
            PropertyEntries.Add(("Foliage", "MinColorGreen"), typeof(float));
            PropertyEntries.Add(("Foliage", "MaxColorGreen"), typeof(float));
            PropertyEntries.Add(("Foliage", "AlphaMode"), typeof(int));
            PropertyEntries.Add(("Foliage", "SEED"), typeof(int));
            PropertyEntries.Add(("Foliage", "RenderDist"), typeof(Vector2));
            PropertyEntries.Add(("Foliage", "StartUV"), typeof(Vector2));
            PropertyEntries.Add(("Foliage", "SphereLimits"), typeof(Vector2));
            PropertyEntries.Add(("Foliage", "MaxRotation"), typeof(float));
            PropertyEntries.Add(("Foliage", "Alpha"), typeof(Vector2));
            PropertyEntries.Add(("Foliage", "MinColorAlpha"), typeof(float));
            PropertyEntries.Add(("Foliage", "MaxSpawnBox"), typeof(Vector3));
            PropertyEntries.Add(("Foliage", "EndUV"), typeof(Vector2));
            PropertyEntries.Add(("Foliage", "SizeVariance"), typeof(float));
            PropertyEntries.Add(("Foliage", "MaxRenderDist"), typeof(float));
            PropertyEntries.Add(("Foliage", "Texture"), typeof(Crc));
            PropertyEntries.Add(("Foliage", "SpawnBox"), typeof(Vector3[]));
            PropertyEntries.Add(("Foliage", "Variations"), typeof(int));
            PropertyEntries.Add(("Foliage", "ParticleDirection"), typeof(Vector3));
            PropertyEntries.Add(("Foliage", "0xFD44ECDE"), typeof(float));
            PropertyEntries.Add(("Foliage", "SpecularPower"), typeof(float)); // unused?
            PropertyEntries.Add(("Foliage", "SpawnOffset"), typeof(Vector3)); // unused?
            PropertyEntries.Add(("Foliage", "RenderMode"), typeof(int)); // unused?
            PropertyEntries.Add(("Foliage", "NumTilesInRow"), typeof(int)); // unused?
            PropertyEntries.Add(("Foliage", "Layout"), typeof(int)); // unused?
            PropertyEntries.Add(("Foliage", "Clusters"), typeof(int)); // unused?

            PropertyEntries.Add(("FoliageFx", "0x664DE09A"), typeof(int));
            PropertyEntries.Add(("FoliageFx", "0x0C8091C3"), typeof(int));
            PropertyEntries.Add(("FoliageFx", "0x12AABDBB"), typeof(int));
            PropertyEntries.Add(("FoliageFx", "0x8E499CBD"), typeof(int));
            PropertyEntries.Add(("FoliageFx", "0xC3D9DCE4"), typeof(int));
            PropertyEntries.Add(("FoliageFx", "0xE33015E8"), typeof(int));

            PropertyEntries.Add(("FxBoneStateList", "Rank"), typeof(int));
            PropertyEntries.Add(("FxBoneStateList", "Bone_Neck"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "bone_head"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_HeadBase"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_Brow_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_Brow_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_OutBrow_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_OutBrow_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_Eye_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_Eye_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_LowLid_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_LowLid_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_UnderEye_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_UnderEye_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_EyeBlink_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_EyeBlink_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_Cheek_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_Cheek_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_LowerCheek_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_LowerCheek_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_UpperLip_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_UpperLip_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_LowerLip_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_LowerLip_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_LipCorner_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_LipCorner_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_InnerLowLip_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_InnerLowLip_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_InnerUpperLip_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_InnerUpperLip_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_OuterUpperLip_Left"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_OuterUpperLip_Right"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_Sneer"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_JawBone"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_MouthBase"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "0xD44E722A"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_Teeth_Top"), typeof(Crc));
            PropertyEntries.Add(("FxBoneStateList", "Bone_Teeth_Bottom"), typeof(Crc));

            PropertyEntries.Add(("FxHumanBodyPart", "ColorVariationsA"), typeof(int));
            PropertyEntries.Add(("FxHumanBodyPart", "ColorVariation"), typeof(Crc));
            PropertyEntries.Add(("FxHumanBodyPart", "Hat Particle"), typeof(Crc));
            PropertyEntries.Add(("FxHumanBodyPart", "0x18281B3B"), typeof(byte));
            PropertyEntries.Add(("FxHumanBodyPart", "Color"), typeof(Color));
            PropertyEntries.Add(("FxHumanBodyPart", "HighDetailModel"), typeof(Crc));
            PropertyEntries.Add(("FxHumanBodyPart", "LowDetailModel"), typeof(Crc));
            PropertyEntries.Add(("FxHumanBodyPart", "CombinedLODModel"), typeof(Crc));
            PropertyEntries.Add(("FxHumanBodyPart", "ColorRandWeight"), typeof(float));
            PropertyEntries.Add(("FxHumanBodyPart", "ColorVariationsB"), typeof(int));
            PropertyEntries.Add(("FxHumanBodyPart", "0xF2F6EAEC"), typeof(Crc));

            PropertyEntries.Add(("FxHumanBodySetup", "Face"), typeof(Crc));
            PropertyEntries.Add(("FxHumanBodySetup", "GearC"), typeof(int));
            PropertyEntries.Add(("FxHumanBodySetup", "0x37A9525D"), typeof(int));
            PropertyEntries.Add(("FxHumanBodySetup", "head"), typeof(int));
            PropertyEntries.Add(("FxHumanBodySetup", "CombinedLODModel"), typeof(int));
            PropertyEntries.Add(("FxHumanBodySetup", "SkinToneMax"), typeof(int));
            PropertyEntries.Add(("FxHumanBodySetup", "FxHumanHeadVariation"), typeof(Crc));
            PropertyEntries.Add(("FxHumanBodySetup", "BodyPart"), typeof(Crc));
            PropertyEntries.Add(("FxHumanBodySetup", "UpperBody"), typeof(int));
            PropertyEntries.Add(("FxHumanBodySetup", "FxHumanBodyPartVariation"), typeof(Crc));
            PropertyEntries.Add(("FxHumanBodySetup", "GearA"), typeof(int));
            PropertyEntries.Add(("FxHumanBodySetup", "LowerBody"), typeof(int));
            PropertyEntries.Add(("FxHumanBodySetup", "BodyPartProbability"), typeof(float));
            PropertyEntries.Add(("FxHumanBodySetup", "FaceProbability"), typeof(float));
            PropertyEntries.Add(("FxHumanBodySetup", "SkinToneMin"), typeof(int));
            PropertyEntries.Add(("FxHumanBodySetup", "GearB"), typeof(int));
            PropertyEntries.Add(("FxHumanBodySetup", "0xD4D4470A"), typeof(int));

            PropertyEntries.Add(("FxHumanHead", "ColorVariationsB"), typeof(int));
            PropertyEntries.Add(("FxHumanHead", "0xA714324C"), typeof(int));
            PropertyEntries.Add(("FxHumanHead", "ColorRandWeight"), typeof(float));
            PropertyEntries.Add(("FxHumanHead", "LowDetailModel"), typeof(Crc));
            PropertyEntries.Add(("FxHumanHead", "0xEE7D0BA3"), typeof(Crc));
            PropertyEntries.Add(("FxHumanHead", "0xF2F6EAEC"), typeof(Crc));
            PropertyEntries.Add(("FxHumanHead", "0xF83F6C44"), typeof(Crc));
            PropertyEntries.Add(("FxHumanHead", "0x6DA79337"), typeof(int));
            PropertyEntries.Add(("FxHumanHead", "Color"), typeof(Color));
            PropertyEntries.Add(("FxHumanHead", "HighDetailModel"), typeof(Crc));
            PropertyEntries.Add(("FxHumanHead", "ColorVariationsA"), typeof(int));
            PropertyEntries.Add(("FxHumanHead", "FaceFxModel"), typeof(Crc));
            PropertyEntries.Add(("FxHumanHead", "0x08C3EE6A"), typeof(float));
            PropertyEntries.Add(("FxHumanHead", "ColorVariation"), typeof(int));
            PropertyEntries.Add(("FxHumanHead", "FaceFxActor"), typeof(Crc));

            PropertyEntries.Add(("GlobalHumanParams", "SprintInputBreakDeg"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "MaxJumpVelocityChange"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0x734725B4"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0x6E6C1E76"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "ExtraHardLandVelocity"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "FallingGravityMult"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "KissTargetFacingAngleMax"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "WallScurryRunVel"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "SprintDirMaxDeltaMag"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "AutoCamStartBlending"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "SeanMeleeDamageMult"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0x620638F7"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "SideJumpHeightClamber"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "FallDamageRagdoll"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "BufferedMoveExpire"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "death timescale for sean"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "HandPinDist"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0x5B344D5B"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "death slowdown"), typeof(bool));
            PropertyEntries.Add(("GlobalHumanParams", "MinJumpHeight"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "death timescale"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "AutoCamDelayStopLooking"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "SwimEnterDepth"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "AutoCamBlendDuration"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "MaxSprintJumpVelocityChange"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "HardLandVelocity"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "RagdollCollisionSpeedGain"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0x2EA7EE38"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "WaterDragMul"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "MaxMeleeTargetPull"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "GrabDist"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "Gravity"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0x12FD7382"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "FallDamageMinSpeed"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "SwimSlowSpeed"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "CarJumpLength"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0x0BB652B3"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "SwimExitDepth"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0x0434677B"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "KissPlayerFacingAngleMax"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "WaterDragMinDepth"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "AimAnimMaxDeviationY"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "SprintSpeed"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "WallScurryRunDist"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0xACE10817"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "RunSpeed"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "WallScurryExtraDist"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0xAB5952AC"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0xA7617930"), typeof(Vector2));
            PropertyEntries.Add(("GlobalHumanParams", "SwimFastSpeed"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "WallScurryWalkDist"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "blocked hit sound"), typeof(Crc));
            PropertyEntries.Add(("GlobalHumanParams", "KissRadius"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "FootholdMaxDepth"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "RagdollCollisionSpeedLoss"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "AutoCamDelayStopAiming"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "MaxJumpGravityMult"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "Auto Reload Time"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "RagdollCollisionRadius"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "BackJumpHeightClamber"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "BackJumpLengthClamber"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "SprintInputSecs"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "FallScreamVel"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "AirResistanceMultiplier"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "FallDamageDeath"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "MinUpJumpHeightClamber"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "HandHangHeight"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "NeutralTargetAngle"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0xC978CC1C"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "AimAnimMaxDeviationX"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "FallDamageMult"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "WalkSpeed"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "WaterDragMaxDepth"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0xC41759B1"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "DirectionalTargetAngle"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "ImpactDamageMinSpeed"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "FootholdHeight"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "RagdollCollisionSpeedThresh"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "death scale duration"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "NaziMeleeDamageMult"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "ImpactDamageMult"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "MaxFallDamageRestTime"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "SideJumpLengthClamber"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "MinCarJumpHeight"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "0xE1B7A252"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "JumpRunSpeedDrag"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "ImpactDamageMinSpeedBelow"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "AutoCamDelayStartMoving"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "SprintInputMag"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "MinAirResistanceSpeed"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "AutoCamDelayFinishAnim"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "uniterr. hit sound"), typeof(Crc));
            PropertyEntries.Add(("GlobalHumanParams", "WallScurryWalkVel"), typeof(float));
            PropertyEntries.Add(("GlobalHumanParams", "MeleeTurnSpeed"), typeof(float));

            PropertyEntries.Add(("GroupCameraSettings", "View Yaw"), typeof(float));
            PropertyEntries.Add(("GroupCameraSettings", "GroupCenterTension"), typeof(Vector3));
            PropertyEntries.Add(("GroupCameraSettings", "Z Pull Tension"), typeof(float));
            PropertyEntries.Add(("GroupCameraSettings", "Min Subject Distance"), typeof(float));

            PropertyEntries.Add(("GroupTransition", "0xEB4DFB24"), typeof(Crc));

            PropertyEntries.Add(("GunnerSeat", "SecondWeaponYawLimits"), typeof(Vector2));
            PropertyEntries.Add(("GunnerSeat", "0x6C36BA37"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x699D6C8F"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x5D348D7A"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x64F9B346"), typeof(byte));
            PropertyEntries.Add(("GunnerSeat", "0x585F7DB9"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x536AF4EA"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "SecondaryWeapons"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "SecondWeaponPitchBone"), typeof(int));
            PropertyEntries.Add(("GunnerSeat", "0x3E2755DD"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "SecondWeaponPitchLimits"), typeof(Vector2));
            PropertyEntries.Add(("GunnerSeat", "SecondWeaponSeatMotorStrengthFactor"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x27B2EBEE"), typeof(bool));
            PropertyEntries.Add(("GunnerSeat", "0x25CA9693"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x1B9993D5"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "SecondaryWeaponBPrint"), typeof(int));
            PropertyEntries.Add(("GunnerSeat", "0x1B64EFAD"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x1A080E0B"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x1A446AB1"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x049038EA"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x0107C0D3"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x027FAADE"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "SecondWeaponFireBone"), typeof(int));
            PropertyEntries.Add(("GunnerSeat", "0x6EBF2C31"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0xCD8D678E"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "WeaponMotorStrengthFactor"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "SecondaryWeapon"), typeof(int));
            PropertyEntries.Add(("GunnerSeat", "0xB0C2D055"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "SecondWeaponWeaponMotorStrengthFactor"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0xAED17721"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "SecondWeaponYawBone"), typeof(int));
            PropertyEntries.Add(("GunnerSeat", "DefaultWeapon"), typeof(Crc));
            PropertyEntries.Add(("GunnerSeat", "SeatMotorStrengthFactor"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0x952F679E"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0xDFD6597A"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0xDF6B6D8D"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0xD8F11E45"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0xCEFCD0D9"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0xCF9A64E9"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0xEABA3AEF"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0xF79298AE"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "0xFB9E20AE"), typeof(float));
            PropertyEntries.Add(("GunnerSeat", "WeaponName"), typeof(Crc));

            PropertyEntries.Add(("Highlight", "0xEB42BD07"), typeof(int));
            PropertyEntries.Add(("Highlight", "0x64CD994E"), typeof(Crc));
            PropertyEntries.Add(("Highlight", "0xDED701DD"), typeof(bool));
            PropertyEntries.Add(("Highlight", "0x9DF1F45D"), typeof(bool));
            PropertyEntries.Add(("Highlight", "0x2B6FE1AE"), typeof(float));
            PropertyEntries.Add(("Highlight", "MaxDistance"), typeof(float));
            PropertyEntries.Add(("Highlight", "0x674AE58A"), typeof(float));
            PropertyEntries.Add(("Highlight", "0x22F2C045"), typeof(float));
            PropertyEntries.Add(("Highlight", "0x43B43E44"), typeof(Color));
            PropertyEntries.Add(("Highlight", "0x09CA3AD5"), typeof(float));
            PropertyEntries.Add(("Highlight", "0x02CF30C4"), typeof(Color));
            PropertyEntries.Add(("Highlight", "0x0952A055"), typeof(float));
            PropertyEntries.Add(("Highlight", "0x9CCC519B"), typeof(Color));
            PropertyEntries.Add(("Highlight", "0x67C2800A"), typeof(float));
            PropertyEntries.Add(("Highlight", "0x9469454A"), typeof(float));
            PropertyEntries.Add(("Highlight", "0xC0EB6A3A"), typeof(float));
            PropertyEntries.Add(("Highlight", "0xDDB15F1B"), typeof(Color));
            PropertyEntries.Add(("Highlight", "0xE823F441"), typeof(float));
            PropertyEntries.Add(("Highlight", "0x661C6BD6"), typeof(float));

            PropertyEntries.Add(("Human", "Labels"), typeof(Crc));
            PropertyEntries.Add(("Human", "HUDName"), typeof(Crc));
            PropertyEntries.Add(("Human", "ToLoadPriority"), typeof(int));
            PropertyEntries.Add(("Human", "0xD5B7F2AF"), typeof(int));
            PropertyEntries.Add(("Human", "0xC25EF593"), typeof(bool));
            PropertyEntries.Add(("Human", "Suspicion"), typeof(bool));
            PropertyEntries.Add(("Human", "Team"), typeof(int));
            PropertyEntries.Add(("Human", "0xB84A3449"), typeof(int));
            PropertyEntries.Add(("Human", "Ammo"), typeof(int));
            PropertyEntries.Add(("Human", "AICombatParams"), typeof(Crc));
            PropertyEntries.Add(("Human", "0xD2A2EC4D"), typeof(bool));
            PropertyEntries.Add(("Human", "Pistol Stow Anim"), typeof(Crc));
            PropertyEntries.Add(("Human", "SuspicionChangeGreenHighlight"), typeof(Crc));
            PropertyEntries.Add(("Human", "MeleeType"), typeof(Crc));
            PropertyEntries.Add(("Human", "MissionStarterHighlight"), typeof(Crc));
            PropertyEntries.Add(("Human", "0xE3E53FE0"), typeof(bool));
            PropertyEntries.Add(("Human", "0xE3982DED"), typeof(float));
            PropertyEntries.Add(("Human", "0xE04F2AA1"), typeof(int));
            PropertyEntries.Add(("Human", "BodySetup"), typeof(Crc));
            PropertyEntries.Add(("Human", "Flammable"), typeof(bool));
            PropertyEntries.Add(("Human", "PistolSlot"), typeof(Crc));
            PropertyEntries.Add(("Human", "0xAD02C909"), typeof(bool));
            PropertyEntries.Add(("Human", "RunsFromFire"), typeof(bool));
            PropertyEntries.Add(("Human", "0xA66C0F96"), typeof(float));
            PropertyEntries.Add(("Human", "0xA1EB4A60"), typeof(int));
            PropertyEntries.Add(("Human", "0xA61D707E"), typeof(Crc));
            PropertyEntries.Add(("Human", "0xB1170EEE"), typeof(int));
            PropertyEntries.Add(("Human", "RifleButt Bone"), typeof(Crc));
            PropertyEntries.Add(("Human", "0xB33D5C94"), typeof(int));
            PropertyEntries.Add(("Human", "Rifle Stow Anim"), typeof(Crc));
            PropertyEntries.Add(("Human", "Honkable"), typeof(bool));
            PropertyEntries.Add(("Human", "0x88277056"), typeof(int));
            PropertyEntries.Add(("Human", "SuspicionChangeOrangeHighlight"), typeof(Crc));
            PropertyEntries.Add(("Human", "Weight"), typeof(float));
            PropertyEntries.Add(("Human", "FxBodySetup"), typeof(Crc));
            PropertyEntries.Add(("Human", "0x9F7E509D"), typeof(Crc));
            PropertyEntries.Add(("Human", "0x92408EE1"), typeof(Crc));
            PropertyEntries.Add(("Human", "0x8FDD5479"), typeof(int));
            PropertyEntries.Add(("Human", "0x9196CE7D"), typeof(float));
            PropertyEntries.Add(("Human", "Voices"), typeof(Crc));
            PropertyEntries.Add(("Human", "0x79473CAE"), typeof(bool));
            PropertyEntries.Add(("Human", "0x790553C3"), typeof(float));
            PropertyEntries.Add(("Human", "Weapon"), typeof(Crc));
            PropertyEntries.Add(("Human", "Rifle Takeout Anim"), typeof(Crc));
            PropertyEntries.Add(("Human", "0x771C9E1F"), typeof(char[]));
            PropertyEntries.Add(("Human", "0x74E5FB06"), typeof(int));
            PropertyEntries.Add(("Human", "Pistol Takeout Anim"), typeof(Crc));
            PropertyEntries.Add(("Human", "0x6FDC0334"), typeof(bool));
            PropertyEntries.Add(("Human", "Item"), typeof(int));
            PropertyEntries.Add(("Human", "0x6D63FBFA"), typeof(float));
            PropertyEntries.Add(("Human", "Female"), typeof(bool));
            PropertyEntries.Add(("Human", "SuspicionChangeYellowHighlight"), typeof(Crc));
            PropertyEntries.Add(("Human", "SuspicionChangeRedHighlight"), typeof(Crc));
            PropertyEntries.Add(("Human", "character"), typeof(bool));
            PropertyEntries.Add(("Human", "0x63F4B09E"), typeof(int));
            PropertyEntries.Add(("Human", "isGooseSteppingNazi"), typeof(byte));
            PropertyEntries.Add(("Human", "SkeletonScale"), typeof(Crc));
            PropertyEntries.Add(("Human", "0x48330E09"), typeof(float));
            PropertyEntries.Add(("Human", "0x4C1FEF5B"), typeof(int));
            PropertyEntries.Add(("Human", "0x4700FEE0"), typeof(bool));
            PropertyEntries.Add(("Human", "0x3E5E2614"), typeof(int));
            PropertyEntries.Add(("Human", "0x377E4246"), null);
            PropertyEntries.Add(("Human", "0x37A088D3"), typeof(float));
            PropertyEntries.Add(("Human", "0x43F1B4A2"), typeof(Crc));
            PropertyEntries.Add(("Human", "0x37562519"), typeof(float));
            PropertyEntries.Add(("Human", "FloatSlot"), null);
            PropertyEntries.Add(("Human", "0x2ADDC0F9"), typeof(bool));
            PropertyEntries.Add(("Human", "0x2EFB7E33"), typeof(Crc));
            PropertyEntries.Add(("Human", "RifleTip Bone"), typeof(Crc));
            PropertyEntries.Add(("Human", "AnimBank"), typeof(int));
            PropertyEntries.Add(("Human", "Hot"), typeof(bool));
            PropertyEntries.Add(("Human", "0x0B03B982"), typeof(bool));
            PropertyEntries.Add(("Human", "HandSlot"), typeof(Crc));
            PropertyEntries.Add(("Human", "0x0A9CD9E0"), typeof(float));
            PropertyEntries.Add(("Human", "MagicSlot"), null);
            PropertyEntries.Add(("Human", "0x063C7CE7"), typeof(Crc));
            PropertyEntries.Add(("Human", "0x1E89A51C"), typeof(bool));
            PropertyEntries.Add(("Human", "RifleSlot"), typeof(Crc));
            PropertyEntries.Add(("Human", "RandomItem"), typeof(Crc));
            PropertyEntries.Add(("Human", "Pistol Bone"), typeof(Crc));

            PropertyEntries.Add(("HumanBodyPart", "Model"), typeof(Crc));
            PropertyEntries.Add(("HumanBodyPart", "LodModel"), typeof(Crc));
            PropertyEntries.Add(("HumanBodyPart", "Color"), typeof(Color));
            PropertyEntries.Add(("HumanBodyPart", "ColorRandWeight"), typeof(float));
            PropertyEntries.Add(("HumanBodyPart", "ColorVariation"), typeof(Crc));
            PropertyEntries.Add(("HumanBodyPart", "Color 2 variations"), typeof(int));
            PropertyEntries.Add(("HumanBodyPart", "Color 3 variations"), typeof(int));

            PropertyEntries.Add(("HumanBodySetup", "SkinTone"), typeof(Vector2));
            PropertyEntries.Add(("HumanBodySetup", "BodyPart"), typeof(Crc));
            PropertyEntries.Add(("HumanBodySetup", "Face"), typeof(Crc));
            PropertyEntries.Add(("HumanBodySetup", "BodyRandWeight"), typeof(float));
            PropertyEntries.Add(("HumanBodySetup", "FaceProbability"), typeof(float));
            PropertyEntries.Add(("HumanBodySetup", "HumanBodyPartVariation"), typeof(Crc));
            PropertyEntries.Add(("HumanBodySetup", "FxHumanHeadVariation"), typeof(Crc));
            PropertyEntries.Add(("HumanBodySetup", "FxHead"), typeof(int));
            PropertyEntries.Add(("HumanBodySetup", "head"), typeof(int));
            PropertyEntries.Add(("HumanBodySetup", "Upper Body"), typeof(int));
            PropertyEntries.Add(("HumanBodySetup", "Lower Body"), typeof(int));
            PropertyEntries.Add(("HumanBodySetup", "GearA"), typeof(int));
            PropertyEntries.Add(("HumanBodySetup", "GearB"), typeof(int));
            PropertyEntries.Add(("HumanBodySetup", "GearC"), typeof(int));

            PropertyEntries.Add(("HumanPhysics", "HumanPhysics"), typeof(Crc));
            PropertyEntries.Add(("HumanPhysics", "UseFastCollision"), typeof(bool));

            PropertyEntries.Add(("HumanSkeletonScale", "HeadScale"), typeof(float));
            PropertyEntries.Add(("HumanSkeletonScale", "BodyScale"), typeof(float));
            PropertyEntries.Add(("HumanSkeletonScale", "BoneScale"), typeof(float));
            PropertyEntries.Add(("HumanSkeletonScale", "Model"), typeof(Crc));
            PropertyEntries.Add(("HumanSkeletonScale", "Bone scales"), typeof(int));

            PropertyEntries.Add(("Item", "0x953C60F5"), typeof(float));
            PropertyEntries.Add(("Item", "SabotageExplosion"), typeof(int));
            PropertyEntries.Add(("Item", "Equippable"), typeof(bool));
            PropertyEntries.Add(("Item", "0x6ADE730A"), typeof(Crc));
            PropertyEntries.Add(("Item", "IsDetonatorItem"), typeof(bool));
            PropertyEntries.Add(("Item", "Display Name"), typeof(int));
            PropertyEntries.Add(("Item", "Special ID"), typeof(int));
            PropertyEntries.Add(("Item", "HiddenWhileEquiped"), typeof(bool));
            PropertyEntries.Add(("Item", "IsUniqueItem"), typeof(bool));
            PropertyEntries.Add(("Item", "Pickup High ANim"), typeof(Crc));
            PropertyEntries.Add(("Item", "Model"), typeof(int));
            PropertyEntries.Add(("Item", "0x5D4608B5"), typeof(bool));
            PropertyEntries.Add(("Item", "SlotType"), typeof(Crc));
            PropertyEntries.Add(("Item", "CanDrop"), typeof(bool));
            PropertyEntries.Add(("Item", "Pickup Low Anim"), typeof(Crc));
            PropertyEntries.Add(("Item", "Label"), typeof(Crc));
            PropertyEntries.Add(("Item", "PickupHighlight"), typeof(Crc));

            PropertyEntries.Add(("ItemCache", "Display Name"), typeof(Crc));
            PropertyEntries.Add(("ItemCache", "0x97EE12C6"), typeof(bool));

            PropertyEntries.Add(("LeafSpawner", "0x14E198F5"), typeof(Crc));
            PropertyEntries.Add(("LeafSpawner", "Radius"), typeof(float));
            PropertyEntries.Add(("LeafSpawner", "0x9DB73D8F"), typeof(float));

            PropertyEntries.Add(("LightAttachement", "Light"), typeof(int));
            PropertyEntries.Add(("LightAttachement", "LightBone"), typeof(int));
            PropertyEntries.Add(("LightAttachement", "LightAttachement"), typeof(int));
            PropertyEntries.Add(("LightAttachement", "LightRotationSpeed"), typeof(float));
            PropertyEntries.Add(("LightAttachement", "LightRotationBone"), typeof(int));
            PropertyEntries.Add(("LightAttachement", "LightRotationAngleLimit"), typeof(float));
            PropertyEntries.Add(("LightAttachement", "Light Attachements"), typeof(int));

            PropertyEntries.Add(("LightHalo", "Anim Speed"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0xC5B44A23"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0xB93F2EFB"), typeof(byte));
            PropertyEntries.Add(("LightHalo", "0xAD13EC8C"), typeof(float));
            PropertyEntries.Add(("LightHalo", "Overbright"), typeof(float));
            PropertyEntries.Add(("LightHalo", "MaxFullViewableAngle"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0x905B3312"), typeof(float));
            PropertyEntries.Add(("LightHalo", "Anim Min Intensity"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0x8ACA5BCA"), typeof(float));
            PropertyEntries.Add(("LightHalo", "Rotation"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0x8978B4C4"), typeof(Color));
            PropertyEntries.Add(("LightHalo", "0xE7F46F43"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0xE30917DE"), typeof(Crc));
            PropertyEntries.Add(("LightHalo", "Height"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0xD0822FCD"), typeof(Color));
            PropertyEntries.Add(("LightHalo", "0xD3D07581"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0xFCC5D7DE"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0xED3DCB8A"), typeof(float));
            PropertyEntries.Add(("LightHalo", "Texture"), typeof(Crc));
            PropertyEntries.Add(("LightHalo", "0xFF8ABD17"), typeof(byte));
            PropertyEntries.Add(("LightHalo", "0x7702A37E"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0x76E3E09E"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0x753DC701"), typeof(float));
            PropertyEntries.Add(("LightHalo", "MaxViewableAngle"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0x74E42872"), typeof(float));
            PropertyEntries.Add(("LightHalo", "AnimType"), typeof(Crc));
            PropertyEntries.Add(("LightHalo", "0x67F3F28B"), typeof(byte));
            PropertyEntries.Add(("LightHalo", "dynamic"), typeof(byte));
            PropertyEntries.Add(("LightHalo", "Color"), typeof(Color));
            PropertyEntries.Add(("LightHalo", "LWTFColor"), typeof(Color));
            PropertyEntries.Add(("LightHalo", "Radius"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0x26F78C7B"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0x1F89A84E"), typeof(Color));
            PropertyEntries.Add(("LightHalo", "0x21B7E323"), typeof(float));
            PropertyEntries.Add(("LightHalo", "FadeOut"), typeof(byte));
            PropertyEntries.Add(("LightHalo", "Color2"), typeof(Color));
            PropertyEntries.Add(("LightHalo", "0x0DEC56C7"), typeof(int));
            PropertyEntries.Add(("LightHalo", "0x2FD9B489"), typeof(float));
            PropertyEntries.Add(("LightHalo", "DepthBias"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0x280BF82C"), typeof(float));
            PropertyEntries.Add(("LightHalo", "0x3229D516"), typeof(Crc));
            PropertyEntries.Add(("LightHalo", "FilamentRadius"), typeof(float));

            PropertyEntries.Add(("LightHaloAttachment", "LightHaloRotationSpeed"), typeof(float));
            PropertyEntries.Add(("LightHaloAttachment", "LightHaloRotationBone"), typeof(int));
            PropertyEntries.Add(("LightHaloAttachment", "LightHaloBone"), typeof(int));
            PropertyEntries.Add(("LightHaloAttachment", "LightHalo"), typeof(int));
            PropertyEntries.Add(("LightHaloAttachment", "LightHaloAttachment"), typeof(int));
            PropertyEntries.Add(("LightHaloAttachment", "LightHaloRotationAngleLimit"), typeof(float));
            PropertyEntries.Add(("LightHaloAttachment", "Light Halo Attachments"), typeof(int));

            PropertyEntries.Add(("LightSettings", "ConeOnly"), typeof(byte));
            PropertyEntries.Add(("LightSettings", "ConeFallOffMax"), typeof(float));
            PropertyEntries.Add(("LightSettings", "NightTime Specular Color"), typeof(Color));
            PropertyEntries.Add(("LightSettings", "Color"), typeof(Color));
            PropertyEntries.Add(("LightSettings", "ConeEdgeFade"), typeof(float));
            PropertyEntries.Add(("LightSettings", "DayTime Specular Color"), typeof(Color));
            PropertyEntries.Add(("LightSettings", "Radius"), typeof(float));
            PropertyEntries.Add(("LightSettings", "ConeLength"), typeof(float));
            PropertyEntries.Add(("LightSettings", "0x2CFA7B6B"), typeof(float));
            PropertyEntries.Add(("LightSettings", "0x15667836"), typeof(float));
            PropertyEntries.Add(("LightSettings", "NightTime Color"), typeof(Color));
            PropertyEntries.Add(("LightSettings", "Specular Color"), typeof(Color));
            PropertyEntries.Add(("LightSettings", "ConeScrollSpeed"), typeof(float));
            PropertyEntries.Add(("LightSettings", "intensity"), typeof(float));
            PropertyEntries.Add(("LightSettings", "ConeFallOffMin"), typeof(float));
            PropertyEntries.Add(("LightSettings", "0x69282DF5"), typeof(byte));
            PropertyEntries.Add(("LightSettings", "LWTF Intensity"), typeof(float));
            PropertyEntries.Add(("LightSettings", "0x63A27078"), typeof(byte));
            PropertyEntries.Add(("LightSettings", "Near Plane"), typeof(float));
            PropertyEntries.Add(("LightSettings", "DayTime Color"), typeof(Color));
            PropertyEntries.Add(("LightSettings", "0x52FCF5D4"), typeof(float));
            PropertyEntries.Add(("LightSettings", "Group"), typeof(int));
            PropertyEntries.Add(("LightSettings", "0x73E741E3"), typeof(byte));
            PropertyEntries.Add(("LightSettings", "0x762D7CD9"), typeof(float));
            PropertyEntries.Add(("LightSettings", "NightTime Intensity"), typeof(float));
            PropertyEntries.Add(("LightSettings", "AnimType"), typeof(Crc));
            PropertyEntries.Add(("LightSettings", "0x725EAA9B"), typeof(float));
            PropertyEntries.Add(("LightSettings", "Start U"), typeof(float));
            PropertyEntries.Add(("LightSettings", "0x7A9D55F3"), typeof(byte));
            PropertyEntries.Add(("LightSettings", "LWTF Specular Color"), typeof(Color));
            PropertyEntries.Add(("LightSettings", "HasCone"), typeof(byte));
            PropertyEntries.Add(("LightSettings", "Specular Intensity"), typeof(float));
            PropertyEntries.Add(("LightSettings", "Width"), typeof(float));
            PropertyEntries.Add(("LightSettings", "Anim Min Intensity"), typeof(float));
            PropertyEntries.Add(("LightSettings", "0x976E00A1"), typeof(float));
            PropertyEntries.Add(("LightSettings", "OuterAngle"), typeof(float));
            PropertyEntries.Add(("LightSettings", "LWTF Color"), typeof(Color));
            PropertyEntries.Add(("LightSettings", "LWTF Specular Intensity"), typeof(float));
            PropertyEntries.Add(("LightSettings", "DayTime Intensity"), typeof(float));
            PropertyEntries.Add(("LightSettings", "0xACFFC219"), typeof(float));
            PropertyEntries.Add(("LightSettings", "NightTime Specular Intensity"), typeof(float));
            PropertyEntries.Add(("LightSettings", "CubeMapTexture"), typeof(char[]));
            PropertyEntries.Add(("LightSettings", "DayTime Specular Intensity"), typeof(float));
            PropertyEntries.Add(("LightSettings", "ConeAlphaMultiplier"), typeof(float));
            PropertyEntries.Add(("LightSettings", "InnerAngle"), typeof(float));
            PropertyEntries.Add(("LightSettings", "Start V"), typeof(float));
            PropertyEntries.Add(("LightSettings", "0xF7688793"), typeof(float));
            PropertyEntries.Add(("LightSettings", "LightType"), typeof(Crc));
            PropertyEntries.Add(("LightSettings", "On"), typeof(bool));
            PropertyEntries.Add(("LightSettings", "0xF0758D60"), typeof(byte));
            PropertyEntries.Add(("LightSettings", "CastShadow"), typeof(byte));
            PropertyEntries.Add(("LightSettings", "Height"), typeof(float));
            PropertyEntries.Add(("LightSettings", "0xD1488AB4"), typeof(int));
            PropertyEntries.Add(("LightSettings", "ConeColor"), typeof(Color));
            PropertyEntries.Add(("LightSettings", "Anim Speed"), typeof(float));

            PropertyEntries.Add(("LightVolume", "0x7D90365D"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x782741C7"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x69AB9F30"), typeof(bool));
            PropertyEntries.Add(("LightVolume", "0x46CCAA90"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x6086BEE4"), typeof(bool));
            PropertyEntries.Add(("LightVolume", "0x353B440D"), typeof(Vector2));
            PropertyEntries.Add(("LightVolume", "interactive"), typeof(bool));
            PropertyEntries.Add(("LightVolume", "0x3484922D"), typeof(Vector2));
            PropertyEntries.Add(("LightVolume", "Highlight"), typeof(Crc));
            PropertyEntries.Add(("LightVolume", "0x0BDC2B41"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x1098118E"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0xB333BBA2"), typeof(Vector2));
            PropertyEntries.Add(("LightVolume", "0xB258547F"), typeof(Vector3));
            PropertyEntries.Add(("LightVolume", "0xB27D09C2"), typeof(Vector2));
            PropertyEntries.Add(("LightVolume", "0xB1AC0908"), typeof(Vector2));
            PropertyEntries.Add(("LightVolume", "0x7E3618EC"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x82E72ED1"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x99A76612"), typeof(Vector2));
            PropertyEntries.Add(("LightVolume", "0xD8A0A867"), typeof(Crc));
            PropertyEntries.Add(("LightVolume", "0xBBA9DA2F"), typeof(Vector2));
            PropertyEntries.Add(("LightVolume", "0xCEC7707A"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0xCF2FE183"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0xEBD77BB3"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0xFB88ADF2"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x6BCD7814"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x3AD49195"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x03BCDA2A"), typeof(Color));
            PropertyEntries.Add(("LightVolume", "0x1F89A84E"), typeof(Color));
            PropertyEntries.Add(("LightVolume", "0x66C720F2"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x8C8BD14A"), typeof(Color));
            PropertyEntries.Add(("LightVolume", "0xB1B3B678"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0xEDC59AF4"), typeof(float));
            PropertyEntries.Add(("LightVolume", "0x622DA6EE"), typeof(Crc));

            PropertyEntries.Add(("Melee", "ReadyTime"), typeof(Vector2));
            PropertyEntries.Add(("Melee", "AttackRandom"), typeof(float));
            PropertyEntries.Add(("Melee", "BlockDuringReady"), typeof(float));
            PropertyEntries.Add(("Melee", "AIRxnType"), typeof(Crc));
            PropertyEntries.Add(("Melee", "AttackTime"), typeof(Vector2));
            PropertyEntries.Add(("Melee", "StrongAttack"), typeof(float));
            PropertyEntries.Add(("Melee", "CircleToBack"), typeof(bool));
            PropertyEntries.Add(("Melee", "Melee_AIReaction"), typeof(Crc));
            PropertyEntries.Add(("Melee", "BlockDuringAttack"), typeof(float));
            PropertyEntries.Add(("Melee", "BlockRandom"), typeof(float));
            PropertyEntries.Add(("Melee", "WeakAttack"), typeof(float));
            PropertyEntries.Add(("Melee", "BlockTime"), typeof(Vector2));
            PropertyEntries.Add(("Melee", "BlockDuringBlock"), typeof(float));
            PropertyEntries.Add(("Melee", "InitiallyGrabbable"), typeof(bool));
            PropertyEntries.Add(("Melee", "AIRxnWeight"), typeof(float));
            PropertyEntries.Add(("Melee", "GrabTimeMult"), typeof(float));
            PropertyEntries.Add(("Melee", "BlockArc"), typeof(float));
            PropertyEntries.Add(("Melee", "BlockTargetDistanceMult"), typeof(float));
            PropertyEntries.Add(("Melee", "AttackTargetWidthMult"), typeof(float));

            PropertyEntries.Add(("MeleeWeapon", "hits to break"), typeof(int));
            PropertyEntries.Add(("MeleeWeapon", "two handed"), typeof(bool));
            PropertyEntries.Add(("MeleeWeapon", "NoWillToFight"), typeof(bool));
            PropertyEntries.Add(("MeleeWeapon", "Melee Type"), typeof(Crc));
            PropertyEntries.Add(("MeleeWeapon", "damage multiplier"), typeof(float));
            PropertyEntries.Add(("MeleeWeapon", "FlashIcon"), typeof(Crc));

            PropertyEntries.Add(("MiniGame", "CameraOffset"), typeof(Vector3));
            PropertyEntries.Add(("MiniGame", "0x493400CD"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "0x4EDD8F8D"), typeof(bool));
            PropertyEntries.Add(("MiniGame", "0x35B4A4E2"), typeof(float));
            PropertyEntries.Add(("MiniGame", "IconIndex"), typeof(int));
            PropertyEntries.Add(("MiniGame", "ObjFailAnim"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "0x344BEE18"), typeof(bool));
            PropertyEntries.Add(("MiniGame", "SuspicionType"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "0x0CC154BC"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "NumberRotations"), typeof(Vector2));
            PropertyEntries.Add(("MiniGame", "RotateClockwise"), typeof(bool));
            PropertyEntries.Add(("MiniGame", "0x0429A59B"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "IdleAnim"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "SpeedMultiplier"), typeof(float));
            PropertyEntries.Add(("MiniGame", "AutoIncrement"), typeof(bool));
            PropertyEntries.Add(("MiniGame", "InputAdjustment"), typeof(float));
            PropertyEntries.Add(("MiniGame", "0x8D9B17F3"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "MiniGameType"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "0x864AC31B"), typeof(bool));
            PropertyEntries.Add(("MiniGame", "ObjPassAnim"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "Fuse"), typeof(bool));
            PropertyEntries.Add(("MiniGame", "DegradeSpeed"), typeof(float));
            PropertyEntries.Add(("MiniGame", "IntoAnim"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "DegradeToZero"), typeof(bool));
            PropertyEntries.Add(("MiniGame", "CanDisarm"), typeof(bool));
            PropertyEntries.Add(("MiniGame", "OutofAnim"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "0xE832FB02"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "FailAnim"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "CompletionTime"), typeof(float));
            PropertyEntries.Add(("MiniGame", "0xE2612F04"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "InputThreshold"), typeof(Vector2));
            PropertyEntries.Add(("MiniGame", "PassAnim"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "Adjustable Time"), typeof(bool));
            PropertyEntries.Add(("MiniGame", "0xEE032533"), typeof(bool));
            PropertyEntries.Add(("MiniGame", "0xEF03C774"), typeof(Crc));
            PropertyEntries.Add(("MiniGame", "LockOnComplete"), typeof(bool));

            PropertyEntries.Add(("ModelRenderable", "Model"), typeof(Crc));
            PropertyEntries.Add(("ModelRenderable", "NoWillToFight"), typeof(bool));

            PropertyEntries.Add(("Ordnance", "Sabotage Minigame"), typeof(Crc));
            PropertyEntries.Add(("Ordnance", "0x336E0ADA"), typeof(float));
            PropertyEntries.Add(("Ordnance", "Force"), typeof(float));
            PropertyEntries.Add(("Ordnance", "MidLightMG"), typeof(Crc));
            PropertyEntries.Add(("Ordnance", "Flare Effect"), typeof(Crc));
            PropertyEntries.Add(("Ordnance", "GroundSabotageMG"), typeof(Crc));
            PropertyEntries.Add(("Ordnance", "GroundLightMG"), typeof(Crc));
            PropertyEntries.Add(("Ordnance", "Light Minigame"), typeof(Crc));
            PropertyEntries.Add(("Ordnance", "MaxDistance"), typeof(float));
            PropertyEntries.Add(("Ordnance", "AmmoType"), typeof(Crc));
            PropertyEntries.Add(("Ordnance", "MidSabotageMG"), typeof(Crc));
            PropertyEntries.Add(("Ordnance", "AIDamage"), typeof(float));
            PropertyEntries.Add(("Ordnance", "AISpeed"), typeof(float));
            PropertyEntries.Add(("Ordnance", "0xE9DBFBD0"), typeof(float));
            PropertyEntries.Add(("Ordnance", "0xEFF83A31"), typeof(Vector2));
            PropertyEntries.Add(("Ordnance", "Damage"), typeof(float));
            PropertyEntries.Add(("Ordnance", "MaximumAmount"), typeof(int));

            PropertyEntries.Add(("ParticleEffect", "Particle Set List"), typeof(int));
            PropertyEntries.Add(("ParticleEffect", "Emitter"), typeof(char[]));
            PropertyEntries.Add(("ParticleEffect", "Emitters"), typeof(int));
            PropertyEntries.Add(("ParticleEffect", "SET"), typeof(int));
            PropertyEntries.Add(("ParticleEffect", "0x37077436"), typeof(Crc));

            PropertyEntries.Add(("ParticleEffectSpawner", "Damage Region"), typeof(Crc));
            PropertyEntries.Add(("ParticleEffectSpawner", "ParticleEffect"), typeof(Crc));
            PropertyEntries.Add(("ParticleEffectSpawner", "SpawnInterval"), typeof(Vector2));
            PropertyEntries.Add(("ParticleEffectSpawner", "MaxDistance"), typeof(float));
            PropertyEntries.Add(("ParticleEffectSpawner", "ParticleEffectSpawnerPart"), typeof(Crc));
            PropertyEntries.Add(("ParticleEffectSpawner", "Effects"), typeof(int));

            PropertyEntries.Add(("PerkFactors", "0x75522F59"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x6447FB46"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x6F15FBE7"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x7387DF05"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x62BCEC63"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x4AD025E7"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x459DEE09"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0x43183CB4"), typeof(int));
            PropertyEntries.Add(("PerkFactors", "0x44FA72C8"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0x3BD59983"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0x3B650848"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0x38C95B83"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x2E2BA7C4"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x361DFE33"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0x2B5E9FBF"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x22D75E37"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x07153895"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x00AB9A20"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0x05A9A969"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xD4279D41"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xC9519C52"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xCCDD79CF"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xCF1AB351"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xC22609B3"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xA7D41104"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xA83E5FCB"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xA8E66F6B"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xA21776AC"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xA06B069D"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xA0AF0C71"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xA1D01048"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x9CFA471A"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0x795B115A"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0x968646DF"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0x9C7B628A"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xE6A76EDD"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xE505C244"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xE60187CB"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xE69A8743"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xE2669BE2"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xD653701A"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xDC9E11A2"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xDFA39497"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xFA3B5F67"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xEEA4FEF6"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xEF42C1C8"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xF0BB8F39"), typeof(float));
            PropertyEntries.Add(("PerkFactors", "0xFB10C187"), typeof(Crc));
            PropertyEntries.Add(("PerkFactors", "0xFD96D0B6"), typeof(Crc));

            PropertyEntries.Add(("Perks", "0x6EC8D450"), typeof(Crc));
            PropertyEntries.Add(("Perks", "0x613BBB29"), typeof(Crc));
            PropertyEntries.Add(("Perks", "0x23B995CA"), typeof(int));
            PropertyEntries.Add(("Perks", "0x4F1B0BC9"), typeof(Crc));
            PropertyEntries.Add(("Perks", "0x6349214F"), typeof(int));
            PropertyEntries.Add(("Perks", "0x68707986"), null);
            PropertyEntries.Add(("Perks", "0xC9709CBC"), typeof(Crc));
            PropertyEntries.Add(("Perks", "0xB24A0D49"), typeof(Crc));
            PropertyEntries.Add(("Perks", "0xB1FD379F"), null);
            PropertyEntries.Add(("Perks", "0xAD3330EF"), typeof(bool));
            PropertyEntries.Add(("Perks", "0xB0F94B6E"), typeof(Crc));
            PropertyEntries.Add(("Perks", "0x9D4695F9"), typeof(int));
            PropertyEntries.Add(("Perks", "0x8D8DE708"), typeof(Crc));
            PropertyEntries.Add(("Perks", "0x8D3C0B47"), typeof(int));
            PropertyEntries.Add(("Perks", "0x77ED2684"), typeof(int));
            PropertyEntries.Add(("Perks", "0x832ED2D8"), typeof(int));
            PropertyEntries.Add(("Perks", "0xE8DA4DD0"), typeof(Crc));
            PropertyEntries.Add(("Perks", "0xEA588FB9"), typeof(float));
            PropertyEntries.Add(("Perks", "0xFE8DD9CC"), typeof(bool));
            PropertyEntries.Add(("Perks", "0xE5943224"), typeof(int));
            PropertyEntries.Add(("Perks", "0xE1BA3193"), typeof(Crc));
            PropertyEntries.Add(("Perks", "0xDEE6B121"), typeof(Crc));
            PropertyEntries.Add(("Perks", "0xCD0B7C7A"), typeof(int));
            PropertyEntries.Add(("Perks", "0xDB67DD79"), typeof(int));

            PropertyEntries.Add(("PhysicalOrdnance", "Angular Velocity"), typeof(float));
            PropertyEntries.Add(("PhysicalOrdnance", "Activate On Contact"), typeof(bool));
            PropertyEntries.Add(("PhysicalOrdnance", "Weight"), typeof(float));
            PropertyEntries.Add(("PhysicalOrdnance", "Display Name"), typeof(Crc));
            PropertyEntries.Add(("PhysicalOrdnance", "DamageRegion"), typeof(Crc));
            PropertyEntries.Add(("PhysicalOrdnance", "DISTANCE"), typeof(float));
            PropertyEntries.Add(("PhysicalOrdnance", "0xE52E6A81"), typeof(float));
            PropertyEntries.Add(("PhysicalOrdnance", "Friction"), typeof(float));
            PropertyEntries.Add(("PhysicalOrdnance", "Lifespan"), typeof(float));
            PropertyEntries.Add(("PhysicalOrdnance", "0x4E03A742"), typeof(Vector2));
            PropertyEntries.Add(("PhysicalOrdnance", "Radius"), typeof(float));
            PropertyEntries.Add(("PhysicalOrdnance", "TakeOff Angle"), typeof(float));
            PropertyEntries.Add(("PhysicalOrdnance", "Explosion"), typeof(Crc));
            PropertyEntries.Add(("PhysicalOrdnance", "Model"), typeof(Crc));
            PropertyEntries.Add(("PhysicalOrdnance", "0x6ADE730A"), typeof(Crc));
            PropertyEntries.Add(("PhysicalOrdnance", "Bounciness"), typeof(float));
            PropertyEntries.Add(("PhysicalOrdnance", "ChargeTime"), typeof(float));

            PropertyEntries.Add(("PhysicsParticle", "Randomize Number"), typeof(float));
            PropertyEntries.Add(("PhysicsParticle", "Model"), typeof(Crc));
            PropertyEntries.Add(("PhysicsParticle", "Friction"), typeof(float));
            PropertyEntries.Add(("PhysicsParticle", "0xADAE06C3"), typeof(byte));
            PropertyEntries.Add(("PhysicsParticle", "Effect"), typeof(Crc));
            PropertyEntries.Add(("PhysicsParticle", "0x50F8BFFA"), typeof(byte));
            PropertyEntries.Add(("PhysicsParticle", "Restitution"), typeof(float));
            PropertyEntries.Add(("PhysicsParticle", "Collision Effect"), typeof(Crc));
            PropertyEntries.Add(("PhysicsParticle", "0x3904F4E4"), typeof(int));
            PropertyEntries.Add(("PhysicsParticle", "MASS"), typeof(float));
            PropertyEntries.Add(("PhysicsParticle", "AttachParticleFX"), typeof(int));

            PropertyEntries.Add(("PhysicsParticleSet", "Life Span"), typeof(Vector2));
            PropertyEntries.Add(("PhysicsParticleSet", "Randomize Number"), typeof(float));
            PropertyEntries.Add(("PhysicsParticleSet", "PhysicsParticleRef"), typeof(Crc));
            PropertyEntries.Add(("PhysicsParticleSet", "Rotation Speed"), typeof(Vector2));
            PropertyEntries.Add(("PhysicsParticleSet", "Distance Cutoff"), typeof(float));
            PropertyEntries.Add(("PhysicsParticleSet", "0x91C23C14"), typeof(Vector2));
            PropertyEntries.Add(("PhysicsParticleSet", "Num Particle"), typeof(Vector2));
            PropertyEntries.Add(("PhysicsParticleSet", "Linear Speed"), typeof(Vector2));
            PropertyEntries.Add(("PhysicsParticleSet", "Velocity Cube"), typeof(Vector3[]));
            PropertyEntries.Add(("PhysicsParticleSet", "External Force"), typeof(Vector3));
            PropertyEntries.Add(("PhysicsParticleSet", "Camera Bias"), typeof(float));
            PropertyEntries.Add(("PhysicsParticleSet", "ParticleList"), typeof(int));
            PropertyEntries.Add(("PhysicsParticleSet", "SET"), typeof(Crc));
            PropertyEntries.Add(("PhysicsParticleSet", "Air Drag"), typeof(float));

            PropertyEntries.Add(("PhysicsVehicle", "VehicleWheel"), typeof(Crc));
            PropertyEntries.Add(("PhysicsVehicle", "Vehicle Setup"), typeof(Crc));
            PropertyEntries.Add(("PhysicsVehicle", "0x0A1ED360"), typeof(float));
            PropertyEntries.Add(("PhysicsVehicle", "Wheels"), typeof(int));

            PropertyEntries.Add(("PhysicsVehicleWheel", "PristineWheelBone"), typeof(Crc));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0x0F132DD1"), typeof(byte));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0x2FC58DC9"), typeof(int));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0xA54722AC"), typeof(float));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0x8C0CB944"), typeof(float));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0x8DB64FA5"), typeof(Vector2));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0x9AE508A0"), typeof(float));
            PropertyEntries.Add(("PhysicsVehicleWheel", "RadiusBuffer"), typeof(float));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0x3B3B5B97"), typeof(Vector2));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0x85850491"), typeof(float));
            PropertyEntries.Add(("PhysicsVehicleWheel", "VehicleWheelFX"), typeof(Crc));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0xC6CD5F80"), typeof(float));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0xB8739CD5"), typeof(Crc));
            PropertyEntries.Add(("PhysicsVehicleWheel", "Steering Speed Inertia"), typeof(float));
            PropertyEntries.Add(("PhysicsVehicleWheel", "0xBF5E0AAC"), typeof(byte));
            PropertyEntries.Add(("PhysicsVehicleWheel", "WheelBone"), typeof(Crc));
            PropertyEntries.Add(("PhysicsVehicleWheel", "Steering Speed"), typeof(float));
            PropertyEntries.Add(("PhysicsVehicleWheel", "Simulation ID"), typeof(int));

            PropertyEntries.Add(("Player", "0x7E049E2A"), typeof(float));
            PropertyEntries.Add(("Player", "0x6D696EE4"), typeof(float));
            PropertyEntries.Add(("Player", "DyingSlowdownDamageMult"), typeof(float));
            PropertyEntries.Add(("Player", "0x69C018DD"), typeof(float));
            PropertyEntries.Add(("Player", "WallPush"), typeof(bool));
            PropertyEntries.Add(("Player", "0x6212644C"), typeof(float));
            PropertyEntries.Add(("Player", "0x37A1DA8A"), typeof(float));
            PropertyEntries.Add(("Player", "0x24DF19FC"), typeof(float));
            PropertyEntries.Add(("Player", "0x3016F5BF"), typeof(float));
            PropertyEntries.Add(("Player", "0x245FCBA5"), typeof(float));
            PropertyEntries.Add(("Player", "SlideForwardDist"), typeof(float));
            PropertyEntries.Add(("Player", "Health"), typeof(Crc));
            PropertyEntries.Add(("Player", "DyingIdleTime"), typeof(float));
            PropertyEntries.Add(("Player", "0xD55D3ED5"), typeof(float));
            PropertyEntries.Add(("Player", "strafe horz assist"), typeof(float));
            PropertyEntries.Add(("Player", "Default"), typeof(float));
            PropertyEntries.Add(("Player", "strafe vert assist"), typeof(float));
            PropertyEntries.Add(("Player", "CornerLook"), typeof(bool));
            PropertyEntries.Add(("Player", "MarioCam"), typeof(bool));
            PropertyEntries.Add(("Player", "min"), typeof(float));
            PropertyEntries.Add(("Player", "DyingMoveTime"), typeof(float));
            PropertyEntries.Add(("Player", "AmountToRecharge"), typeof(float));
            PropertyEntries.Add(("Player", "CamLookForwardMultiplier"), typeof(float));
            PropertyEntries.Add(("Player", "0xF2AEAB3E"), typeof(float));
            PropertyEntries.Add(("Player", "CameraSet"), typeof(Crc));
            PropertyEntries.Add(("Player", "MinAngleDiff"), typeof(float));
            PropertyEntries.Add(("Player", "DyingSlowdownTimescale"), typeof(Vector2));
            PropertyEntries.Add(("Player", "LookForwardMultiplier"), typeof(float));
            PropertyEntries.Add(("Player", "0x9DB45444"), typeof(float));
            PropertyEntries.Add(("Player", "0x7EDCA8E7"), typeof(float));
            PropertyEntries.Add(("Player", "run-walk threshold"), typeof(float));
            PropertyEntries.Add(("Player", "TimeUntilCharge"), typeof(float));
            PropertyEntries.Add(("Player", "max"), typeof(float));
            PropertyEntries.Add(("Player", "SlideAcrossDist"), typeof(float));
            PropertyEntries.Add(("Player", "0xB2C82EE0"), typeof(Vector2));
            PropertyEntries.Add(("Player", "0xB80AC9D9"), typeof(float));
            PropertyEntries.Add(("Player", "0xB283E894"), typeof(float));
            PropertyEntries.Add(("Player", "CameraSets"), typeof(int));
            PropertyEntries.Add(("Player", "DyingSlowdownDamageTimer"), typeof(float));
            PropertyEntries.Add(("Player", "DyingSlowdownHealth"), typeof(float));

            PropertyEntries.Add(("PlayerCollision", "0xC0C16868"), typeof(float));
            PropertyEntries.Add(("PlayerCollision", "0x28BBFBF2"), typeof(float));
            PropertyEntries.Add(("PlayerCollision", "0x4ABE700F"), typeof(float));
            PropertyEntries.Add(("PlayerCollision", "0xD5C76750"), typeof(bool));

            PropertyEntries.Add(("Prop", "0xC900D2CF"), typeof(bool));
            PropertyEntries.Add(("Prop", "APForce"), typeof(float));
            PropertyEntries.Add(("Prop", "0xC6300246"), typeof(float));
            PropertyEntries.Add(("Prop", "PBone"), null);
            PropertyEntries.Add(("Prop", "APPushDir"), typeof(Vector3));
            PropertyEntries.Add(("Prop", "0xAEFF60B8"), typeof(bool));
            PropertyEntries.Add(("Prop", "APActiveImmediately"), typeof(bool));
            PropertyEntries.Add(("Prop", "0xA52E4407"), typeof(bool));
            PropertyEntries.Add(("Prop", "0xA5B32FB8"), typeof(bool));
            PropertyEntries.Add(("Prop", "0x9ECE86A3"), typeof(bool));
            PropertyEntries.Add(("Prop", "0x784B7D7D"), typeof(bool));
            PropertyEntries.Add(("Prop", "0x98BC61FA"), typeof(bool));
            PropertyEntries.Add(("Prop", "APAngleDampening"), typeof(float));
            PropertyEntries.Add(("Prop", "0xD6CFED23"), typeof(int));
            PropertyEntries.Add(("Prop", "APTimeToPush"), typeof(float));
            PropertyEntries.Add(("Prop", "APIsPristine"), typeof(bool));
            PropertyEntries.Add(("Prop", "KeyFramedAtStart"), typeof(bool));
            PropertyEntries.Add(("Prop", "0xE7BA6724"), typeof(Crc));
            PropertyEntries.Add(("Prop", "APRandomNegForce"), typeof(bool));
            PropertyEntries.Add(("Prop", "0xF3EAB72D"), typeof(int));
            PropertyEntries.Add(("Prop", "0xF5572421"), typeof(bool));
            PropertyEntries.Add(("Prop", "BlueprintAttachment"), typeof(Crc));
            PropertyEntries.Add(("Prop", "0x7729C979"), typeof(bool));
            PropertyEntries.Add(("Prop", "NumToSpawnRange"), typeof(Vector2));
            PropertyEntries.Add(("Prop", "0x5AAB9E8A"), typeof(bool));
            PropertyEntries.Add(("Prop", "0x5E36BD15"), typeof(bool));
            PropertyEntries.Add(("Prop", "0x588EBC7F"), typeof(bool));
            PropertyEntries.Add(("Prop", "APAnti-Dampening"), typeof(float));
            PropertyEntries.Add(("Prop", "Effect"), typeof(Crc));
            PropertyEntries.Add(("Prop", "0x6ADE730A"), typeof(Crc));
            PropertyEntries.Add(("Prop", "0x62F3FCEB"), typeof(float));
            PropertyEntries.Add(("Prop", "APLinearDampening"), typeof(float));
            PropertyEntries.Add(("Prop", "0x725C3533"), typeof(int));
            PropertyEntries.Add(("Prop", "APTimeToStop"), typeof(float));
            PropertyEntries.Add(("Prop", "Blueprint"), typeof(Crc));
            PropertyEntries.Add(("Prop", "SabotageType"), typeof(Crc));
            PropertyEntries.Add(("Prop", "0x3E495387"), typeof(bool));
            PropertyEntries.Add(("Prop", "APTimeToStart"), typeof(float));
            PropertyEntries.Add(("Prop", "0x302B2084"), typeof(bool));
            PropertyEntries.Add(("Prop", "RotateDataList"), typeof(int));
            PropertyEntries.Add(("Prop", "0x34225FE1"), typeof(Crc));
            PropertyEntries.Add(("Prop", "Bones"), null);
            PropertyEntries.Add(("Prop", "Melee Target"), typeof(bool));
            PropertyEntries.Add(("Prop", "PercentageToSpawn"), typeof(float));
            PropertyEntries.Add(("Prop", "0x2CE48D05"), typeof(bool));
            PropertyEntries.Add(("Prop", "DamageEffect"), typeof(bool));
            PropertyEntries.Add(("Prop", "0x2947CB2D"), typeof(Crc));
            PropertyEntries.Add(("Prop", "APTargetRotations"), typeof(Vector3));
            PropertyEntries.Add(("Prop", "APPushDirRotates"), typeof(bool));
            PropertyEntries.Add(("Prop", "RotTargetBone"), typeof(Crc));
            PropertyEntries.Add(("Prop", "0x2255030D"), typeof(bool));
            PropertyEntries.Add(("Prop", "Attachments"), typeof(int));
            PropertyEntries.Add(("Prop", "PushTargetBone"), typeof(Crc));
            PropertyEntries.Add(("Prop", "0x21550886"), typeof(bool));
            PropertyEntries.Add(("Prop", "AnimatedPropRotateData"), typeof(Crc));
            PropertyEntries.Add(("Prop", "PushDataList"), typeof(int));
            PropertyEntries.Add(("Prop", "APDeactivateImmediately"), typeof(bool));
            PropertyEntries.Add(("Prop", "0x1A62C9AE"), typeof(bool));
            PropertyEntries.Add(("Prop", "Debris"), typeof(Crc));
            PropertyEntries.Add(("Prop", "ShowFarScene"), typeof(bool));
            PropertyEntries.Add(("Prop", "APTimeRange"), typeof(Vector2));
            PropertyEntries.Add(("Prop", "APOffset"), typeof(Vector3));
            PropertyEntries.Add(("Prop", "0x0B9BEF11"), typeof(bool));
            PropertyEntries.Add(("Prop", "0x099A56FA"), typeof(float));
            PropertyEntries.Add(("Prop", "AnimatedPropPushData"), typeof(Crc));
            PropertyEntries.Add(("Prop", "APSound"), typeof(Crc));
            PropertyEntries.Add(("Prop", "InitialEffect"), typeof(bool));

            PropertyEntries.Add(("RandomObj", "BlueprintList"), typeof(int));
            PropertyEntries.Add(("RandomObj", "LabelFilter"), typeof(char[]));
            PropertyEntries.Add(("RandomObj", "LocationBased"), typeof(bool));
            PropertyEntries.Add(("RandomObj", "Weight"), typeof(float));
            PropertyEntries.Add(("RandomObj", "RandomBlueprintNode"), typeof(Crc));
            PropertyEntries.Add(("RandomObj", "Amount"), typeof(int));
            PropertyEntries.Add(("RandomObj", "BlueprintName"), typeof(Crc));
            PropertyEntries.Add(("RandomObj", "0x84F14707"), typeof(Crc));

            PropertyEntries.Add(("ScriptController", "InitialModule"), typeof(char[]));
            PropertyEntries.Add(("ScriptController", "0xCA6B9057"), typeof(int));
            PropertyEntries.Add(("ScriptController", "0xC072E94A"), typeof(float));
            PropertyEntries.Add(("ScriptController", "RainStartDist"), typeof(float));
            PropertyEntries.Add(("ScriptController", "ExecutionSpeakerName"), typeof(int));
            PropertyEntries.Add(("ScriptController", "Path"), typeof(char[]));
            PropertyEntries.Add(("ScriptController", "0xBE5952B1"), typeof(float));
            PropertyEntries.Add(("ScriptController", "LightningStopDist"), typeof(float));
            PropertyEntries.Add(("ScriptController", "0xA3DBF09D"), typeof(bool));
            PropertyEntries.Add(("ScriptController", "0xA77EDAAA"), typeof(bool));
            PropertyEntries.Add(("ScriptController", "0x97EE12C6"), typeof(bool));
            PropertyEntries.Add(("ScriptController", "LightningStartDist"), typeof(float));
            PropertyEntries.Add(("ScriptController", "Spawner"), typeof(char[]));
            PropertyEntries.Add(("ScriptController", "ExecutionType"), typeof(int));
            PropertyEntries.Add(("ScriptController", "RainDelta"), typeof(float));
            PropertyEntries.Add(("ScriptController", "0xE4E34549"), typeof(int));
            PropertyEntries.Add(("ScriptController", "ANIM"), typeof(int));
            PropertyEntries.Add(("ScriptController", "0xFE8BAE82"), typeof(int));
            PropertyEntries.Add(("ScriptController", "0xDDF1EE0F"), typeof(float));
            PropertyEntries.Add(("ScriptController", "0xD72AA401"), typeof(int));
            PropertyEntries.Add(("ScriptController", "0xCACFD6AA"), typeof(int));
            PropertyEntries.Add(("ScriptController", "0xCE629E7E"), typeof(int));
            PropertyEntries.Add(("ScriptController", "ActivationRadius"), typeof(float));
            PropertyEntries.Add(("ScriptController", "InvincibleRange"), typeof(float));
            PropertyEntries.Add(("ScriptController", "FreeplayType"), typeof(int));
            PropertyEntries.Add(("ScriptController", "0x77776D5A"), typeof(int));
            PropertyEntries.Add(("ScriptController", "RainStopDist"), typeof(float));
            PropertyEntries.Add(("ScriptController", "0x722C0953"), typeof(int));
            PropertyEntries.Add(("ScriptController", "0x6A9AB174"), typeof(float));
            PropertyEntries.Add(("ScriptController", "0x4177A478"), typeof(int));
            PropertyEntries.Add(("ScriptController", "Is3D"), typeof(bool));
            PropertyEntries.Add(("ScriptController", "0x37FC008D"), typeof(float));
            PropertyEntries.Add(("ScriptController", "0x2F1820CC"), typeof(float));
            PropertyEntries.Add(("ScriptController", "ExecutionerName"), typeof(int));
            PropertyEntries.Add(("ScriptController", "ActivationChance"), typeof(float));
            PropertyEntries.Add(("ScriptController", "0x0424DD32"), typeof(bool));
            PropertyEntries.Add(("ScriptController", "0x07B13063"), typeof(int));
            PropertyEntries.Add(("ScriptController", "0x165C1FD7"), typeof(int));

            PropertyEntries.Add(("SearcherSeat", "0x585F7DB9"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "0x27B2EBEE"), typeof(bool));
            PropertyEntries.Add(("SearcherSeat", "0x341FFD9C"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "0xB4E28AAF"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "0xA419D2AA"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "0xB0C2D055"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "0x94207504"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "0x5D348D7A"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "0x804D355B"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "SeatMotorStrengthFactor"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "0xDFD6597A"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "0xC8823FF8"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "SearcherMotorStrengthFactor"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "DefaultSearcher"), typeof(int));
            PropertyEntries.Add(("SearcherSeat", "0xFB9E20AE"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "PathDirection"), typeof(Crc));
            PropertyEntries.Add(("SearcherSeat", "PathDuration"), typeof(float));
            PropertyEntries.Add(("SearcherSeat", "LightDistControl"), typeof(bool));
            PropertyEntries.Add(("SearcherSeat", "SearcherName"), typeof(Crc));
            PropertyEntries.Add(("SearcherSeat", "PathReference"), typeof(Crc));
            PropertyEntries.Add(("SearcherSeat", "PathMovementType"), typeof(Crc));
            PropertyEntries.Add(("SearcherSeat", "PathName"), typeof(Crc));

            PropertyEntries.Add(("Seat", "0x6C9E76F1"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0x3BF1CDB6"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0x2CAA3B38"), typeof(float));
            PropertyEntries.Add(("Seat", "0x330537AE"), typeof(byte));
            PropertyEntries.Add(("Seat", "0x3B645216"), typeof(byte));
            PropertyEntries.Add(("Seat", "0x137F0C86"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0x02F5B417"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0x0B3C9AEB"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0x11EEDAB0"), typeof(int));
            PropertyEntries.Add(("Seat", "0x560A68C4"), typeof(Crc));
            PropertyEntries.Add(("Seat", "FastCamera"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0x516AA372"), typeof(int));
            PropertyEntries.Add(("Seat", "0x43E5A73B"), typeof(int));
            PropertyEntries.Add(("Seat", "Camera"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0x4C2D60BB"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0xB8EE1BA8"), typeof(int));
            PropertyEntries.Add(("Seat", "0xB2EF75E8"), typeof(int));
            PropertyEntries.Add(("Seat", "0xB838A7E9"), typeof(Vector2));
            PropertyEntries.Add(("Seat", "0xB2E3212D"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0xAC256817"), typeof(byte));
            PropertyEntries.Add(("Seat", "0xAC2CF95E"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0xB1113237"), typeof(float));
            PropertyEntries.Add(("Seat", "0xA7642F51"), typeof(float));
            PropertyEntries.Add(("Seat", "0xA2ECF38A"), typeof(float));
            PropertyEntries.Add(("Seat", "0x9EB6A878"), typeof(float));
            PropertyEntries.Add(("Seat", "0x9D66FD52"), typeof(byte));
            PropertyEntries.Add(("Seat", "0x99360455"), typeof(int));
            PropertyEntries.Add(("Seat", "0x7FA0EFE1"), typeof(float));
            PropertyEntries.Add(("Seat", "0x8B109314"), typeof(Crc));
            PropertyEntries.Add(("Seat", "VehicleViewTransition"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0xD73C58BC"), typeof(int));
            PropertyEntries.Add(("Seat", "0xD892AD22"), typeof(byte));
            PropertyEntries.Add(("Seat", "0xCE4384BF"), typeof(Vector2));
            PropertyEntries.Add(("Seat", "SeatName"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0xC216DE38"), typeof(float));
            PropertyEntries.Add(("Seat", "0xC615E301"), typeof(int));
            PropertyEntries.Add(("Seat", "SeatBone"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0xE441FD9A"), typeof(Vector2));
            PropertyEntries.Add(("Seat", "0xEB4F93E7"), typeof(int));
            PropertyEntries.Add(("Seat", "DefaultSeatSpawnObject"), typeof(Crc));
            PropertyEntries.Add(("Seat", "0xF23C0127"), typeof(Crc));
            PropertyEntries.Add(("Seat", "IdleAnimation"), typeof(Crc));
            PropertyEntries.Add(("Seat", "SeatSpawnObject"), typeof(Crc));
            PropertyEntries.Add(("Seat", "SeatSpawnObjectOverride"), typeof(Crc));
            PropertyEntries.Add(("Seat", "SeatNameRef"), typeof(Crc));
            PropertyEntries.Add(("Seat", "DriverSettings"), typeof(Crc));
            PropertyEntries.Add(("Seat", "PassengerSettings"), typeof(Crc));
            PropertyEntries.Add(("Seat", "GunnerSettings"), typeof(Crc));
            PropertyEntries.Add(("Seat", "SearcherSettings"), typeof(Crc));
            PropertyEntries.Add(("Seat", "CommonSeatSettings"), typeof(Crc));

            PropertyEntries.Add(("SeatWithMount", "AimerSettings"), typeof(Crc));

            PropertyEntries.Add(("Targetable", "Width"), typeof(float));
            PropertyEntries.Add(("Targetable", "Height"), typeof(float));

            PropertyEntries.Add(("ToneMapping", "Adapatation Time"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Max White"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "LerpValue"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "StartHour"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Gamma"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Max Key"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Luminance Bias"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Key Value"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Black Threshold"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Min Lum"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Max Lum"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Weight Crisp"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Weight Original"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Black Bias"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Adapatation Ratio"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "StartMinute"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Min Key"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Weight Blurred"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "Threshold"), typeof(float));
            PropertyEntries.Add(("ToneMapping", "BloomLerp"), typeof(float));

            PropertyEntries.Add(("Trappable", "0xE77DB9D6"), typeof(float));

            PropertyEntries.Add(("TweakableColors", "Index3Variations"), typeof(int));
            PropertyEntries.Add(("TweakableColors", "ColorRandWeight"), typeof(float));
            PropertyEntries.Add(("TweakableColors", "Color"), typeof(Color));
            PropertyEntries.Add(("TweakableColors", "Index2Variations"), typeof(int));
            PropertyEntries.Add(("TweakableColors", "Index1Variations"), typeof(int));
            PropertyEntries.Add(("TweakableColors", "ColorVariation"), typeof(Crc));

            PropertyEntries.Add(("Vehicle", "0x7514BE0C"), typeof(int));
            PropertyEntries.Add(("Vehicle", "0x4E69EFB4"), typeof(float));
            PropertyEntries.Add(("Vehicle", "AITurnAngleToBrake"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0x4BFC24F8"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0x3E33738B"), typeof(int));
            PropertyEntries.Add(("Vehicle", "SabotageType"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "LoRes Sound Bank"), typeof(char[]));
            PropertyEntries.Add(("Vehicle", "FireHealthThreshold"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0x372ECA29"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0x389D5EAE"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x372A4CFB"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0x36323226"), typeof(int));
            PropertyEntries.Add(("Vehicle", "Passenger Seat Settings"), typeof(int));
            PropertyEntries.Add(("Vehicle", "DeathCountdownMax"), typeof(float));
            PropertyEntries.Add(("Vehicle", "BrakelightBoneDipAngle"), typeof(float));
            PropertyEntries.Add(("Vehicle", "AIMinSteerLookahead"), typeof(float));
            PropertyEntries.Add(("Vehicle", "SmokeHealthThreshold"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0x6435EDF4"), typeof(int));
            PropertyEntries.Add(("Vehicle", "0x5D9068A3"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "TaillightHalo"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x4E6BA751"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x5167D9D0"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "SearcherSeatSpec"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "BrakelightLight"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "HeadlightBoneDipAngle"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0x2867754B"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0x1A14BDDA"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x0BCDA30E"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "HeadlightBone"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "DynamicPriority"), typeof(uint));
            PropertyEntries.Add(("Vehicle", "DeathCountdownMin"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0x108CE590"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "HeadlightLight"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x23B7A240"), typeof(int));
            PropertyEntries.Add(("Vehicle", "TaillightLight"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "BrakelightBone"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "VehicleTaillights"), typeof(int));
            PropertyEntries.Add(("Vehicle", "0x1D2C62A4"), typeof(float));
            PropertyEntries.Add(("Vehicle", "BrakelightHalo"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "ReverselightBoneDipAngle"), typeof(float));
            PropertyEntries.Add(("Vehicle", "HeadlightHalo"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x29AB6EDC"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "VehicleExhaustEffects"), typeof(int));
            PropertyEntries.Add(("Vehicle", "Nazi"), typeof(byte));
            PropertyEntries.Add(("Vehicle", "VehicleEngineEffects"), typeof(int));
            PropertyEntries.Add(("Vehicle", "Searcher Seat Settings"), typeof(int));
            PropertyEntries.Add(("Vehicle", "TaillightBone"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "SeatSpec"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x3003EC45"), typeof(Vector2));
            PropertyEntries.Add(("Vehicle", "HiRes Sound Bank"), typeof(char[]));
            PropertyEntries.Add(("Vehicle", "0xB6041786"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0xB85E6EF1"), typeof(float));
            PropertyEntries.Add(("Vehicle", "ParkedWeakness"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0xAFEC20D2"), typeof(float));
            PropertyEntries.Add(("Vehicle", "VehicleReverselights"), typeof(int));
            PropertyEntries.Add(("Vehicle", "DriverSeatSpec"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0xA8413DF8"), typeof(int));
            PropertyEntries.Add(("Vehicle", "0xAE911955"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0xA7DCD5CC"), typeof(int));
            PropertyEntries.Add(("Vehicle", "0xA69A8211"), typeof(byte));
            PropertyEntries.Add(("Vehicle", "AITurnFactor"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0xA46B0807"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "Display Name"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x8BC7C405"), typeof(int));
            PropertyEntries.Add(("Vehicle", "0x8D701079"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x8A888A08"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x7A13EB63"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "AIMaxSteerLookahead"), typeof(float));
            PropertyEntries.Add(("Vehicle", "Gunner Seat Settings"), typeof(int));
            PropertyEntries.Add(("Vehicle", "TaillightBoneDipAngle"), typeof(float));
            PropertyEntries.Add(("Vehicle", "VehicleTaillight"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x999A233F"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "GunnerSeatSpec"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "VehicleHeadlight"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0xF7F2C6BC"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "ReverselightBone"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0xF1EE5038"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0xEEF41706"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0xF063A2FC"), typeof(int));
            PropertyEntries.Add(("Vehicle", "0xEDBA8801"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0xE34A94FB"), typeof(int));
            PropertyEntries.Add(("Vehicle", "0xE0F0480C"), typeof(byte));
            PropertyEntries.Add(("Vehicle", "BigExplosionDelay"), typeof(Vector2));
            PropertyEntries.Add(("Vehicle", "VehicleReverselight"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "VehicleBrakelights"), typeof(int));
            PropertyEntries.Add(("Vehicle", "CamSpeedMod"), typeof(float));
            PropertyEntries.Add(("Vehicle", "ReverselightHalo"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "Civilian"), typeof(bool));
            PropertyEntries.Add(("Vehicle", "0xD48F11EE"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "ReverselightLight"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "VehicleHeadlights"), typeof(int));
            PropertyEntries.Add(("Vehicle", "0xCA612893"), typeof(int));
            PropertyEntries.Add(("Vehicle", "0xC6649C6E"), typeof(float));
            PropertyEntries.Add(("Vehicle", "0xC40E776E"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0xC6300246"), typeof(float));
            PropertyEntries.Add(("Vehicle", "BigExplBPrint"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "VehicleBrakelight"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "VehiclePhysics"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "AIVehicleSettings"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "VehicleEngineSound"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "VehicleEffects"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0x0A1AF7EF"), typeof(Crc));
            PropertyEntries.Add(("Vehicle", "0xC795D87F"), typeof(float));
            PropertyEntries.Add(("Vehicle", "Max Speed"), typeof(float)); // unused?

            PropertyEntries.Add(("VirVehicleEngine", "MaxRPM"), typeof(float));
            PropertyEntries.Add(("VirVehicleEngine", "MaxRPMResistance"), typeof(float));
            PropertyEntries.Add(("VirVehicleEngine", "MaxRPMTorque"), typeof(float));
            PropertyEntries.Add(("VirVehicleEngine", "OptRPM"), typeof(float));
            PropertyEntries.Add(("VirVehicleEngine", "MinRPMResistance"), typeof(float));
            PropertyEntries.Add(("VirVehicleEngine", "MinRPMTorque"), typeof(float));
            PropertyEntries.Add(("VirVehicleEngine", "OptRPMResistance"), typeof(float));
            PropertyEntries.Add(("VirVehicleEngine", "HorsePower"), typeof(Vector3));
            PropertyEntries.Add(("VirVehicleEngine", "MinRPM"), typeof(float));
            PropertyEntries.Add(("VirVehicleEngine", "ClutchSlipRPM"), typeof(float));
            PropertyEntries.Add(("VirVehicleEngine", "0xB2A09C8C"), typeof(float));
            PropertyEntries.Add(("VirVehicleEngine", "0xDEC209DB"), typeof(float));

            PropertyEntries.Add(("VirVehicleSetup", "Vehicle Chassis"), typeof(Crc));
            PropertyEntries.Add(("VirVehicleSetup", "Vehicle Engine"), typeof(Crc));
            PropertyEntries.Add(("VirVehicleSetup", "Vehicle Transmission"), typeof(Crc));

            PropertyEntries.Add(("WaterController", "0x95FF5441"), typeof(Crc));
            PropertyEntries.Add(("WaterController", "0x3B2E2AA3"), typeof(float));
            PropertyEntries.Add(("WaterController", "0x33175C7A"), typeof(float));
            PropertyEntries.Add(("WaterController", "0x35AFA241"), typeof(float));
            PropertyEntries.Add(("WaterController", "0x25D7DDC9"), typeof(float));
            PropertyEntries.Add(("WaterController", "Specular Color"), typeof(Color));
            PropertyEntries.Add(("WaterController", "0x1B9B037D"), typeof(float));
            PropertyEntries.Add(("WaterController", "0xA491DFCA"), typeof(Color));
            PropertyEntries.Add(("WaterController", "0x7F39F1BB"), typeof(Color));
            PropertyEntries.Add(("WaterController", "0xA2300E26"), typeof(Color));
            PropertyEntries.Add(("WaterController", "0xB9969F89"), typeof(float));
            PropertyEntries.Add(("WaterController", "Specular Intensity"), typeof(float));

            PropertyEntries.Add(("WaterFlow", "Force"), typeof(float));

            PropertyEntries.Add(("Weapon", "0x7C0B86B9"), typeof(bool));
            PropertyEntries.Add(("Weapon", "0x334A7577"), typeof(int));
            PropertyEntries.Add(("Weapon", "AI Max Miss Angle"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x0F3D263B"), typeof(float));
            PropertyEntries.Add(("Weapon", "still spread"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x1508D7DD"), typeof(Vector4));
            PropertyEntries.Add(("Weapon", "0x0B3830D6"), typeof(float));
            PropertyEntries.Add(("Weapon", "shots in a clip"), typeof(int));
            PropertyEntries.Add(("Weapon", "0x079C2D5B"), typeof(float));
            PropertyEntries.Add(("Weapon", "Normal CameraSetting"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "AI close adjust time"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI burst cooldown"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI Max DPS Multiplier"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x0140B5C6"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x00F23035"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x0008DD0E"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x001223FE"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x04AD1998"), typeof(bool));
            PropertyEntries.Add(("Weapon", "0x0266D69F"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI Grenades Disabled"), typeof(bool));
            PropertyEntries.Add(("Weapon", "0x04E3420D"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "0x2FD5EE93"), typeof(float));
            PropertyEntries.Add(("Weapon", "SoundVariations"), typeof(int));
            PropertyEntries.Add(("Weapon", "ArtillerySightedCrosshairs"), typeof(bool));
            PropertyEntries.Add(("Weapon", "wind down time"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x2F5D8019"), typeof(Vector2));
            PropertyEntries.Add(("Weapon", "0x2E022594"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x2CAA4ACE"), typeof(float));
            PropertyEntries.Add(("Weapon", "aim anim"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "0x2A22FDC1"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x26AE7598"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x2652FAE8"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI med adjust time"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x1724437A"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x1B4B7AF6"), typeof(float));
            PropertyEntries.Add(("Weapon", "Ordnance"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "hide after fire"), typeof(bool));
            PropertyEntries.Add(("Weapon", "AnimBank"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "0x64F0F584"), typeof(int));
            PropertyEntries.Add(("Weapon", "bash force"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x63468C4B"), typeof(float));
            PropertyEntries.Add(("Weapon", "remove when empty"), typeof(bool));
            PropertyEntries.Add(("Weapon", "0x56D6092E"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x560A68C4"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "spread heat / shot"), typeof(float));
            PropertyEntries.Add(("Weapon", "sighted spread mult"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x52619D52"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI close inaccuracy"), typeof(Vector2));
            PropertyEntries.Add(("Weapon", "0x3BF1CDB6"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "0x379CD846"), typeof(float));
            PropertyEntries.Add(("Weapon", "horz aim priority"), typeof(float));
            PropertyEntries.Add(("Weapon", "vert aim priority"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x36F01F83"), typeof(float));
            PropertyEntries.Add(("Weapon", "audible radius"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x44297E86"), typeof(byte));
            PropertyEntries.Add(("Weapon", "0x3C967A20"), typeof(int));
            PropertyEntries.Add(("Weapon", "LoRes Sound Bank"), typeof(char[]));
            PropertyEntries.Add(("Weapon", "0x797A84EF"), typeof(float));
            PropertyEntries.Add(("Weapon", "rate of fire"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x7B7A641C"), typeof(float));
            PropertyEntries.Add(("Weapon", "aim box width"), typeof(float));
            PropertyEntries.Add(("Weapon", "aim action anim"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "0x74BEF49A"), typeof(int));
            PropertyEntries.Add(("Weapon", "wind up time"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x7777D77E"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "target seek speed"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x6DB7B63D"), typeof(int));
            PropertyEntries.Add(("Weapon", "Crosshairs"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "0x71E30E69"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x6CA0E2FE"), typeof(Vector4));
            PropertyEntries.Add(("Weapon", "vertical recoil"), typeof(Vector2));
            PropertyEntries.Add(("Weapon", "0x69059D08"), typeof(bool));
            PropertyEntries.Add(("Weapon", "0x69DCA2B6"), typeof(int));
            PropertyEntries.Add(("Weapon", "0x6B484076"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "HiRes Sound Bank"), typeof(char[]));
            PropertyEntries.Add(("Weapon", "AI Cover Disabled"), typeof(bool));
            PropertyEntries.Add(("Weapon", "0xA3868095"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xA3C83F69"), typeof(bool));
            PropertyEntries.Add(("Weapon", "Crosshair Scale"), typeof(float));
            PropertyEntries.Add(("Weapon", "ArtilleryCrosshairs"), typeof(bool));
            PropertyEntries.Add(("Weapon", "aim box height"), typeof(float));
            PropertyEntries.Add(("Weapon", "run spread"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x9737DC5B"), typeof(Vector2));
            PropertyEntries.Add(("Weapon", "Automatic"), typeof(bool));
            PropertyEntries.Add(("Weapon", "reload time"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x95F2D6BB"), typeof(bool));
            PropertyEntries.Add(("Weapon", "aim run anim"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "0x8FABBD6C"), typeof(float));
            PropertyEntries.Add(("Weapon", "CameraFireMovementScale"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x8EB3B1A4"), typeof(float));
            PropertyEntries.Add(("Weapon", "min range"), typeof(float));
            PropertyEntries.Add(("Weapon", "0x865EADCB"), typeof(float));
            PropertyEntries.Add(("Weapon", "reload anim"), typeof(int));
            PropertyEntries.Add(("Weapon", "optimal range"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI med range max"), typeof(float));
            PropertyEntries.Add(("Weapon", "dist aim priority"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xAE28D6E9"), typeof(float));
            PropertyEntries.Add(("Weapon", "Maximum Bloom Spread"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xADDEBDCA"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "starting clips"), typeof(int));
            PropertyEntries.Add(("Weapon", "AI close range max"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xA75D5327"), typeof(int));
            PropertyEntries.Add(("Weapon", "AI Presence"), typeof(float));
            PropertyEntries.Add(("Weapon", "horizontal recoil"), typeof(Vector2));
            PropertyEntries.Add(("Weapon", "0xB838A7E9"), typeof(Vector2));
            PropertyEntries.Add(("Weapon", "0xB4D334D8"), typeof(bool));
            PropertyEntries.Add(("Weapon", "OrdnanceBone"), typeof(int));
            PropertyEntries.Add(("Weapon", "max range"), typeof(float));
            PropertyEntries.Add(("Weapon", "Finishing Move Type"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "AI far adjust time"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xB91AF09E"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "special anim type"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "MuzzleFlashTime"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xEBB3F257"), typeof(float));
            PropertyEntries.Add(("Weapon", "MuzzleFlashLightIndex"), typeof(int));
            PropertyEntries.Add(("Weapon", "0xE545FBF4"), typeof(bool));
            PropertyEntries.Add(("Weapon", "0xE79E08FE"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI med inaccuracy"), typeof(Vector2));
            PropertyEntries.Add(("Weapon", "0xE1F8DAFC"), typeof(bool));
            PropertyEntries.Add(("Weapon", "0xE0AC4345"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xDDF78D47"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xE0428E0D"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xFA9AD969"), typeof(float));
            PropertyEntries.Add(("Weapon", "spread cool / sec"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI Base Accuracy"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI burst duration"), typeof(float));
            PropertyEntries.Add(("Weapon", "FlashIcon"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "0xEFED376D"), typeof(float));
            PropertyEntries.Add(("Weapon", "CameraSetting"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "Sighted crosshairs"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "walk spread"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI far range max"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xD850F531"), typeof(int));
            PropertyEntries.Add(("Weapon", "0xDB4B8095"), typeof(float));
            PropertyEntries.Add(("Weapon", "0xD3F2828B"), typeof(bool));
            PropertyEntries.Add(("Weapon", "0xCE9447BC"), typeof(float));
            PropertyEntries.Add(("Weapon", "fire anim"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "Buddy Item"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "Melee Type"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "0xCE4384BF"), typeof(Vector2));
            PropertyEntries.Add(("Weapon", "0xCA55EEC9"), typeof(byte));
            PropertyEntries.Add(("Weapon", "0xC882AF1A"), typeof(float));
            PropertyEntries.Add(("Weapon", "pellets per shot"), typeof(int));
            PropertyEntries.Add(("Weapon", "0xCAF505EA"), typeof(Crc));
            PropertyEntries.Add(("Weapon", "0xC6300246"), typeof(float));
            PropertyEntries.Add(("Weapon", "shot spread range"), typeof(Vector2));
            PropertyEntries.Add(("Weapon", "0xBD8F9C00"), typeof(float));
            PropertyEntries.Add(("Weapon", "AI far inaccuracy"), typeof(Vector2));
            PropertyEntries.Add(("Weapon", "0xC40ED65B"), typeof(int));

            PropertyEntries.Add(("0xD0C6D015", "0xE813FE45"), typeof(int));
            PropertyEntries.Add(("0xD0C6D015", "Rotation"), typeof(Vector3));
            PropertyEntries.Add(("0xD0C6D015", "Offset"), typeof(Vector3));
            PropertyEntries.Add(("0xD0C6D015", "LightVolume"), typeof(Crc));
            PropertyEntries.Add(("0xD0C6D015", "0xF2CCAF79"), typeof(Crc));
            PropertyEntries.Add(("0xD0C6D015", "0xFD4C5C69"), typeof(Crc));
        }

        public static Type GetTypeOfProperty(string group, string name)
        {
            return PropertyEntries.TryGetValue((group, name), out Type type) ? type : null;
        }

        private static string FormatEmptyType(byte[] bytes)
        {
            var bytesStr = BitConverter.ToString(bytes).Replace('-', ' ');

            string guessStr = "";
            if (bytes.Length >= 4)
            {
                var intStr = bytes.Length >= 4 ? BitConverter.ToInt32(bytes, 0).ToString() : "";
                var floatStr = bytes.Length >= 4 ? BitConverter.ToSingle(bytes, 0).ToString("0.00") : "";
                var stringVal = Encoding.UTF8.GetString(bytes);
                var crcVal = bytes.Length >= 4 ? new Crc(BitConverter.ToUInt32(bytes, 0)).ToString() : "";

                for (var i = 0; i < bytes.Length; ++i)
                {
                    // Don't check nulltermination
                    if (i == bytes.Length - 1 && bytes[i] == 0)
                        break;

                    // Check every character to be valid
                    if (bytes[i] < 0x20 || bytes[i] > 0x7E)
                    {
                        stringVal = null;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(stringVal))
                {
                    guessStr = $" (I: {intStr,-15} | F: {floatStr,-15} | Crc: {crcVal})";
                }
                else
                {
                    if (stringVal[^1] == '\0')
                        stringVal = stringVal[0..^1];

                    guessStr = $" (I: {intStr,-15} | F: {floatStr,-15} | S: {stringVal,-15} | Crc: {crcVal})";
                }
            }

            return $"{bytesStr,-30}{guessStr}";
        }

        public static object ReadPropertyValue(BinaryReader reader, Type type, int size)
        {
            if (type == null)
                return FormatEmptyType(reader.ReadBytes(size));

            switch (type.Name)
            {
                case "Int32":
                    return reader.ReadInt32();

                case "UInt32":
                    return reader.ReadUInt32();

                case "Boolean":
                    return reader.ReadBoolean();

                case "Byte":
                    return reader.ReadByte();

                case "Char[]":
                    return reader.ReadStringWithMaxLength(size);

                case "String":
                    return reader.ReadStringWithMaxLength(reader.ReadInt32());

                case "Crc":
                    return new Crc(reader.ReadUInt32());

                case "LuaParam":
                    return new LuaParam(reader);

                case "Single":
                    return reader.ReadSingle();

                case "Double":
                    return reader.ReadDouble();

                case "Byte[]":
                    return BitConverter.ToString(reader.ReadBytes(size)).Replace('-', ' ');

                case "Vector2":
                    return new Vector2(reader.ReadSingle(), reader.ReadSingle());

                case "Vector3":
                    return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                case "Vector4":
                    return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                case "Vector3[]":
                    if (size % 12 != 0)
                        throw new Exception("Invalid size for Vector3 array!");

                    var vectors = new Vector3[size / 12];
                    for (var i = 0; i < vectors.Length; ++i)
                        vectors[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                    return vectors;

                case "Color":
                    var red = reader.ReadByte();
                    var blue = reader.ReadByte();
                    var green = reader.ReadByte();
                    var alpha = reader.ReadByte();

                    return Color.FromArgb(alpha, red, blue, green);

                default:
                    Console.WriteLine($"Unknown type found in Blueprint::ReadPropertyValue! Type: {type.Name}");
                    return null;
            }
        }

        public static string FormatCrc(uint crc)
        {
            return $"0x{crc:X8}";
        }
    }
}
