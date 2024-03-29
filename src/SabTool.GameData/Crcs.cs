﻿namespace SabTool.GameData;

public static class Crcs
{
    // Common
    public static readonly Crc ZERO = new Crc(0x0);
    public static readonly Crc NONE = new(0x4FF9F863);
    public static readonly Crc DisplayName = new(0x97AC8632);
    public static readonly Crc ListElement = new(0x4D3D1B7D);

    #region Blueprints
    #region Fields
    public static readonly Crc Dynamic = new(0x6302F1CC);
    public static readonly Crc UseDynamicPool = new(0x8C9928FB);
    public static readonly Crc Priority = new(0x87519019);
    public static readonly Crc Backup = new(0xBF83D3AF);
    public static readonly Crc Managed = new(0x0C2DB3B4);

    public static readonly Crc Version = new(0xA1F61387);
    public static readonly Crc FlashFile = new(0x9CAA0ADF);

    public static readonly Crc LightHaloRotationSpeed = new(0x54F0B858);
    public static readonly Crc LightHaloRotationBone = new(0xB4A80855);
    public static readonly Crc LightHaloBone = new(0xF80ABFB7);
    public static readonly Crc LightHalo = new(0x438C44F5);
    public static readonly Crc LightHaloAttachment = new(0x0B213D8E);
    public static readonly Crc LightHaloRotationAngleLimit = new(0x1C128785);
    public static readonly Crc LightHaloAttachments = new(0x2A8F3B51);

    public static readonly Crc LightAttachments = new(0xA873293A);
    public static readonly Crc LightAttachment = new(0xA3613F4D);
    public static readonly Crc LightRotationSpeed = new(0x8ED97E12);
    public static readonly Crc LightRotationBone = new(0x6C9D982B);
    public static readonly Crc LightBone = new(0x9AB4F3ED);
    public static readonly Crc Light = new(0xC25641DF);
    public static readonly Crc LightRotationAngleLimit = new(0x81EB0E4F);

    public static readonly Crc Model = new(0x5B724250);
    public static readonly Crc SlotType = new(0x1E757409);
    public static readonly Crc Equippable = new(0x75060ED9);
    public static readonly Crc HiddenWhileEquiped = new(0xF7CEEE0B);
    public static readonly Crc Unk0x6ADE730A = new(0x6ADE730A);
    public static readonly Crc PickupHighlight = new(0x0D111AB7);
    public static readonly Crc PickupLowAnim = new(0x0F59CD06);
    public static readonly Crc Unk0x953C60F5 = new(0x953C60F5);
    public static readonly Crc PickupHighAnim = new(0x2D1BCD5A);
    public static readonly Crc NoWillToFight = new(0xAE1ED17F);
    public static readonly Crc SpecialID = new(0xC6676245);
    public static readonly Crc IsUniqueItem = new(0x650A47ED);
    public static readonly Crc IsDetonatorItem = new(0x70785B2A);
    public static readonly Crc CanDrop = new(0x13F47172);
    public static readonly Crc Label = new(0x06DA8775);
    public static readonly Crc SubItem1 = new(0xF482A959);
    public static readonly Crc SubItem1Amount = new(0x27DC224F);
    public static readonly Crc SubItem2 = new(0x927B534E);
    public static readonly Crc SubItem2Amount = new(0xCE2AB312);
    public static readonly Crc SubItem3 = new(0x747D62AB);
    public static readonly Crc SubItem3Amount = new(0x4EB751CD);
    public static readonly Crc SubItem4 = new(0x728A2B78);
    public static readonly Crc SubItem4Amount = new(0xE47D7D10);
    public static readonly Crc Unk0x7EDFDECA = new(0x7EDFDECA);

