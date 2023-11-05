namespace SabTool.GameData.Blueprints;

public class ItemBlueprint : Blueprint
{
    public Crc Model { get; set; } = Crcs.ZERO;
    public Crc SlotType { get; set; } = Crcs.ZERO;
    public bool Equipable { get; set; }
    public bool HiddenWhileEquipped { get; set; }
    public Crc Prop0x6ADE730A { get; set; } = Crcs.NONE;
    public Crc PickupHighlight { get; set; } = Crcs.NONE;
    public Crc DisplayName { get; set; } = Crcs.ZERO;
    public Crc PickupLowAnim { get; set; } = Crcs.ZERO;
    public Crc Prop0x953C60F5 { get; set; } = Crcs.NONE;
    public Crc PickupHighAnim { get; set; } = Crcs.ZERO;
    public bool NoWillToFight { get; set; }
    public Crc SpecialID { get; set; } = Crcs.ZERO;
    public bool IsUniqueItem { get; set; }
    public bool IsDetonatorItem { get; set; }
    public bool CanDrop { get; set; }
    public Crc Label { get; set; } = Crcs.NONE;
    public Crc SubItem1 { get; set; } = Crcs.NONE;
    public int SubItem1Amount { get; set; }
    public Crc SubItem2 { get; set; } = Crcs.ZERO;
    public int SubItem2Amount { get; set; }
    public Crc SubItem3 { get; set; } = Crcs.ZERO;
    public int SubItem3Amount { get; set; }
    public Crc SubItem4 { get; set; } = Crcs.ZERO;
    public int SubItem4Amount { get; set; }
    public bool Prop0x7EDFDECA { get; set; }

    public LightAttachmentBlueprintPart LightAttachmentBlueprint { get; } = new();

    public ItemBlueprint(Crc type, Crc name)
        : base(type, name)
    {
    }

    public override bool SetProperty(Crc field, PropertySet properties)
    {
        if (LightAttachmentBlueprint.SetProperty(field, properties))
            return true;

        if (field == Crcs.Model)
        {
            Model = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.SlotType)
        {
            SlotType = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.Equippable)
        {
            Equipable = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.HiddenWhileEquiped)
        {
            HiddenWhileEquipped = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.Unk0x6ADE730A)
        {
            Prop0x6ADE730A = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.PickupHighlight)
        {
            PickupHighlight = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.DisplayName)
        {
            DisplayName = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.PickupLowAnim)
        {
            PickupLowAnim = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.Unk0x953C60F5)
        {
            Prop0x953C60F5 = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.PickupHighAnim)
        {
            PickupHighAnim = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.NoWillToFight)
        {
            NoWillToFight = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.SpecialID)
        {
            SpecialID = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.IsUniqueItem)
        {
            IsUniqueItem = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.IsDetonatorItem)
        {
            IsDetonatorItem = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.CanDrop)
        {
            CanDrop = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.Label)
        {
            Label = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.SubItem1)
        {
            SubItem1 = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.SubItem1Amount)
        {
            SubItem1Amount = properties.ReadInt32();
            return true;
        }

        if (field == Crcs.SubItem2)
        {
            SubItem2 = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.SubItem2Amount)
        {
            SubItem2Amount = properties.ReadInt32();
            return true;
        }

        if (field == Crcs.SubItem3)
        {
            SubItem3 = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.SubItem3Amount)
        {
            SubItem3Amount = properties.ReadInt32();
            return true;
        }

        if (field == Crcs.SubItem4)
        {
            SubItem4 = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.SubItem4Amount)
        {
            SubItem4Amount = properties.ReadInt32();
            return true;
        }

        if (field == Crcs.Unk0x7EDFDECA)
        {
            Prop0x7EDFDECA = properties.ReadBoolean();
            return true;
        }

        return false;
    }

