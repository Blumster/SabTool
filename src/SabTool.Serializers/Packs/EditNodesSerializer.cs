using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Packs;

using SabTool.Data.Interfaces;
using SabTool.Data.Packs;
using SabTool.GameData;
using SabTool.Serializers.Json.Converters;
using SabTool.Serializers.Misc;
using SabTool.Utils;
using SabTool.Utils.Extensions;
using System.Linq;

public static class EditNodesSerializer
{
    public static EditNodes DeserializeRaw(Stream stream, IBlueprintDepot? depot)
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

                var node = DeserializeEditNodeRaw(stream, name, depot);

                if (stream.Position != offset + size)
                {
                    Console.WriteLine($"EditNode {node.Name} was not read properly! Start: {offset}, size:{size} , end: {stream.Position}, expectedEnd: {offset + size}");
                    //throw new Exception($"Did not read the whole EditNode properly!");
                }

                nodes.Nodes.Add(node);
            }
            finally
            {
                stream.Position = currentPosition;
            }
        }

        return nodes;
    }

    public static EditNode DeserializeEditNodeRaw(Stream stream, Crc name, IBlueprintDepot? depot = null)
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
            var editNodeObject = DeserializeEditNodeObject(reader, i, depot);

            if (editNode.Objects.ContainsKey(editNodeObject.Name))
                Console.WriteLine($"EditNode {editNode.Name} already contains object: {editNodeObject.Name}! Skipping...");
            else
                editNode.Objects.Add(editNodeObject.Name, editNodeObject);
        }

        return editNode;
    }

    private static EditNodeObject DeserializeEditNodeObject(BinaryReader reader, int entityId, IBlueprintDepot? depot = null)
    {
        var obj = new EditNodeObject
        {
            Id = entityId,
            Name = new(reader.ReadUInt32()),
        };

        var propertyToOffset = new List<long>();
        var lastPropertyOffset = 0L;

        
        var dynamicFenceFlags = 0;
        var dynamicFenceNodeCount = 0;
        var dynamicPathNodeCount = 0;
        var volumeCount = 0;
        var zoneTriggerRegionFlags = 0;

        var pathNodes = new Dictionary<int, AIFencePathNode>();
        var posts = new Dictionary<int, AIFencePost>();

        var propertyCount = reader.ReadInt32();

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

                    obj.ClassName = new Crc(Hash.StringToHash(classNameStr));
                    break;

                case 0xB1524043u: // CollisionModule
                    obj.CollisionModule = new(reader.ReadUInt32());
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
                    obj.Height = reader.ReadSingle();
                    break;

                case 0x79450675: // Parent
                    obj.ParentName = reader.ReadStringWithMaxLength(128);
                    if (obj.ParentName == "NONE")
                        obj.ParentName = null;

                    break;

                case 0x05D7FC60u: // Position
                    obj.WorldMatrix.M41 = reader.ReadSingle();
                    obj.WorldMatrix.M42 = reader.ReadSingle();
                    obj.WorldMatrix.M43 = reader.ReadSingle();
                    obj.WorldMatrix.M44 = 0.0f;
                    break;

                case 0x42498680: // Script
                    obj.Script = reader.ReadStringWithMaxLength(128);
                    break;

                case 0x28E10525: // Type
                    var typeStr = reader.ReadStringWithMaxLength(256);

                    obj.Type = new Crc(Hash.StringToHash(typeStr));
                    break;

                case 0x52856DBA: // Volumes
                    volumeCount = reader.ReadInt32();
                    break;

                case 0xFBB4BDA8u: // XAxis
                    obj.WorldMatrix.M11 = reader.ReadSingle();
                    obj.WorldMatrix.M12 = reader.ReadSingle();
                    obj.WorldMatrix.M13 = reader.ReadSingle();
                    obj.WorldMatrix.M14 = 0.0f;
                    break;

                case 0xD0C26E7Du: // YAxis
                    obj.WorldMatrix.M21 = reader.ReadSingle();
                    obj.WorldMatrix.M22 = reader.ReadSingle();
                    obj.WorldMatrix.M23 = reader.ReadSingle();
                    obj.WorldMatrix.M24 = 0.0f;
                    break;

                case 0x4FB76002u: // ZAxis
                    obj.WorldMatrix.M31 = reader.ReadSingle();
                    obj.WorldMatrix.M32 = reader.ReadSingle();
                    obj.WorldMatrix.M33 = reader.ReadSingle();
                    obj.WorldMatrix.M34 = 0.0f;
                    break;

                case 0x3CE51772u: // zone trigger region flags (actual CRC unknown)
                    zoneTriggerRegionFlags = reader.ReadInt32();
                    break;

                default:
                    if (IsWSDProperty(propertyType))
                    {
                        for (var nodeIndex = 0; nodeIndex < 100; ++nodeIndex)
                        {
                            if (propertyType.Value == Hash.StringToHash($"Node{nodeIndex:D2}"))
                            {
                                pathNodes.Add(nodeIndex, new AIFencePathNode
                                {
                                    Offset = new(reader.ReadVector3(), 0.0f)
                                });
                            }
                            else if (propertyType.Value == Hash.StringToHash($"FenceNode{nodeIndex:D2}"))
                            {
                                posts.Add(nodeIndex, new AIFencePost
                                {
                                    FenceNodeOffset = new(reader.ReadVector3(), 0.0f)
                                });
                            }
                            else if (propertyType.Value == Hash.StringToHash($"FenceNodeFlag{nodeIndex:D2}"))
                            {
                                if (!posts.ContainsKey(nodeIndex))
                                    throw new Exception($"Should not happen, flags before fence node?????");

                                posts[nodeIndex].NodeFlags = reader.ReadUInt32();
                            }
                            else if (propertyType.Value == Hash.StringToHash($"FenceWallFlag{nodeIndex:D2}"))
                            {
                                if (!posts.ContainsKey(nodeIndex))
                                    throw new Exception($"Should not happen, flags before fence node?????");

                                posts[nodeIndex].WallFlags = reader.ReadUInt32();
                            }
                        }
                    }/*
                    else
                    {
                        if (obj.ClassName is null)
                            throw new Exception($"This EditNode has no class...");

                        var bpType = BlueprintType.Unknown;

                        switch (obj.ClassName.Value)
                        {
                            case 0x0359D640u: // Occluder
                                bpType = BlueprintType.Occluder;
                                break;

                            case 0x2E0750D5u: // AIRoadNode
                                bpType = BlueprintType.AIRoad;
                                break;

                            case 0x70B6908Du: // AISidewalk
                                bpType = BlueprintType.AISidewalk;
                                break;

                            case 0x7D718595u: // Locator
                                if (obj.Script is var script && script != "NONE")
                                    bpType = BlueprintType.LocatorScripted;
                                else if (obj.Type is not null && obj.Type.Value == 0xDE850532u)
                                    bpType = BlueprintType.LocatorGarage;
                                else
                                    bpType = BlueprintType.Locator;
                                break;

                            case 0x330B1105u: // Rope
                                bpType = BlueprintType.Rope;
                                break;

                            case 0xD359BD64u: // WTFPortal
                                bpType = BlueprintType.WillToFightPortal;
                                break;

                            case 0xC3FA36EAu: // DetailBlock
                                break; // TODO: not real properties, but raw data

                            case 0x29CD42CEu: // TreeCluster
                                break; // TODO: not real properties, but raw data

                            case 0xBEA1F68Bu: // Trigger
                                if (obj.Type is null)
                                    throw new Exception($"Unknown Trigger type!");

                                switch (obj.Type.Value)
                                {
                                    case 0xAF36A899u: // Polygon
                                        //bpType = BlueprintType.TriggerPolygon;
                                        break;

                                    case 0x976F2D35u: // RainTrigger
                                        //bpType = BlueprintType.TriggerRain;
                                        break;

                                    case 0x0A0AFF97u: // Zone
                                        //bpType = BlueprintType.TriggerZone;
                                        break;

                                    case 0x9EE26816u: // SoundTrigger
                                        //bpType = BlueprintType.TriggerSound;
                                        break;
                                }
                                
                                break;

                            case 0x2730D015u: // Module
                                bpType = BlueprintType.Module;
                                break;

                            default:
                                if (depot is not null)
                                {
                                    var bp = depot.GetBlueprintByNameCrc(obj.ClassName) as Blueprint;
                                    if (bp is not null)
                                        bpType = bp.Type;
                                    else
                                        Console.WriteLine($"Maybe class name is a real class? ClassName: {obj.ClassName}");
                                }
                                break;
                        }

                        // HACK: skip triggers & ropes
                        if (obj.ClassName.Value != 0xBEA1F68Bu && obj.ClassName.Value != 0x330B1105u && obj.Type is not null)
                        {
                            if (bpType == BlueprintType.Unknown)
                                bpType = (BlueprintType)obj.Type.Value;
                            else if ((BlueprintType)obj.Type.Value != bpType)
                                throw new Exception($"WTF");
                        }

                        if (obj.ClassName.Value != 0xBEA1F68Bu && bpType == BlueprintType.Unknown)
                            Console.WriteLine("Blueprint data without class???");

                        if (bpType == BlueprintType.Unknown)
                        {
                            Console.WriteLine($"Unknown BPType for object {obj.Name}!");

                            obj.InstanceData.Add((propertyType, reader.ReadBytes(propertySize)));
                        }
                        else
                        {
                            var property = PropertySerializer.DeserializeWithNameAndSizeRaw(reader, propertyType, propertySize);

                            obj.InstanceData.Add((propertyType, BlueprintFieldTypes.ReadProperty(bpType, property)));
                        }
                    }*/

                    break;
            }

            if (startOffset + propertySize != reader.BaseStream.Position)
                throw new Exception($"Under or over reading property! Start: {startOffset} size: {propertySize} end: {reader.BaseStream.Position}");
        }

        obj.PathNodes = Enumerable.Range(0, dynamicPathNodeCount).Select(i => pathNodes.GetValueOrDefault(i)).ToArray();
        obj.Posts = Enumerable.Range(0, dynamicFenceNodeCount).Select(i => posts.GetValueOrDefault(i)).ToArray();

        if (obj.ClassName is null)
            throw new Exception($"ClassName is null for {obj.Name}!");

        // TODO: skip? does this happen at all?
        if (obj.ClassName.Value == Hash.StringToHash("Saboteur"))
        {
            reader.BaseStream.Position = lastPropertyOffset;

            Console.WriteLine("Found Saboteur! Skipping...");
            return obj;
        }

        switch (obj.ClassName.Value)
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

        //reader.BaseStream.Position = lastPropertyOffset;

        return obj;
    }

    private static bool IsWSDProperty(Crc property)
    {
        switch (property.Value)
        {
            case 0x5D78EDE2u: // ClassName
            case 0xB1524043u: // CollisionModule
            case 0xE85F4CFCu: // DynamicFenceFlags
            case 0x1ACF73A2: // DynamicFenceNodes
            case 0x6B9391D0u: // DynamicPathNodes
            case 0xE1AE04B8u: // Height
            case 0x79450675: // Parent
            case 0x05D7FC60u: // Position
            case 0x42498680: // Script
            case 0x28E10525: // Type
            case 0x52856DBA: // Volumes
            case 0xFBB4BDA8u: // XAxis
            case 0xD0C26E7Du: // YAxis
            case 0x4FB76002u: // ZAxis
            case 0x3CE51772u: // zone trigger region flags (actual CRC unknown)
                return true;

            default:
                break;
        }

        for (var i = 0; i < 100; ++i)
        {
            if (property.Value == Hash.StringToHash($"Node{i:D2}") ||
                property.Value == Hash.StringToHash($"FenceNode{i:D2}") ||
                property.Value == Hash.StringToHash($"FenceNodeFlag{i:D2}") ||
                property.Value == Hash.StringToHash($"FenceWallFlag{i:D2}"))
                return true;
        }

        return false;
    }

    public static void SerializeJSON(EditNodes editNodes, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(editNodes, Formatting.Indented, new CrcConverter(), new DictionaryConverter(), new DictionaryObjectConverter(), new PropertyConverter()));
    }
}