    public static readonly Crc Weakness = new(0xDD7EAE2C);
    public static readonly Crc InitialHealth = new(0x8CBC442D);
    public static readonly Crc Unk0x7C0B86B9 = new(0x7C0B86B9);
    public static readonly Crc Unk0x334A7577 = new(0x334A7577);
    public static readonly Crc AIMaxMissAngle = new(0x1667D2C6);
    public static readonly Crc PartList = new(0xBB0BF4A2);
    public static readonly Crc Unk0xE3EB3242 = new(0xE3EB3242);
    public static readonly Crc GroupName = new(0xC8D0F617);
    public static readonly Crc DamageGroups = new(0x49D5C2AE);
    public static readonly Crc Unk0x3C56B891 = new(0x3C56B891);
    public static readonly Crc Unk0x75791974 = new(0x75791974);
    public static readonly Crc Unk0x24E96E81 = new(0x24E96E81);
    public static readonly Crc Unk0xB37BF5B2 = new(0xB37BF5B2);
    public static readonly Crc Unk0x81CD9A01 = new(0x81CD9A01);
    public static readonly Crc Unk0xF7C72618 = new(0xF7C72618);
    public static readonly Crc Unk0x54019561 = new(0x54019561);
    public static readonly Crc Unk0x4F18FA7A = new(0x4F18FA7A);
    public static readonly Crc Unk0x12066797 = new(0x12066797);
    public static readonly Crc Unk0x2D96C4F2 = new(0x2D96C4F2);
    public static readonly Crc Unk0x8ECB69DB = new(0x8ECB69DB);
    public static readonly Crc Unk0xF9C458E7 = new(0xF9C458E7);
    public static readonly Crc Unk0x6AC12B1E = new(0x6AC12B1E);
    public static readonly Crc Unk0x0E29263B = new(0x0E29263B);
    public static readonly Crc Unk0x3D960EA8 = new(0x3D960EA8);
    public static readonly Crc Unk0x2A3029F3 = new(0x2A3029F3);
    public static readonly Crc PartImpulse = new(0x97E0DB0F);
    public static readonly Crc Unk0x414631B3 = new(0x414631B3);
    public static readonly Crc Unk0x49C74990 = new(0x49C74990);
    public static readonly Crc Unk0xA1B82BA7 = new(0xA1B82BA7);
    public static readonly Crc Unk0xC11C102E = new(0xC11C102E);
    public static readonly Crc Unk0x416E074F = new(0x416E074F);
    public static readonly Crc Unk0x7C203814 = new(0x7C203814);
    public static readonly Crc Unk0x73FF7621 = new(0x73FF7621);
    public static readonly Crc Unk0xD0E3A1A1 = new(0xD0E3A1A1);
    public static readonly Crc Unk0xDA3A3781 = new(0xDA3A3781);
    public static readonly Crc Unk0xAC0F66B8 = new(0xAC0F66B8);
    public static readonly Crc ExplosionOnDeath = new(0x82E62211);
    public static readonly Crc AreaDamage = new(0x3121EECD);
    public static readonly Crc Unk0xC86877FD = new(0xC86877FD);
    public static readonly Crc Unk0x55817592 = new(0x55817592);
    public static readonly Crc Unk0x496A25D2 = new(0x496A25D2);
    public static readonly Crc Unk0xFD4869CD = new(0xFD4869CD);
    public static readonly Crc List = new(0x1AAC9531);

    public static readonly Crc Unk0x6DD070B1 = new(0x6DD070B1);
    public static readonly Crc Unk0x30812467 = new(0x30812467);
    public static readonly Crc Unk0x3885F292 = new(0x3885F292);
    public static readonly Crc Unk0x558C91D7 = new(0x558C91D7);
    public static readonly Crc Unk0x22650220 = new(0x22650220);
    public static readonly Crc Unk0x03E19511 = new(0x03E19511);
    public static readonly Crc Unk0x187B88F6 = new(0x187B88F6);
    public static readonly Crc BoneName = new(0x207FBA52);
    public static readonly Crc DamageWeakness = new(0x9465294B);
    public static readonly Crc Unk0x7DB13D1F = new(0x7DB13D1F);
    public static readonly Crc Pristine = new(0x86DE6639);
    public static readonly Crc DamageInitialHealth = new(0x8D117B98);
    public static readonly Crc PartTimer = new(0xC4C441BD);
    public static readonly Crc DamageGroup = new(0xDD2AA7AB);
    #endregion

    #region Types
    public static readonly Crc CommonUIPersistent = new(0x7434754C);
    public static readonly Crc Common = new(0x2447DFFA);
    public static readonly Crc SingleImage = new(0xA89A5F4C);
    public static readonly Crc InteriorImages = new(0xC3B9DAAB);
    public static readonly Crc AIPathPt = new(0x2736360A);