    public override int WriteProperties(PropertyWriter writer)
    {
        WriteCommonProperty(Crcs.Dynamic, writer);

        WriteCrcProperty(writer, Crcs.Model, Model);
        WriteCrcProperty(writer, Crcs.SlotType, SlotType);
        WriteBoolProperty(writer, Crcs.Equippable, Equipable);
        WriteBoolProperty(writer, Crcs.HiddenWhileEquiped, HiddenWhileEquipped);

        var propCount = LightAttachmentBlueprint.WriteProperties(writer);

        WriteCrcProperty(writer, Crcs.Unk0x6ADE730A, Prop0x6ADE730A);
        WriteCrcProperty(writer, Crcs.PickupHighlight, PickupHighlight);
        WriteCrcProperty(writer, Crcs.DisplayName, DisplayName);
        WriteCrcProperty(writer, Crcs.PickupLowAnim, PickupLowAnim);
        WriteCrcProperty(writer, Crcs.Unk0x953C60F5, Prop0x953C60F5);
        WriteCrcProperty(writer, Crcs.PickupHighAnim, PickupHighAnim);
        WriteBoolProperty(writer, Crcs.NoWillToFight, NoWillToFight);
        WriteCrcProperty(writer, Crcs.SpecialID, SpecialID);
        WriteBoolProperty(writer, Crcs.IsUniqueItem, IsUniqueItem);
        WriteBoolProperty(writer, Crcs.IsDetonatorItem, IsDetonatorItem);
        WriteBoolProperty(writer, Crcs.CanDrop, CanDrop);
        WriteCrcProperty(writer, Crcs.Label, Label);
        WriteCrcProperty(writer, Crcs.SubItem1, SubItem1);
        WriteIntProperty(writer, Crcs.SubItem1Amount, SubItem1Amount);
        WriteCrcProperty(writer, Crcs.SubItem2, SubItem2);
        WriteIntProperty(writer, Crcs.SubItem2Amount, SubItem2Amount);
        WriteCrcProperty(writer, Crcs.SubItem3, SubItem3);
        WriteIntProperty(writer, Crcs.SubItem3Amount, SubItem3Amount);
        WriteCrcProperty(writer, Crcs.SubItem4, SubItem4);
        WriteIntProperty(writer, Crcs.SubItem4Amount, SubItem4Amount);
        WriteBoolProperty(writer, Crcs.Unk0x7EDFDECA, Prop0x7EDFDECA);

        return 26 + propCount;
    }

    protected override void DumpFields(StringBuilder sb)
    {
        DumpField(sb, Crcs.Common, Crcs.Dynamic, IsDynamic);

        DumpField(sb, Crcs.Item, Crcs.Model, Model);
        DumpField(sb, Crcs.Item, Crcs.SlotType, SlotType);
        DumpField(sb, Crcs.Item, Crcs.Equippable, Equipable);
        DumpField(sb, Crcs.Item, Crcs.HiddenWhileEquiped, HiddenWhileEquipped);

        LightAttachmentBlueprint.DumpFields(sb);

        DumpField(sb, Crcs.Item, Crcs.Unk0x6ADE730A, Prop0x6ADE730A);
        DumpField(sb, Crcs.Item, Crcs.PickupHighlight, PickupHighlight);
        DumpField(sb, Crcs.Item, Crcs.DisplayName, DisplayName);
        DumpField(sb, Crcs.Item, Crcs.PickupLowAnim, PickupLowAnim);
        DumpField(sb, Crcs.Item, Crcs.Unk0x953C60F5, Prop0x953C60F5);
        DumpField(sb, Crcs.Item, Crcs.PickupHighAnim, PickupHighAnim);
        DumpField(sb, Crcs.Item, Crcs.NoWillToFight, NoWillToFight);
        DumpField(sb, Crcs.Item, Crcs.SpecialID, SpecialID);
        DumpField(sb, Crcs.Item, Crcs.IsUniqueItem, IsUniqueItem);
        DumpField(sb, Crcs.Item, Crcs.IsDetonatorItem, IsDetonatorItem);
        DumpField(sb, Crcs.Item, Crcs.CanDrop, CanDrop);
        DumpField(sb, Crcs.Item, Crcs.Label, Label);
        DumpField(sb, Crcs.Item, Crcs.SubItem1, SubItem1);
        DumpField(sb, Crcs.Item, Crcs.SubItem1Amount, SubItem1Amount);
        DumpField(sb, Crcs.Item, Crcs.SubItem2, SubItem2);
        DumpField(sb, Crcs.Item, Crcs.SubItem2Amount, SubItem2Amount);
        DumpField(sb, Crcs.Item, Crcs.SubItem3, SubItem3);
        DumpField(sb, Crcs.Item, Crcs.SubItem3Amount, SubItem3Amount);
        DumpField(sb, Crcs.Item, Crcs.SubItem4, SubItem4);
        DumpField(sb, Crcs.Item, Crcs.SubItem4Amount, SubItem4Amount);
        DumpField(sb, Crcs.Item, Crcs.Unk0x7EDFDECA, Prop0x7EDFDECA);
    }
}
