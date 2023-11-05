namespace SabTool.GameData.Blueprints;

public class FlashBlueprint : Blueprint
{
    public FlashBlueprint(Crc type, Crc name) 
        : base(type, name)
    {
    }

    public int Version { get; set; }
    public Crc FlashFile { get; set; } = Crcs.ZERO;

    public override bool SetProperty(Crc field, PropertySet properties)
    {
        if (field == Crcs.Version)
        {
            Version = properties.ReadInt32();
            return true;
        }

        if (field == Crcs.FlashFile)
        {
            FlashFile = new(properties.ReadUInt32());
            return true;
        }

        return false;
    }

    public override int WriteProperties(PropertyWriter writer)
    {
        WriteCommonProperty(Crcs.Dynamic, writer);
        WriteCommonProperty(Crcs.Managed, writer);

        WriteIntProperty(writer, Crcs.Version, Version);
        WriteCrcProperty(writer, Crcs.FlashFile, FlashFile);

        return 4;
    }

    protected override void DumpFields(StringBuilder sb)
    {
        DumpField(sb, Crcs.Common, Crcs.Dynamic, IsDynamic);
        DumpField(sb, Crcs.Common, Crcs.Managed, IsManaged);

        DumpField(sb, Type, Crcs.Version, Version);
        DumpField(sb, Type, Crcs.FlashFile, FlashFile);
    }
}
