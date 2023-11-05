namespace SabTool.GameData.Blueprints;

public class DamageableRootBlueprintPart : DamageableBlueprintPart
{
    public List<DamageablePartBlueprintPart> SubParts { get; } = new();
    public List<DamageableGroup> Groups { get; } = new();

    public override bool SetProperty(Crc field, PropertySet properties)
    {
        if (field == Crcs.PartList)
        {
            SubParts.Add(new DamageablePartBlueprintPart());

            properties.BaseStream.Position += 4;

            return true;
        }

        if (field == Crcs.DamageGroups)
        {
            Groups.Add(new DamageableGroup());

            properties.BaseStream.Position += 4;

            return true;
        }

        if (field == Crcs.Unk0xE3EB3242)
        {
            var data = properties.ReadInt32();

            if (Groups.Count > 0)
                Groups[^1].Int44 = data;

            return true;
        }

        if (field == Crcs.GroupName)
        {
            var data = properties.ReadUInt32();

            if (Groups.Count > 0)
                Groups[^1].Name = new Crc(data);

            return true;
        }

        if (field == Crcs.Unk0x3C56B891)
        {
            var data = properties.ReadUInt32();

            if (Groups.Count > 0)
                Groups[^1].Unk48 = new Crc(data);

            return true;
        }

        if (SubParts.Count == 0 || !SubParts[^1].SetProperty(field, properties))
            return base.SetProperty(field, properties);

        return true;
    }

    public override int WriteProperties(PropertyWriter writer)
    {
        var count = 0;

        if (Groups.Count > 0)
        {
            Blueprint.WriteCrcProperty(writer, Crcs.List, Crcs.DamageGroups);

            ++count;

            foreach (var group in Groups)
            {
                Blueprint.WriteCrcProperty(writer, Crcs.DamageGroups, Crcs.ListElement);
                Blueprint.WriteCrcProperty(writer, Crcs.GroupName, group.Name);

                if (group.Unk48 != Crcs.ZERO)
                {
                    Blueprint.WriteCrcProperty(writer, Crcs.Unk0x3C56B891, group.Unk48);

                    ++count;
                }

                Blueprint.WriteIntProperty(writer, Crcs.Unk0xE3EB3242, group.Int44);

                count += 3;
            }
        }

        if (SubParts.Count > 0)
        {
            Blueprint.WriteCrcProperty(writer, Crcs.List, Crcs.PartList);

            ++count;

            foreach (var part in SubParts)
            {
                Blueprint.WriteCrcProperty(writer, Crcs.PartList, Crcs.ListElement);

                count += 1 + part.WriteProperties(writer);
            }
        }

        return count;
    }

    public override void DumpFields(StringBuilder sb)
    {
        if (Groups.Count > 0)
        {
            Blueprint.DumpField(sb, Crcs.DamageableRoot, Crcs.List, Crcs.DamageGroups);

            foreach (var group in Groups)
            {
                Blueprint.DumpField(sb, Crcs.DamageableRoot, Crcs.DamageGroups, Crcs.ListElement);
                Blueprint.DumpField(sb, Crcs.DamageableRoot, Crcs.GroupName, group.Name);

                if (group.Unk48 != Crcs.ZERO)
                    Blueprint.DumpField(sb, Crcs.DamageableRoot, Crcs.Unk0x3C56B891, group.Unk48);

                Blueprint.DumpField(sb, Crcs.DamageableRoot, Crcs.Unk0xE3EB3242, group.Int44);
            }
        }

        if (SubParts.Count > 0)
        {
            Blueprint.DumpField(sb, Crcs.DamageableRoot, Crcs.List, Crcs.PartList);

            foreach (var part in SubParts)
            {
                Blueprint.DumpField(sb, Crcs.DamageableRoot, Crcs.PartList, Crcs.ListElement);

                part.DumpFields(sb);
            }
        }
    }

    public class DamageableGroup
    {
        public Crc Name { get; set; } = Crcs.ZERO;
        public int Int44 { get; set; }
        public Crc Unk48 { get; set; } = Crcs.ZERO;
    }
}
