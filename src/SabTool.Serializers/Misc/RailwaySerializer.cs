using System.Numerics;
using System.Text;

using SabTool.Data.Misc;
using SabTool.Utils;
using SabTool.Utils.Extensions;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;

namespace SabTool.Serializers.Misc;

public class RailwaySerializer
{
    public static List<RailwaySpline> DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.ASCII, true);

        if (!reader.CheckHeaderString("RAI6", reversed: true))
        {
            throw new Exception("Invalid RAI6 header found!");
        }

        int splineCount = reader.ReadInt32();

        List<RailwaySpline> result = new()
        {
            Capacity = splineCount
        };


        foreach (int splineIndex in Enumerable.Range(0, splineCount))
        {
            RailwaySpline spline = new()
            {
                Name = reader.ReadPrefixedString(),
                NameCrc = new Crc(reader.ReadUInt32()),
                Distance = reader.ReadSingle()
            };
            int nodeCount = reader.ReadUInt16();

            foreach (int nodeIndex in Enumerable.Range(0, nodeCount))
            {
                RailwaySplineNode node = new()
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

                int attachmentCount = reader.ReadInt32();
                foreach (int attachmentIndex in Enumerable.Range(0, attachmentCount))
                {
                    RailwaySplineNodeAttachment attachment = new()
                    {
                        SplineNameCrc = new Crc(reader.ReadUInt32()),
                        NodeNameCrc = new Crc(reader.ReadUInt32())
                    };
                    node.Attachments.Add(attachment);
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

        foreach (RailwaySpline spline in railway)
        {
            string modelName = spline.Name;
            MeshBuilder<VertexPosition> mesh = new(modelName);
            PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexEmpty, VertexEmpty> primitive = mesh.UsePrimitive(material, 2);
            RailwaySplineNode? previousNode = null;
            foreach (RailwaySplineNode node in spline.Nodes)
            {
                if (previousNode != null)
                {
                    _ = primitive.AddLine(new VertexPosition(previousNode.Pos), new VertexPosition(node.Pos));
                }
                previousNode = node;
            }
            _ = scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        ModelRoot model = scene.ToGltf2();
        model.SaveGLTF(Path.Combine(outputPath, "railway.gltf"), new WriteSettings
        {
            Validation = SharpGLTF.Validation.ValidationMode.Strict
        });
    }
}
