using SabTool.Utils.Extensions;

namespace SabTool.GameData.Blueprints;

public class DamageablePartBlueprintPart : DamageableBlueprintPart
{
    public float Prop0x6DD070B1 { get; set; } = 1.0f;
    public string Prop0x30812467 { get; set; } = string.Empty;
    public bool Prop0x3885F292 { get; set; }
    public string Prop0x558C91D7 { get; set; } = string.Empty;
    public bool Prop0x22650220 { get; set; } = true;
    public float Prop0x03E19511 { get; set; } = 0.5f;
    public string Prop0x187B88F6 { get; set; } = string.Empty;
    public Crc BoneName { get; set; } = Crcs.ZERO;
    public float DamageWeakness { get; set; }
    public string Prop0x7DB13D1F { get; set; } = string.Empty;
    public bool Pristine { get; set; }
    public float DamageInitialHealth { get; set; }
    public float PartTimer { get; set; } = 60.0f;
    public Crc DamageGroup { get; set; } = Crcs.ZERO;

    public override bool SetProperty(Crc field, PropertySet properties)
    {
        if (field == Crcs.Unk0x6DD070B1)
        {
            Prop0x6DD070B1 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x30812467)
        {
            Prop0x30812467 = properties.ReadStringWithMaxLength(64);
            return true;
        }

        if (field == Crcs.Unk0x3885F292)
        {
            Prop0x3885F292 = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.Unk0x558C91D7)
        {
            Prop0x558C91D7 = properties.ReadStringWithMaxLength(64);
            return true;
        }

        if (field == Crcs.Unk0x22650220)
        {
            Prop0x22650220 = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.Unk0x03E19511)
        {
            Prop0x03E19511 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x187B88F6)
        {
            Prop0x187B88F6 = properties.ReadStringWithMaxLength(64);
            return true;
        }

        if (field == Crcs.BoneName)
        {
            BoneName = new Crc(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.DamageWeakness)
        {
            DamageWeakness = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x7DB13D1F)
        {
            Prop0x7DB13D1F = properties.ReadStringWithMaxLength(64);
            return true;
        }

        if (field == Crcs.Pristine)
        {
            Pristine = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.DamageInitialHealth)
        {
            DamageInitialHealth = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.PartTimer)
        {
            PartTimer = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.DamageGroup)
        {
            DamageGroup = new Crc(properties.ReadUInt32());
            return true;
        }

        return base.SetProperty(field, properties);
    }

    public override int WriteProperties(PropertyWriter writer)
    {
        var count = 0;

        Blueprint.WriteCrcProperty(writer, Crcs.BoneName, BoneName);

        if (DamageInitialHealth != -1.0f)
        {
            Blueprint.WriteFloatProperty(writer, Crcs.DamageInitialHealth, DamageInitialHealth);

            ++count;
        }

        if (DamageWeakness != -1.0f)
        {
            Blueprint.WriteFloatProperty(writer, Crcs.DamageWeakness, DamageWeakness);

            ++count;
        }

        if (PartTimer != 60.0f)
        {
            Blueprint.WriteFloatProperty(writer, Crcs.PartTimer, PartTimer);

            ++count;
        }

        if (!string.IsNullOrEmpty(Prop0x7DB13D1F))
        {
            Blueprint.WriteStringProperty(writer, Crcs.Unk0x7DB13D1F, Prop0x7DB13D1F, 64);

            ++count;
        }

        if (!string.IsNullOrEmpty(Prop0x558C91D7))
        {
            Blueprint.WriteStringProperty(writer, Crcs.Unk0x558C91D7,Prop0x558C91D7, 64);

            ++count;
        }

        if (!string.IsNullOrEmpty(Prop0x30812467))
        {
            Blueprint.WriteStringProperty(writer, Crcs.Unk0x30812467, Prop0x30812467, 64);

            ++count;
        }

        if (Prop0x03E19511 != 0.5f)
        {
            Blueprint.WriteFloatProperty(writer, Crcs.Unk0x03E19511, Prop0x03E19511);

            ++count;
        }

        if (!string.IsNullOrEmpty(Prop0x187B88F6))
        {
            Blueprint.WriteStringProperty(writer, Crcs.Unk0x187B88F6, Prop0x187B88F6, 64);

            ++count;
        }

        if (Prop0x6DD070B1 != 1.0f)
        {
            Blueprint.WriteFloatProperty(writer, Crcs.Unk0x6DD070B1, Prop0x6DD070B1);

            ++count;
        }

        Blueprint.WriteBoolProperty(writer, Crcs.Unk0x3885F292, Prop0x3885F292);
        Blueprint.WriteBoolProperty(writer, Crcs.Unk0x22650220, Prop0x22650220);
        Blueprint.WriteBoolProperty(writer, Crcs.Pristine, Pristine);
        Blueprint.WriteCrcProperty(writer, Crcs.DamageGroup, DamageGroup);

        return 5 + count;
    }

    public override void DumpFields(StringBuilder sb)
    {
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.BoneName, BoneName);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.DamageInitialHealth, DamageInitialHealth);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.DamageWeakness, DamageWeakness);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.PartTimer, PartTimer);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.Unk0x7DB13D1F, Prop0x7DB13D1F);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.Unk0x558C91D7, Prop0x558C91D7);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.Unk0x30812467, Prop0x30812467);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.Unk0x03E19511, Prop0x03E19511);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.Unk0x187B88F6, Prop0x187B88F6);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.Unk0x6DD070B1, Prop0x6DD070B1);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.Unk0x3885F292, Prop0x3885F292);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.Unk0x22650220, Prop0x22650220);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.Pristine, Pristine);
        Blueprint.DumpField(sb, Crcs.DamageablePart, Crcs.DamageGroup, DamageGroup);
    }
}