    public static readonly Crc CreditName = new(0xFE44888F);
    public static readonly Crc ImageFolder = new(0xD8E3E358);
    public static readonly Crc Rumble = new(0xF49470BA);
    public static readonly Crc Cinematics = new(0x0F7BD3F7);
    public static readonly Crc WaterParticleFx = new(0x67B79A3A);
    public static readonly Crc VerletBoneObject = new(0x5C4D7FF4);
    public static readonly Crc LeafSpawner = new(0x32B6B82F);
    public static readonly Crc WaterTexture = new(0xD0B706D3);
    public static readonly Crc WaterController = new(0x1AECDEB6);
    public static readonly Crc FoliageFx = new(0xD26EF074);
    public static readonly Crc SeatAnimationsDriver = new(0x98F0845D);
    public static readonly Crc SeatAnimationsSearcher = new(0x3740085A);
    public static readonly Crc SeatAnimationsGunner = new(0xC59EAF6A);
    public static readonly Crc SeatAnimationsPassenger = new(0xD30EC2FD);
    public static readonly Crc SeatAnimations = new(0xA964185F);
    public static readonly Crc AIChatter = new(0x669746CA);
    public static readonly Crc AIChatterSet = new(0xD414DC12);
    public static readonly Crc EscHWTF = new(0x954C883D);
    public static readonly Crc FlashMovie = new(0x9CC86FE3);
    public static readonly Crc Weapon = new(0x787C0871);
    public static readonly Crc MeleeWeapon = new(0x07370953);
    public static readonly Crc Ammo = new(0xBA16485F);
    public static readonly Crc Bullet = new(0x74FD1BA9);
    public static readonly Crc FlameOrdnance = new(0x9961B9C6);
    public static readonly Crc PhysicalOrdnance = new(0x284C8026);
    public static readonly Crc Rocket = new(0x5434C7ED);
    public static readonly Crc HumanPhysics = new(0xCB248263);
    public static readonly Crc Item = new(0x71798A24);
    public static readonly Crc Damageable = new(0x6068DB46);
    public static readonly Crc Unknown = new(0x4D2A3469);
    public static readonly Crc Audible = new(0xCC1EE14D);
    public static readonly Crc Ordnance = new(0x277B8B55);
    public static readonly Crc Targetable = new(0xF0DDFE9E);
    public static readonly Crc Controllable = new(0x2C70C910);
    public static readonly Crc Player = new(0x600E3B1E);
    public static readonly Crc Human = new(0xAD431BF0);
    public static readonly Crc CameraSet = new(0xF66B2194);
    public static readonly Crc Car = new(0xCE27C791);
    public static readonly Crc Vehicle = new(0x2B8D1FDF);
    public static readonly Crc Truck = new(0xFA10EAC6);
    public static readonly Crc APC = new(0x9CDBD62D);
    public static readonly Crc Tank = new(0xC686AE99);
    public static readonly Crc Aimer = new(0x1F18A531);
    public static readonly Crc Spore = new(0xBF708C00);
    public static readonly Crc VehicleCollision = new(0x8848D9D1);
    public static readonly Crc PlayerCollision = new(0x05568616);
    public static readonly Crc Prop = new(0xCEB60D96);
    public static readonly Crc ModelRenderable = new(0x38D88BB8);
    public static readonly Crc LightAttachement = new(0xA3613F4D);
    public static readonly Crc AttrPtAttachable = new(0xB88094A3);
    public static readonly Crc TrainList = new(0x3F7ED4CD);
    public static readonly Crc Train = new(0xB1796D79);
    public static readonly Crc Foliage = new(0x34612F86);
    public static readonly Crc Trappable = new(0xB2C8FC90);
    public static readonly Crc AIAttractionPt = new(0x92473A24);
    public static readonly Crc DamageSphere = new(0x6898A77D);
    public static readonly Crc DamageRegion = new(0xC87F672A);
    public static readonly Crc Explosion = new(0x16930AFE);
    public static readonly Crc AIController = new(0x001BAB2B);
    public static readonly Crc ScriptController = new(0x7A6E9D28);
    public static readonly Crc AISpawner = new(0xAD3EFBE5);
    public static readonly Crc AICombatParams = new(0xC43B697D);
    public static readonly Crc AICrowdBlocker = new(0x4751C524);
    public static readonly Crc GlobalHumanParams = new(0x9BD2B975);
    public static readonly Crc Melee = new(0x0A9A5883);
    public static readonly Crc ParticleEffect = new(0x4B19B750);
    public static readonly Crc RadialBlur = new(0x72F138CF);
    public static readonly Crc Sound = new(0xDD62BA1A);
    public static readonly Crc Turret = new(0x01212327);
    public static readonly Crc SearchTurret = new(0x1F72E643);
    public static readonly Crc TrainCarriage = new(0x28DBEF3F);
    public static readonly Crc TrainEngine = new(0x060D8FC7);
    public static readonly Crc TrainItem = new(0xE2E68598);
    public static readonly Crc SlowMotionCamera = new(0xE0BB7B09);
    public static readonly Crc ElasticTransition = new(0xDC07C4C3);
    public static readonly Crc AnimatedTransition = new(0x153A0DF7);
    public static readonly Crc GroupTransition = new(0xAAA2D2A7);
    public static readonly Crc ScopeTransition = new(0x148C19AA);
    public static readonly Crc CameraSettings = new(0x0BB693A3);
    public static readonly Crc GroupCameraSettings = new(0x175F15E4);
    public static readonly Crc CameraSettingsMisc = new(0xB8F740AF);
    public static readonly Crc ToneMapping = new(0x0C8FC411);
    public static readonly Crc LightSettings = new(0x7A55C8F8);
    public static readonly Crc LightVolume = new(0xD3DFD699);
    public static readonly Crc sub_7FB4D0 = new(0xD0C6D015);
    public static readonly Crc GunnerSeat = new(0x1A8CEE3F);
    public static readonly Crc SearcherSeat = new(0xA95AADD7);
    public static readonly Crc ClothObject = new(0x56A984DE);
    public static readonly Crc ClothForce = new(0x060FD1B4);
    public static readonly Crc WillToFight = new(0xFE6CDF30);
    public static readonly Crc WillToFightNode = new(0x0584B7F4);
    public static readonly Crc WillToFightPortal = new(0xD359BD64);
    public static readonly Crc HealthEffectFilter = new(0x932C532A);
    public static readonly Crc ParticleEffectSpawner = new(0x9DCC91D0);
    public static readonly Crc ButtonPrompt = new(0x35899B4F);
    public static readonly Crc FxBoneStateList = new(0x6D73FB6C);
    public static readonly Crc FxHumanHead = new(0x7DAB4B80);
    public static readonly Crc FxHumanBodyPart = new(0x08271F91);
    public static readonly Crc FxHumanBodySetup = new(0xD9F7FCE1);
    public static readonly Crc FaceExpression = new(0x5A8EE34C);
    public static readonly Crc HumanBodyPart = new(0x52EF7807);
    public static readonly Crc HumanBodySetup = new(0x4DED4763);
    public static readonly Crc HumanSkeletonScale = new(0x5822D085);
    public static readonly Crc AnimatedObject = new(0x3F615DD3);
    public static readonly Crc RandomObj = new(0x75F73A4F);
    public static readonly Crc BonePair = new(0xFD1DAA2F);
    public static readonly Crc BridgeController = new(0x22DC02B6);
    public static readonly Crc Ricochet = new(0x73393C4A);
    public static readonly Crc AIRoad = new(0x2E0750D5);
    public static readonly Crc AISidewalk = new(0x70B6908D);
    public static readonly Crc Highlight = new(0x1CC87777);
    public static readonly Crc MiniGame = new(0xBBDF8DF6);
    public static readonly Crc SabotageTarget = new(0xC5B4B2B6);
    public static readonly Crc BigMap = new(0x3E85904D);
    public static readonly Crc ItemCache = new(0x50C4254A);
    public static readonly Crc VirVehicleWheel = new(0xAFC35781);
    public static readonly Crc VirVehicleTransmission = new(0xDD0A5AF8);
    public static readonly Crc VirVehicleEngine = new(0x91EB2C3E);
    public static readonly Crc VirVehicleChassis = new(0x30970D40);
    public static readonly Crc VirVehicleSetup = new(0x798C1EC7);
    public static readonly Crc VehicleWheelFX = new(0x873E5AC6);
    public static readonly Crc Difficulty = new(0x4C38AB2E);
    public static readonly Crc Shop = new(0x6C266E39);
    public static readonly Crc Perks = new(0xA8CF8D40);
    public static readonly Crc PerkFactors = new(0xBDAA3FB7);
    public static readonly Crc PhysicsParticleSet = new(0xC30934DA);
    public static readonly Crc PhysicsParticle = new(0xFC8B1DA2);
    public static readonly Crc Decal = new(0x209311CC);
    public static readonly Crc DetailObject = new(0xFC92396B);
    public static readonly Crc CivilianProp = new(0xE7B1D609);
    public static readonly Crc Bird = new(0xFF22E748);
    public static readonly Crc BirdSpawner = new(0x7E2A6F48);
    public static readonly Crc Escalation = new(0xF69DEC26);
    public static readonly Crc WaterFlow = new(0xE9162F96);
    public static readonly Crc DamageableRoot = new(0x755FC33E);
    public static readonly Crc PhysicsVehicle = new(0x0E6FC89C);
    public static readonly Crc DamageablePart = new(0xAEE470F5);
    public static readonly Crc PhysicsVehicleWheel = new(0x5B71D0F5);
    public static readonly Crc SeatWithMount = new(0x77F3BA45);
    public static readonly Crc Seat = new(0x10B39BE6);
    public static readonly Crc TweakableColors = new(0x1935B883);
    public static readonly Crc CommonUI_Persistent = new(0x7434754C);
    public static readonly Crc Occluder = new(0x359D640);
    public static readonly Crc TriggerPolygon = new(0xAF36A899);
    public static readonly Crc TriggerRain = new(0x976F2D35);
    public static readonly Crc TriggerZone = new(0x0A0AFF97);
    public static readonly Crc TriggerSound = new(0x9EE26816);
    public static readonly Crc LocatorScripted = new(0x6542BB4B);
    public static readonly Crc LocatorGarage = new(0xDE850532);
    public static readonly Crc Locator = new(0x7D718595);
    public static readonly Crc Rope = new(0x330B1105);
    public static readonly Crc DetailBlock = new(0xC3FA36EA);
    public static readonly Crc Module = new(0x2730D015);
    #endregion
    #endregion
}
