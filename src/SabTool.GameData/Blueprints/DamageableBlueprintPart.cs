using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SabTool.GameData.Blueprints;

public class DamageableBlueprintPart : IBlueprintPart
{
    public float Weakness { get; set; }
    public float InitialHealth { get; set; }
    public DamageableBlueprintData Data { get; } = new();
    public DamageablePart[]? Parts { get; set; }

    private int _currentReadPartIndex = -1;

    public virtual bool SetProperty(Crc field, PropertySet properties)
    {
        if (field == Crcs.Weakness)
        {
            Weakness = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.InitialHealth)
        {
            InitialHealth = properties.ReadSingle();
            return true;
        }

        // Data
        if (field == Crcs.Unk0x75791974)
        {
            Data.Field4 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x24E96E81)
        {
            Data.Field8 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0xB37BF5B2)
        {
            Data.FieldC = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x81CD9A01)
        {
            Data.Field10 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0xF7C72618)
        {
            Data.Field14 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x54019561)
        {
            Data.Field18 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x4F18FA7A)
        {
            Data.Field1C = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x12066797)
        {
            Data.Field20 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x2D96C4F2)
        {
            Data.Field24 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x8ECB69DB)
        {
            Data.Field28 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0xF9C458E7)
        {
            Data.Field2C = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x6AC12B1E)
        {
            Data.Field30 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x0E29263B)
        {
            Data.Field34 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x3D960EA8)
        {
            Data.Field38 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x2A3029F3)
        {
            Data.Field3C = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0xC11C102E)
        {
            Data.Field40 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x416E074F)
        {
            Data.Field44 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x7C203814)
        {
            Data.Field48 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0x73FF7621)
        {
            Data.Field4C = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0xD0E3A1A1)
        {
            Data.Field50 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0xDA3A3781)
        {
            Data.Field54 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.Unk0xAC0F66B8)
        {
            Data.Field58 = properties.ReadSingle();
            return true;
        }

        if (field == Crcs.ExplosionOnDeath)
        {
            var data = new Crc(properties.ReadUInt32());
            if (data == Crcs.NONE)
                data = Crcs.ZERO;

            Data.ExplosionOnDeath = data;
            return true;
        }

        if (field == Crcs.AreaDamage)
        {
            var data = new Crc(properties.ReadUInt32());
            if (data == Crcs.NONE)
                data = Crcs.ZERO;

            Data.AreaDamage = data;
            return true;
        }

        if (field == Crcs.PartImpulse)
        {
            Data.PartImpulse = properties.ReadSingle();
            return true;
        }

        // Parts
        if (field == Crcs.Unk0x414631B3)
        {
            var count = properties.ReadInt32();

            Parts = new DamageablePart[count];

            for (var i = 0; i < Parts.Length; ++i)
                Parts[i] = new();

            _currentReadPartIndex = -1;

            return true;
        }

        if (field == Crcs.Unk0x49C74990)
        {
            ++_currentReadPartIndex;

            properties.BaseStream.Position += 4;

            return true;
        }

        if (field == Crcs.Unk0xA1B82BA7)
        {
            Parts![_currentReadPartIndex].Field0 = new Crc(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.Unk0xC86877FD)
        {
            Parts![_currentReadPartIndex].Field4 = new Crc(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.Unk0x55817592)
        {
            Parts![_currentReadPartIndex].Field8 = new Crc(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.Unk0x496A25D2)
        {
            Parts![_currentReadPartIndex].FieldC = properties.ReadBoolean();
            return true;
        }

        if (field == Crcs.Unk0xFD4869CD)
        {
            (var x, var y) = (properties.ReadSingle(), properties.ReadSingle());

            if (y > x)
                (x, y) = (y, x);

            Parts![_currentReadPartIndex].Field10 = new Vector2(x, y);

            return true;
        }

        return false;
    }

    public virtual int WriteProperties(PropertyWriter writer)
    {
        return 0;
    }

    public int WritePartsProperties(PropertyWriter writer)
    {
        var count = 0;

        if (Parts is not null)
        {
            Blueprint.WriteIntProperty(writer, Crcs.Unk0x414631B3, Parts.Length);

            var i = 1;

            foreach (var part in Parts)
            {
                Blueprint.WriteCrcProperty(writer, Crcs.Unk0x49C74990, new Crc(Hash.StringToHash($"list element{i++}")));
                Blueprint.WriteCrcProperty(writer, Crcs.Unk0xA1B82BA7, part.Field0);
                Blueprint.WriteCrcProperty(writer, Crcs.Unk0xC86877FD, part.Field4);
                Blueprint.WriteCrcProperty(writer, Crcs.Unk0x55817592, part.Field8);
                Blueprint.WriteVector2Property(writer, Crcs.Unk0xFD4869CD, part.Field10);
                Blueprint.WriteBoolProperty(writer, Crcs.Unk0x496A25D2, part.FieldC);
            }

            count += 1 + Parts.Length * 6;
        }

        return count;
    }

    public int WriteDataProperties(PropertyWriter writer)
    {
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x6AC12B1E, Data.Field30);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x75791974, Data.Field4);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x7C203814, Data.Field48);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x4F18FA7A, Data.Field1C);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x2A3029F3, Data.Field3C);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x81CD9A01, Data.Field10);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0xD0E3A1A1, Data.Field50);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x12066797, Data.Field20);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0xC11C102E, Data.Field40);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0xF7C72618, Data.Field14);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0xAC0F66B8, Data.Field58);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x2D96C4F2, Data.Field24);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x416E074F, Data.Field44);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x54019561, Data.Field18);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0xDA3A3781, Data.Field54);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x8ECB69DB, Data.Field28);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x0E29263B, Data.Field34);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x24E96E81, Data.Field8);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x73FF7621, Data.Field4C);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0xF9C458E7, Data.Field2C);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0x3D960EA8, Data.Field38);
        Blueprint.WriteFloatProperty(writer, Crcs.Unk0xB37BF5B2, Data.FieldC);

        return 22;
    }

    public virtual void DumpFields(StringBuilder sb)
    {
    }

    public void DumpPartsFields(StringBuilder sb)
    {
        if (Parts is not null)
        {
            Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x414631B3, Parts.Length);

            var i = 1;

            foreach (var part in Parts)
            {
                Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x49C74990, new Crc(Hash.StringToHash($"list element{i++}")));
                Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0xA1B82BA7, part.Field0);
                Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0xC86877FD, part.Field4);
                Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x55817592, part.Field8);
                Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0xFD4869CD, part.Field10);
                Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x496A25D2, part.FieldC);
            }
        }
    }

    public void DumpDataFields(StringBuilder sb)
    {
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x6AC12B1E, Data.Field30);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x75791974, Data.Field4);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x7C203814, Data.Field48);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x4F18FA7A, Data.Field1C);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x2A3029F3, Data.Field3C);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x81CD9A01, Data.Field10);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0xD0E3A1A1, Data.Field50);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x12066797, Data.Field20);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0xC11C102E, Data.Field40);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0xF7C72618, Data.Field14);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0xAC0F66B8, Data.Field58);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x2D96C4F2, Data.Field24);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x416E074F, Data.Field44);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x54019561, Data.Field18);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0xDA3A3781, Data.Field54);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x8ECB69DB, Data.Field28);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x0E29263B, Data.Field34);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x24E96E81, Data.Field8);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x73FF7621, Data.Field4C);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0xF9C458E7, Data.Field2C);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0x3D960EA8, Data.Field38);
        Blueprint.DumpField(sb, Crcs.Damageable, Crcs.Unk0xB37BF5B2, Data.FieldC);
    }

    public class DamageableBlueprintData
    {
        public float Field4 { get; set; }
        public float Field8 { get; set; }
        public float FieldC { get; set; }
        public float Field10 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public float Field1C { get; set; }
        public float Field20 { get; set; }
        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public float Field2C { get; set; }
        public float Field30 { get; set; }
        public float Field34 { get; set; }
        public float Field38 { get; set; }
        public float Field3C { get; set; }
        public float Field40 { get; set; }
        public float Field44 { get; set; }
        public float Field48 { get; set; }
        public float Field4C { get; set; }
        public float Field50 { get; set; }
        public float Field54 { get; set; }
        public float Field58 { get; set; }
        public Crc ExplosionOnDeath { get; set; } = Crcs.ZERO;
        public Crc AreaDamage { get; set; } = Crcs.ZERO;
        public float PartImpulse { get; set; }
    }

    public class DamageablePart
    {
        public Crc Field0 { get; set; } = Crcs.ZERO;
        public Crc Field4 { get; set; } = Crcs.ZERO;
        public Crc Field8 { get; set; } = Crcs.ZERO;
        public bool FieldC { get; set; }
        public Vector2 Field10 { get; set; }
    }
}
