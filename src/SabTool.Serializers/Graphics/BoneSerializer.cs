using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Graphics;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;

namespace SabTool.Serializers.Graphics;
public static class BoneSerializer
{
    public static Bone DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        Bone bone = new();

        long currentStart = stream.Position;

        bone.UnkNamePtr = new Crc(reader.ReadUInt32());
        bone.UnkByte = reader.ReadByte();

        stream.Position += 0xF;

        bone.Crc = new Crc(reader.ReadUInt32());
        bone.Flags = reader.ReadInt16();
        bone.Index = reader.ReadInt16();
        bone.UnkFlags = reader.ReadByte();

        stream.Position += 0x3;

        bone.Field20 = reader.ReadSingle();
        bone.Field24 = reader.ReadSingle();
        bone.Field28 = reader.ReadSingle();
        bone.Field2C = reader.ReadInt32();
        bone.Field30 = reader.ReadSingle();
        bone.Field34 = reader.ReadSingle();
        bone.Field38 = reader.ReadSingle();
        bone.Field3C = reader.ReadSingle();

        return currentStart + 0x40 != stream.Position
            ? throw new Exception($"Under or orver read of the Bone part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x40}")
            : bone;
    }

    public static void SerializeRaw(Bone bone, Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        long currentStart = stream.Position;

        writer.Write(bone.UnkNamePtr.Value);
        writer.Write(bone.UnkByte);

        stream.Position += 0xF;

        writer.Write(bone.Crc.Value);
        writer.Write(bone.Flags);
        writer.Write(bone.Index);
        writer.Write(bone.UnkFlags);

        stream.Position += 0x3;

        writer.Write(bone.Field20);
        writer.Write(bone.Field24);
        writer.Write(bone.Field28);
        writer.Write(bone.Field2C);
        writer.Write(bone.Field30);
        writer.Write(bone.Field34);
        writer.Write(bone.Field38);
        writer.Write(bone.Field3C);

        if (currentStart + 0x40 != stream.Position)
            throw new Exception($"Under or orver write of the Bone part of the mesh asset! Pos: {stream.Position} | Expected: {currentStart + 0x40}");
    }

    public static Bone? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Bone bone, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(bone, Formatting.Indented, new CrcConverter()));
    }
}
