namespace SabTool.GameData.Blueprints;

public class WeaponBlueprint : ItemBlueprint
{
    public DamageableRootBlueprintPart DamageableRootBlueprint { get; } = new();
    public bool Prop0x7C0B86B9 { get; set; }
    public Crc Prop0x334A7577 { get; set; }
    public float AIMaxMissAngle { get; set; }

    public WeaponBlueprint(Crc type, Crc name)
        : base(type, name)
    {
    }

    public override bool SetProperty(Crc field, PropertySet properties)
    {
        if (base.SetProperty(field, properties) || DamageableRootBlueprint.SetProperty(field, properties))
            return true;

        if (field == Crcs.Unk0x7C0B86B9)
        {
            Prop0x7C0B86B9 = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.Unk0x334A7577)
        {
            Prop0x334A7577 = new Crc(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.AIMaxMissAngle)
        {
            AIMaxMissAngle = properties.ReadSingle();
            return true;
        }

        return false;
    }

    public override int WriteProperties(PropertyWriter writer)
    {
        WriteCommonProperty(Crcs.Dynamic, writer);
        WriteCommonProperty(Crcs.Managed, writer);

        return 2;
    }
}
