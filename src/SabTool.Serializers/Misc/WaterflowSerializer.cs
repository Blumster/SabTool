using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;

namespace SabTool.Serializers.Misc;

using SabTool.Data.Misc;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class WaterflowSerializer
{
    public static Waterflow DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII, true);

        if (!reader.CheckHeaderString("WF07", reversed: true))
            throw new Exception("Invalid WF07 header found!");

        var pointCount = reader.ReadInt32();

        var waterflow = new Waterflow();
        waterflow.Points.Capacity = pointCount;

        foreach (var pointIndex in Enumerable.Range(0, pointCount))
        {
            waterflow.Points.Add(new()
            {
                Name = new(reader.ReadUInt32()),
                Position = reader.ReadVector3(),
                Rotation = reader.ReadQuaternion(),
                Unknown = reader.ReadInt32()
            });
        }

        return waterflow;
    }

    public static void ExportGltf(Waterflow waterflow, string outputPath)
    {
        var scene = new SceneBuilder();
        var material = new MaterialBuilder();

        var modelCounter = 0;
        foreach (var point in waterflow.Points)
        {
            // Get name of point
            var waterflowPointName = point.Name.GetString();
            var waterflowPointFullName = $"{waterflowPointName}_{modelCounter}";

            // Create mesh and add arrow
            var mesh = new MeshBuilder<VertexPosition>(waterflowPointFullName);
            var primitive = mesh.UsePrimitive(material, 2);

            // Point arrow towards x+
            var pointTowardsPositiveXVector = MatrixMath.ConvertDxToOpenGl(new Vector3(20, 0, 0));
            primitive.AddLine(new VertexPosition(0, 0, 0), new VertexPosition(pointTowardsPositiveXVector.X, pointTowardsPositiveXVector.Y, pointTowardsPositiveXVector.Z));

            // Set matrix for point
            var dxTransform = Matrix4x4.CreateFromQuaternion(point.Rotation);
            dxTransform.Translation = new Vector3(point.Position.X, point.Position.Y, point.Position.Z);

            // Add mesh to scene
            scene.AddRigidMesh(mesh, MatrixMath.ConvertDxToOpenGl(dxTransform));
            modelCounter += 1;
        }

        var model = scene.ToGltf2();
        model.SaveGLTF(Path.Combine(outputPath, "waterflow.gltf"), new()
        {
            Validation = SharpGLTF.Validation.ValidationMode.Strict
        });
    }
}
