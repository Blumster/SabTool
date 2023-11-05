namespace SabTool.GameData.Blueprints;

public class LightAttachmentBlueprintPart : IBlueprintPart
{
    public LightAttachmentData[]? AttachmentDatas { get; set; }

    private int _currentIndex = -1;

    public bool SetProperty(Crc field, PropertySet properties)
    {
        if (field == Crcs.LightHaloAttachments)
        {
            AttachmentDatas = new LightAttachmentData[properties.ReadInt32()];

            _currentIndex = -1;

            return true;
        }

        if (AttachmentDatas is null)
            throw new Exception($"LightHaloAttachmentBlueprint properties are in an invalid order!");

        if (field == Crcs.LightHaloAttachment)
        {
            properties.BaseStream.Position += 4;

            ++_currentIndex;

            AttachmentDatas[_currentIndex] = new LightAttachmentData();
            return true;
        }

        if (field == Crcs.LightHalo)
        {
            AttachmentDatas[_currentIndex].Blueprint = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.LightHaloBone)
        {
            AttachmentDatas[_currentIndex].Bone = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.LightHaloRotationBone)
        {
            AttachmentDatas[_currentIndex].RotationBone = new(properties.ReadUInt32());
            return true;
        }

        if (field == Crcs.LightHaloRotationSpeed)
        {
            AttachmentDatas[_currentIndex].RotationSpeed = (float)(properties.ReadSingle() * (Math.PI / 180.0f));
            return true;
        }

        if (field == Crcs.LightHaloRotationAngleLimit)
        {
            AttachmentDatas[_currentIndex].RotationAngleLimit = (float)(properties.ReadSingle() * (Math.PI / 180.0f));
            return true;
        }

        return false;
    }

    public int WriteProperties(PropertyWriter writer)
    {
        if (AttachmentDatas is null)
        {
            Blueprint.WriteIntProperty(writer, Crcs.LightHaloAttachments, 0);
            return 1;
        }

        Blueprint.WriteIntProperty(writer, Crcs.LightHaloAttachments, AttachmentDatas.Length);

        var i = 1;

        foreach (var data in AttachmentDatas)
        {
            Blueprint.WriteCrcProperty(writer, Crcs.LightAttachment, new(Hash.StringToHash($"list element{i++}")));
            Blueprint.WriteCrcProperty(writer, Crcs.Light, data.Blueprint);
            Blueprint.WriteCrcProperty(writer, Crcs.LightBone, data.Bone);
            Blueprint.WriteCrcProperty(writer, Crcs.LightRotationBone, data.RotationBone);
            Blueprint.WriteFloatProperty(writer, Crcs.LightRotationSpeed, data.RotationSpeed);
            Blueprint.WriteFloatProperty(writer, Crcs.LightRotationAngleLimit, data.RotationAngleLimit);
        }

        return 1 + AttachmentDatas.Length * 6;
    }

    public void DumpFields(StringBuilder sb)
    {
        if (AttachmentDatas is null)
        {
            Blueprint.DumpField(sb, Crcs.LightAttachement, Crcs.LightAttachments, 0);
            return;
        }

        Blueprint.DumpField(sb, Crcs.LightAttachement, Crcs.LightAttachments, AttachmentDatas.Length);

        var i = 1;

        foreach (var data in AttachmentDatas)
        {
            Blueprint.DumpField(sb, Crcs.LightAttachement, Crcs.LightAttachment, new Crc(Hash.StringToHash($"list element{i++}")));
            Blueprint.DumpField(sb, Crcs.LightAttachement, Crcs.Light, data.Blueprint);
            Blueprint.DumpField(sb, Crcs.LightAttachement, Crcs.LightBone, data.Bone);
            Blueprint.DumpField(sb, Crcs.LightAttachement, Crcs.LightRotationBone, data.RotationBone);
            Blueprint.DumpField(sb, Crcs.LightAttachement, Crcs.LightRotationSpeed, data.RotationSpeed);
            Blueprint.DumpField(sb, Crcs.LightAttachement, Crcs.LightRotationAngleLimit, data.RotationAngleLimit);
        }
    }

    public class LightAttachmentData
    {
        public Crc Blueprint { get; set; } = Crcs.ZERO;
        public Crc Bone { get; set; } = Crcs.ZERO;
        public Crc RotationBone { get; set; } = Crcs.ZERO;
        public float RotationSpeed { get; set; }
        public float RotationAngleLimit { get; set; }
    }
}
