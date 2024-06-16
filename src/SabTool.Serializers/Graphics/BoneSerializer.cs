using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Graphics;

using SabTool.Data.Graphics;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class BoneSerializer
{
    public static Bone DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var bone = new Bone();

        var currentStart = stream.Position;

        bone.DebugName = new Crc(reader.ReadUInt32());
        bone.LockTranslation = reader.ReadBoolean();

        stream.Position += 0xF;

        bone.CrcName = new Crc(reader.ReadUInt32());
        bone.Flags = reader.ReadInt16();
        bone.Index = reader.ReadInt16();
        bone.ExportFlags = reader.ReadByte();

        stream.Position += 0x3;

        bone.AABBOffset = reader.ReadVector4();
        bone.AABBSize = reader.ReadVector4();

        if (currentStart + 0x40 != stream.Position)
            throw new Exception($"Under or orver read of the Bone part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x40}");

        return bone;
    }

    public static void SerializeRaw(Bone bone, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

        var currentStart = stream.Position;

        writer.Write(bone.DebugName.Value);
        writer.Write(bone.LockTranslation);

        stream.Position += 0xF;

        writer.Write(bone.CrcName.Value);
        writer.Write(bone.Flags);
        writer.Write(bone.Index);
        writer.Write(bone.ExportFlags);

        stream.Position += 0x3;

        writer.Write(bone.AABBOffset.X);
        writer.Write(bone.AABBOffset.Y);
        writer.Write(bone.AABBOffset.Z);
        writer.Write(bone.AABBOffset.W);
        writer.Write(bone.AABBSize.X);
        writer.Write(bone.AABBSize.Y);
        writer.Write(bone.AABBSize.Z);
        writer.Write(bone.AABBSize.W);

        if (currentStart + 0x40 != stream.Position)
            throw new Exception($"Under or orver write of the Bone part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x40}");
    }

    public static Bone? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Bone bone, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(bone, Formatting.Indented, new CrcConverter()));
    }
}
