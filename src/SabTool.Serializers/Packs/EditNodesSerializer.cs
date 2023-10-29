using System;

using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Packs;

using SabTool.Data.Entities;
using SabTool.Data.Packs;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class EditNodesSerializer
{
    public static EditNodes DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var nodes = new EditNodes();

        if (!reader.CheckHeaderString("DE00", reversed: true))
            throw new Exception("Invalid EditNodes header found!");

        var count = reader.ReadInt32();

        for (var i = 0; i < count; ++i)
        {
            var name = new Crc(reader.ReadUInt32());
            var size = reader.ReadInt32();
            var offset = reader.ReadInt32();

            var currentPosition = stream.Position;

            try
            {
                stream.Position = offset;

                var node = DeserializeEditNodeRaw(stream, name);

                //if (stream.Position != offset + size)
                    //throw new Exception($"Did not read the whole EditNode properly!");

                nodes.Nodes.Add(node);
            }
            finally
            {
                stream.Position = currentPosition;
            }
        }

        return nodes;
    }

    public static EditNode DeserializeEditNodeRaw(Stream stream, Crc name)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var editNode = new EditNode
        {
            Name = name
        };

        var version = reader.ReadUInt32();
        if (version != 0)
            throw new Exception($"Invalid EditNode version: {version}!");

        var objectCount = reader.ReadInt32();
        for (var i = 0; i < objectCount; ++i)
        {
            var editNodeObject = DeserializeEditNodeObject(reader, i);



            editNode.Objects.Add(editNodeObject.Name, editNodeObject);
        }

        return editNode;
    }

    private static EditNodeObject DeserializeEditNodeObject(BinaryReader reader, int entityId)
    {
        var obj = new EditNodeObject
        {
            Name = new(reader.ReadUInt32())
        };

        var propertyToOffset = new List<long>();

        Crc? className = null;
        Crc? type = null;
        Crc? collisionModule = null;
        string? script = null;
        string? parentName = null;
        var worldMatrix = new Matrix4x4();
        var height = 0.0f;
        var dynamicFenceFlags = 0;
        var dynamicFenceNodeCount = 0;
        var dynamicPathNodeCount = 0;
        var volumeCount = 0;
        var zoneTriggerRegionFlags = 0;
        AIFencePathNode[]? pathNodes = null;
        AIFencePost[]? posts = null;

        var propertyCount = reader.ReadInt32();
        var propertiesStart = reader.BaseStream.Position;

        for (var i = 0; i < propertyCount; ++i)
        {
            propertyToOffset.Add(reader.BaseStream.Position);

            var propertyType = new Crc(reader.ReadUInt32());
            var propertySize = reader.ReadInt32();

            var startOffset = reader.BaseStream.Position;

            switch (propertyType.Value)
            {
                case 0x5D78EDE2u: // ClassName
                    var classNameStr = reader.ReadStringWithMaxLength(256);

                    className = new Crc(Hash.StringToHash(classNameStr));
                    break;

                case 0xB1524043u: // CollisionModule
                    collisionModule = new(reader.ReadUInt32());
                    break;

                case 0xE85F4CFCu: // DynamicFenceFlags
                    dynamicFenceFlags = reader.ReadInt32();
                    break;

                case 0x1ACF73A2: // DynamicFenceNodes
                    dynamicFenceNodeCount = reader.ReadInt32();
                    break;

                case 0x6B9391D0u: // DynamicPathNodes
                    dynamicPathNodeCount = reader.ReadInt32();
                    break;

                case 0xE1AE04B8u: // Height
                    height = reader.ReadSingle();
                    break;

                case 0x79450675: // Parent
                    parentName = reader.ReadStringWithMaxLength(128);
                    if (parentName == "NONE")
                        parentName = null;

                    break;

                case 0x05D7FC60u: // Position
                    worldMatrix.M41 = reader.ReadSingle();
                    worldMatrix.M42 = reader.ReadSingle();
                    worldMatrix.M43 = reader.ReadSingle();
                    worldMatrix.M44 = 0.0f;
                    break;

                case 0x42498680: // Script
                    script = reader.ReadStringWithMaxLength(128);
                    break;

                case 0x28E10525: // Type
                    var typeStr = reader.ReadStringWithMaxLength(256);

                    type = new Crc(Hash.StringToHash(typeStr));
                    break;

                case 0x52856DBA: // Volumes
                    volumeCount = reader.ReadInt32();
                    break;

                case 0xFBB4BDA8u: // XAxis
                    worldMatrix.M11 = reader.ReadSingle();
                    worldMatrix.M12 = reader.ReadSingle();
                    worldMatrix.M13 = reader.ReadSingle();
                    worldMatrix.M14 = 0.0f;
                    break;

                case 0xD0C26E7Du: // YAxis
                    worldMatrix.M21 = reader.ReadSingle();
                    worldMatrix.M22 = reader.ReadSingle();
                    worldMatrix.M23 = reader.ReadSingle();
                    worldMatrix.M24 = 0.0f;
                    break;

                case 0x4FB76002u: // ZAxis
                    worldMatrix.M31 = reader.ReadSingle();
                    worldMatrix.M32 = reader.ReadSingle();
                    worldMatrix.M33 = reader.ReadSingle();
                    worldMatrix.M34 = 0.0f;
                    break;

                case 0x3CE51772u: // zone trigger region flags (actual CRC unknown)
                    zoneTriggerRegionFlags = reader.ReadInt32();
                    break;

                default:
                    Console.WriteLine($"Not handling {propertyType} right now...");

                    reader.BaseStream.Position = startOffset + propertySize;
                    continue;
            }

            if (startOffset + propertySize != reader.BaseStream.Position)
                throw new Exception($"Under or over reading property! Start: {startOffset} size: {propertySize} end: {reader.BaseStream.Position}");
        }

        var propertiesDataSize = reader.BaseStream.Position - propertiesStart;

        if (dynamicPathNodeCount > 0)
        {
            pathNodes = new AIFencePathNode[dynamicPathNodeCount];

            for (var i = 0; i < dynamicPathNodeCount; ++i)
            {
                var found = false;
                var nodeCrc = new Crc(Hash.StringToHash($"Node{i:D2}"));

                for (var p = 0; p < propertyCount; ++p)
                {
                    reader.BaseStream.Position = propertyToOffset[p];

                    var propertyType = new Crc(reader.ReadUInt32());
                    if (propertyType == nodeCrc)
                    {
                        found = true;

                        // Skip size
                        reader.BaseStream.Position += 4;

                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine($"Unable to find property with name: {nodeCrc}");
                    continue;
                }

                pathNodes[i] = new AIFencePathNode
                {
                    Offset = new(reader.ReadVector3(), 0.0f)
                };
            }
        }

        if (dynamicFenceNodeCount > 0)
        {
            posts = new AIFencePost[dynamicFenceNodeCount];

            for (var i = 0; i < dynamicPathNodeCount; ++i)
            {
                var found = false;
                var nodeCrc = new Crc(Hash.StringToHash($"FenceNode{i:D2}"));

                for (var p = 0; p < propertyCount; ++p)
                {
                    reader.BaseStream.Position = propertyToOffset[p];

                    var propertyType = new Crc(reader.ReadUInt32());
                    if (propertyType == nodeCrc)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine($"Unable to find property with name: {nodeCrc}");
                    continue;
                }

                var propertySize = reader.ReadInt32();
                var propertyStart = reader.BaseStream.Position;

                posts[i] = new AIFencePost
                {
                    FenceNodeOffset = new(reader.ReadVector3(), 0.0f)
                };

                if (propertyStart + propertySize != reader.BaseStream.Position)
                    throw new Exception($"Didn't ready the proper amount of bytes! Start: {propertyStart} size: {propertySize} end: {reader.BaseStream.Position}");

                found = false;
                nodeCrc = new Crc(Hash.StringToHash($"FenceNodeFlag{i:D2}"));

                for (var p = 0; p < propertyCount; ++p)
                {
                    reader.BaseStream.Position = propertyToOffset[p];

                    var propertyType = new Crc(reader.ReadUInt32());
                    if (propertyType == nodeCrc)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine($"Unable to find property with name: {nodeCrc}");
                    continue;
                }

                propertySize = reader.ReadInt32();
                propertyStart = reader.BaseStream.Position;

                posts[i].NodeFlags = reader.ReadUInt32();

                if (propertyStart + propertySize != reader.BaseStream.Position)
                    throw new Exception($"Didn't ready the proper amount of bytes! Start: {propertyStart} size: {propertySize} end: {reader.BaseStream.Position}");

                found = false;
                nodeCrc = new Crc(Hash.StringToHash($"FenceWallFlag{i:D2}"));

                for (var p = 0; p < propertyCount; ++p)
                {
                    reader.BaseStream.Position = propertyToOffset[p];

                    var propertyType = new Crc(reader.ReadUInt32());
                    if (propertyType == nodeCrc)
                    {
                        found = true;

                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine($"Unable to find property with name: {nodeCrc}");
                    continue;
                }

                propertySize = reader.ReadInt32();
                propertyStart = reader.BaseStream.Position;

                posts[i].WallFlags = reader.ReadUInt32();

                if (propertyStart + propertySize != reader.BaseStream.Position)
                    throw new Exception($"Didn't ready the proper amount of bytes! Start: {propertyStart} size: {propertySize} end: {reader.BaseStream.Position}");
            }
        }

        if (className is null)
            throw new Exception($"ClassName is null for {obj.Name}!");

        // TODO: skip? does this happen at all?
        if (className.Value == Hash.StringToHash("Saboteur"))
        {
            Console.WriteLine("Found Saboteur! Skipping...");
            return obj;
        }

        obj.Entity = new EntityDesc
        {
            Id = entityId,
            Name = obj.Name,
            FenceDesc = new AIFenceDesc()
            {
                Posts = posts,
                PathNodes = pathNodes,
                Flags = (byte)(dynamicFenceFlags & 0xFF)
            },
            WorldMatrix = worldMatrix
        };

        switch (className.Value)
        {
            case 0x0359D640u: // Occluder
            case 0x2E0750D5u: // AIRoad
            case 0x70B6908Du: // AISidewalk
            case 0x330B1105u: // Rope
            case 0xD359BD64u: // WTFPortal
            case 0xC3FA36EAu: // DetailBlock
            case 0x29CD42CEu: // TreeCluster
            case 0x2730D015u: // Module
                // PROPERTIES
                break;


            case 0x7D718595u: // Locator
                //   script is not null && script != "NONE" => LocatorScripted - PROPERTIES
                //   type == 0xDE850532 Garage => LocatorGarage - PROPERTIES
                //   else > Locator - PROPERTIES

            case 0xBEA1F68Bu: // Trigger
                //   type == 0xAF36A899 Polygon => TriggerRegion
                //   type == 0x976F2D35 RainTrigger => RainTrigger - TRIGGER_DATA
                //   type == 0x0A0AFF97 Zone => Zone - TRIGGER_DATA
                //   type == 0x9EE26816 SoundTrigger => SoundTrigger - TRIGGER_DATA
                // TRIGGER_DATA:
                // PROP("CustomExportTag"): (size, int, Crc, int, short)
                // PROP("Volumes"): (size, int volumeCount, for volumeCount: asd...)
                // type == 0x9EE26816 SoundTrigger => (crc, int, string, crc, int, crc, for 5: (int, int, int, int, int, string), int, int, int, int, int, for 3: (int, int, int))
                // type == 0xA0AFF97 Zone => PROP("WTFNode") => (for 2: (crc, rc, string, crc, crc, string), crc, crc, int, crc, crc, byte)
                // type == 0x976F2D35 RainTrigger => (crc, int, int)
                // WTF????
                break;

            case 0xC25641DFu: // Light
                break;
        }

        return obj;
    }

    public static void SerializeJSON(EditNodes editNodes, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(editNodes, Formatting.Indented, new CrcConverter(), new DictionaryConverter(), new DictionaryObjectConverter(), new PropertyConverter()));
    }
}
