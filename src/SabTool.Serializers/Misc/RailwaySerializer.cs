using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;

namespace SabTool.Serializers.Misc;

using SabTool.Data.Misc;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public class RailwaySerializer
{
    public static List<RailwaySpline> DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII, true);

        if (!reader.CheckHeaderString("RAI6", reversed: true))
            throw new Exception("Invalid RAI6 header found!");

        var splineCount = reader.ReadInt32();

        var result = new List<RailwaySpline>(splineCount);

        foreach (var splineIndex in Enumerable.Range(0, splineCount))
        {
            var spline = new RailwaySpline()
            {
                Name = reader.ReadPrefixedString(),
                NameCrc = new Crc(reader.ReadUInt32()),
                Distance = reader.ReadSingle()
            };

            // Store hashes
            Hash.FNV32string(spline.Name);
            Hash.StringToHash(spline.Name);

            int nodeCount = reader.ReadUInt16();

            foreach (var nodeIndex in Enumerable.Range(0, nodeCount))
            {
                var node = new RailwaySplineNode()
                {
                    Name = reader.ReadPrefixedString(),
                    NameCrc = new Crc(reader.ReadUInt32()),
                    StartPos = reader.ReadSingle(),
                    Distance = reader.ReadSingle(),
                    Pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    Influence = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    Tangent = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    IsStart = reader.ReadUInt16() == 1,
                    IsEnd = reader.ReadUInt16() == 1,
                    IsStation = reader.ReadUInt16() == 1,
                    MaxSpeed = reader.ReadSingle(),
                    StationWaitTime = reader.ReadSingle(),
                    TrainListBlueprintCrc = new Crc(reader.ReadUInt32())
                };

                // Store hashes
                Hash.FNV32string(node.Name);
                Hash.StringToHash(node.Name);

                var attachmentCount = reader.ReadInt32();
                foreach (var attachmentIndex in Enumerable.Range(0, attachmentCount))
                {
                    node.Attachments.Add(new RailwaySplineNodeAttachment()
                    {
                        SplineNameCrc = new Crc(reader.ReadUInt32()),
                        NodeNameCrc = new Crc(reader.ReadUInt32())
                    });
                }

                spline.Nodes.Add(node);
            }
            result.Add(spline);
        }
        return result;
    }

    public static void ExportGltfSplinePoints(List<RailwaySpline> railway, string outputPath)
    {
        SceneBuilder scene = new();

        MaterialBuilder material = new();

        foreach (var spline in railway)
        {
            var mesh = new MeshBuilder<VertexPosition>(spline.Name);
            var primitive = mesh.UsePrimitive(material, 2);

            RailwaySplineNode? previousNode = null;

            foreach (var node in spline.Nodes)
            {
                if (previousNode != null)
                    primitive.AddLine(new VertexPosition(previousNode.Pos), new VertexPosition(node.Pos));

                previousNode = node;
            }

            scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        var model = scene.ToGltf2();
        model.SaveGLTF(Path.Combine(outputPath, "railway.gltf"), new WriteSettings
        {
            Validation = SharpGLTF.Validation.ValidationMode.Strict
        });
    }
}
