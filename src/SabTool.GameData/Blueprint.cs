namespace SabTool.GameData;

using SabTool.GameData.Blueprints;

[Flags]
public enum BlueprintFlags : byte
{
    None           = 0,
    Dynamic        = 1,
    UseDynamicPool = 2,
    Managed        = 4,
    Unknown8       = 8
}

public abstract class Blueprint
{
    public Crc Type { get; }
    public Crc Name { get; }
    public Crc Backup { get; set; } = Crcs.NONE;
    public byte Priority { get; set; }
    public BlueprintFlags Flags { get; set; }

    public bool IsDynamic => (Flags & BlueprintFlags.Dynamic) == BlueprintFlags.Dynamic;
    public bool UseDynamicPool => (Flags & BlueprintFlags.UseDynamicPool) == BlueprintFlags.UseDynamicPool;
    public bool IsManaged => (Flags & BlueprintFlags.Managed) == BlueprintFlags.Managed;

    public Blueprint(Crc type, Crc name)
    {
        Type = type;
        Name = name;
    }

    public virtual void Init(string name)
    {
    }

    public virtual void PreLoad()
    {
    }

    public virtual bool SetProperty(Crc field, PropertySet properties)
    {
        return false;
    }

    public virtual void PostLoad()
    {
    }

    public void SetProperties(PropertySet properties)
    {
        PreLoad();

        while (properties.BaseStream.Position < properties.BaseStream.Length)
        {
            var fieldName = new Crc(properties.ReadUInt32());
            if (fieldName == Crcs.NONE)
                break;

            var fieldSize = properties.ReadInt32();

            var startOffset = properties.BaseStream.Position;

            if (!SetProperty(fieldName, properties) && !SetCommonProperty(fieldName, properties))
                properties.BaseStream.Position += fieldSize;

            if (startOffset + fieldSize != properties.BaseStream.Position)
                throw new Exception($"Over or under reading blueprint properties! Start: {startOffset}, size: {fieldSize}, end: {properties.BaseStream.Position}, expectedEnd: {startOffset + fieldSize}");
        }

        PostLoad();
    }

    public bool SetCommonProperty(Crc property, PropertySet properties)
    {
        var handled = false;
        // TODO: cast to DamageableBlueprint and SetProperty

        // TODO: cast to AudibleBlueprint and SetProperty

        // TODO: cast to ModelRenderableBlueprint and SetProperty

        // TODO: cast to TargetableBlueprint and SetProperty

        // TODO: cast to AIAttractionBlueprint and SetProperty

        // TODO: cast to ControlalbleBlueprint and SetProperty

        if (property == Crcs.Dynamic)
        {
            if (properties.ReadBoolean())
                Flags |= BlueprintFlags.Dynamic;
            else
                Flags &= ~BlueprintFlags.Dynamic;

            return true;
        }

        if (property == Crcs.UseDynamicPool)
        {
            if (properties.ReadBoolean())
                Flags |= BlueprintFlags.UseDynamicPool;
            else
                Flags &= ~BlueprintFlags.UseDynamicPool;

            return true;
        }

        if (property == Crcs.Priority)
        {
            var prio = properties.ReadInt32();

            Priority = prio < 0 ? (byte)0 : (prio > 255 ? (byte)0xFF : (byte)prio);

            return true;
        }

        if (property == Crcs.Backup)
        {
            Backup = new Crc(properties.ReadUInt32());

            return true;
        }

        if (property == Crcs.Managed)
        {
            if (properties.ReadBoolean())
                Flags |= BlueprintFlags.Managed;
            else
                Flags &= ~BlueprintFlags.Managed;

            return true;
        }

        return handled;
    }

