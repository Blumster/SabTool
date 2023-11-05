namespace SabTool.GameData.Blueprints;

public class LightHaloAttachmentBlueprintPart : IBlueprintPart
{
    public LightHaloAttachmentData[]? AttachmentDatas { get; set; }

    private int _currentIndex = -1;

    public bool SetProperty(Crc field, PropertySet properties)
    {
        if (field == Crcs.LightHaloAttachments)
        {
            AttachmentDatas = new LightHaloAttachmentData[properties.ReadInt32()];

            _currentIndex = -1;

            return true;
        }

        if (AttachmentDatas is null)
            throw new Exception($"LightHaloAttachmentBlueprint properties are in an invalid order!");

        if (field == Crcs.LightHaloAttachment)
        {
            properties.BaseStream.Position += 4;

            ++_currentIndex;

            AttachmentDatas[_currentIndex] = new LightHaloAttachmentData();
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
            Blueprint.WriteCrcProperty(writer, Crcs.LightHaloAttachment, new(Hash.StringToHash($"list element{i++}")));
            Blueprint.WriteCrcProperty(writer, Crcs.LightHalo, data.Blueprint);
            Blueprint.WriteCrcProperty(writer, Crcs.LightHaloBone, data.Bone);
            Blueprint.WriteCrcProperty(writer, Crcs.LightHaloRotationBone, data.RotationBone);
            Blueprint.WriteFloatProperty(writer, Crcs.LightHaloRotationSpeed, data.RotationSpeed);
            Blueprint.WriteFloatProperty(writer, Crcs.LightHaloRotationAngleLimit, data.RotationAngleLimit);
        }

        return 1 + AttachmentDatas.Length * 6;
    }

    public void DumpFields(StringBuilder sb)
    {
        if (AttachmentDatas is null)
        {
            Blueprint.DumpField(sb, Crcs.LightHaloAttachment, Crcs.LightHaloAttachments, 0);
            return;
        }

        Blueprint.DumpField(sb, Crcs.LightHaloAttachment, Crcs.LightHaloAttachments, AttachmentDatas.Length);

        var i = 1;

        foreach (var data in AttachmentDatas)
        {
            Blueprint.DumpField(sb, Crcs.LightHaloAttachment, Crcs.LightHaloAttachment, new Crc(Hash.StringToHash($"list element{i++}")));
            Blueprint.DumpField(sb, Crcs.LightHaloAttachment, Crcs.LightHalo, data.Blueprint);
            Blueprint.DumpField(sb, Crcs.LightHaloAttachment, Crcs.LightHaloBone, data.Bone);
            Blueprint.DumpField(sb, Crcs.LightHaloAttachment, Crcs.LightHaloRotationBone, data.RotationBone);
            Blueprint.DumpField(sb, Crcs.LightHaloAttachment, Crcs.LightHaloRotationSpeed, data.RotationSpeed);
            Blueprint.DumpField(sb, Crcs.LightHaloAttachment, Crcs.LightHaloRotationAngleLimit, data.RotationAngleLimit);
        }
    }

    public class LightHaloAttachmentData
    {
        public Crc Blueprint { get; set; } = Crcs.ZERO;
        public Crc Bone { get; set; } = Crcs.ZERO;
        public Crc RotationBone { get; set; } = Crcs.ZERO;
        public float RotationSpeed { get; set; }
        public float RotationAngleLimit { get; set; }
    }
}
