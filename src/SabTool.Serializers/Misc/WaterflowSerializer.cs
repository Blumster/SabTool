using System;
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

public static class WaterflowSerializer
{
    public static Waterflow DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.ASCII, true);

        if (!reader.CheckHeaderString("WF07", reversed: true))
        {
            throw new Exception("Invalid WF07 header found!");
        }

        int pointCount = reader.ReadInt32();

        Waterflow waterflow = new();
        waterflow.Points.Capacity = pointCount;

        foreach (int pointIndex in Enumerable.Range(0, pointCount))
        {
            WaterflowPoint point = new()
            {
                Name = new Crc(reader.ReadUInt32()),
                Position = reader.ReadVector3(),
                Rotation = reader.ReadQuaternion(),
                Unknown = reader.ReadInt32()
            };
            waterflow.Points.Add(point);
        }
        return waterflow;
    }

    public static void ExportGltf(Waterflow waterflow, string outputPath)
    {
        SceneBuilder scene = new();

        MaterialBuilder material = new MaterialBuilder()
            .WithDoubleSide(false);

        int modelCounter = 0;
        foreach (WaterflowPoint point in waterflow.Points)
        {
            // Get name of point
            string waterflowPointName = point.Name.GetString();
            string waterflowPointFullName = $"{waterflowPointName}_{modelCounter}";
            // Create mesh and add arrow
            MeshBuilder<VertexPosition> mesh = new(waterflowPointFullName);
            var primitive = mesh.UsePrimitive(material, 2);
            // Point arrow towards x+
            Vector3 pointTowardsPositiveXVector = MatrixMath.ConvertDxToOpenGl(new Vector3(20, 0, 0));
            primitive.AddLine(new VertexPosition(0, 0, 0), new VertexPosition(pointTowardsPositiveXVector.X, pointTowardsPositiveXVector.Y, pointTowardsPositiveXVector.Z));
            // Set matrix for point
            Matrix4x4 dxTransform = Matrix4x4.CreateFromQuaternion(point.Rotation);
            dxTransform.Translation = new Vector3(point.Position.X, point.Position.Y, point.Position.Z);
            // Add mesh to scene
            scene.AddRigidMesh(mesh, MatrixMath.ConvertDxToOpenGl(dxTransform));
            modelCounter += 1;
        }

        ModelRoot model = scene.ToGltf2();
        model.SaveGLTF(Path.Combine(outputPath, "waterflow.gltf"), new WriteSettings
        {
            Validation = SharpGLTF.Validation.ValidationMode.Strict
        });
    }
}