    public static Blueprint? Create(string type, string name, PropertySet properties)
    {
        var typeCrc = new Crc(Hash.StringToHash(type));
        var nameCrc = new Crc(Hash.StringToHash(name));

        Blueprint? newBlueprint = null;

        if (typeCrc == Crcs.Weapon)
            newBlueprint = new WeaponBlueprint(typeCrc, nameCrc);
        else if (typeCrc == Crcs.FlashMovie)
            newBlueprint = new FlashBlueprint(typeCrc, nameCrc);

        // create blueprint types

        if (typeCrc == Crcs.CommonUIPersistent ||
            typeCrc == Crcs.Common ||
            typeCrc == Crcs.SingleImage ||
            typeCrc == Crcs.InteriorImages ||
            typeCrc == Crcs.AIPathPt)
            return newBlueprint;

        if (newBlueprint is not null)
        {
            newBlueprint.Priority = 0;

            newBlueprint.Init(name);
            newBlueprint.SetProperties(properties);
        }

        return newBlueprint;
    }

    public virtual int WriteProperties(PropertyWriter writer)
    {
        return 0;
    }

    protected bool WriteCommonProperty(Crc property, PropertyWriter writer)
    {
        if (property == Crcs.Dynamic)
        {
            writer.Write(property.Value);
            writer.Write(1);
            writer.Write(IsDynamic);

            return true;
        }

        if (property == Crcs.UseDynamicPool)
        {
            writer.Write(property.Value);
            writer.Write(1);
            writer.Write(UseDynamicPool);

            return true;
        }

        if (property == Crcs.Priority)
        {
            writer.Write(property.Value);
            writer.Write(4);
            writer.Write((int)Priority);

            return true;
        }

        if (property == Crcs.Backup)
        {
            writer.Write(property.Value);
            writer.Write(4);
            writer.Write(Backup.Value);

            return true;
        }

        if (property == Crcs.Managed)
        {
            writer.Write(property.Value);
            writer.Write(1);
            writer.Write(IsManaged);

            return true;
        }

        return false;
    }

    public static void WriteBoolProperty(PropertyWriter writer, Crc property, bool value)
    {
        writer.Write(property.Value);
        writer.Write(1);
        writer.Write(value);
    }

    public static void WriteIntProperty(PropertyWriter writer, Crc property, int value)
    {
        writer.Write(property.Value);
        writer.Write(4);
        writer.Write(value);
    }

    public static void WriteUintProperty(PropertyWriter writer, Crc property, uint value)
    {
        writer.Write(property.Value);
        writer.Write(4);
        writer.Write(value);
    }

    public static void WriteCrcProperty(PropertyWriter writer, Crc property, Crc? value)
    {
        writer.Write(property.Value);
        writer.Write(4);

        if (value is null)
            writer.Write(Crcs.NONE.Value);
        else
            writer.Write(value.Value);
    }

    public static void WriteStringProperty(PropertyWriter writer, Crc property, string value, int maxLength)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var len = Math.Min(maxLength, bytes.Length);

        writer.Write(property.Value);
        writer.Write(len);
        writer.Write(bytes, 0, len);
    }

    public static void WriteFloatProperty(PropertyWriter writer, Crc property, float value)
    {
        writer.Write(property.Value);
        writer.Write(4);
        writer.Write(value);
    }

    public static void WriteVector2Property(PropertyWriter writer, Crc property, Vector2 value)
    {
        writer.Write(property.Value);
        writer.Write(8);
        writer.Write(value.X);
        writer.Write(value.Y);
    }

    #region Field dumping
    public string Dump()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Blueprint({Type.GetStringOrHexString()}, {Name.GetStringOrHexString()}):");

        DumpFields(sb);

        return sb.ToString();
    }

    public static void DumpField(StringBuilder sb, Crc type, Crc field, object value)
    {
        sb.AppendLine($"[{field.GetHexString()}][{type.GetString(),-30}][{field.GetStringOrHexString(),-30}]: {value}");
    }

    protected virtual void DumpFields(StringBuilder sb)
    {
    }
    #endregion
}

public interface IBlueprintPart
{
    bool SetProperty(Crc field, PropertySet properties);
    int WriteProperties(PropertyWriter writer);
    void DumpFields(StringBuilder sb);
}

/*
    public Blueprint(Crc name)
        : base(name)
    {
    }

    public override bool SetProperty(Crc field, PropertySet properties)
    {

        return base.SetProperty(field, properties);
    }

    public override void WriteProperties(PropertyWriter writer)
    {
        WriteCommonProperty(Crcs.Dynamic, writer);
        WriteCommonProperty(Crcs.Managed, writer);
    } 
*/
