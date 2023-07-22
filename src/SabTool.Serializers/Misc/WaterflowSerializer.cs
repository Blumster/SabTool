using System.Numerics;
using System.Text;

using SabTool.Data.Misc;
using SabTool.Utils;
using SabTool.Utils.Extensions;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;

namespace SabTool.Serializers.Misc;
public static class WaterflowSerializer
{
    public static Waterflow DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.ASCII, true);

        if (!reader.CheckHeaderString("WF07", reversed: true))
            throw new Exception("Invalid WF07 header found!");

        int pointCount = reader.ReadInt32();

        Waterflow waterflow = new();
        waterflow.Points.Capacity = pointCount;

        foreach (int pointIndex in Enumerable.Range(0, pointCount))
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
        SceneBuilder scene = new();
        MaterialBuilder material = new();

        int modelCounter = 0;
        foreach (WaterflowPoint? point in waterflow.Points)
        {
            // Get name of point
            string waterflowPointName = point.Name.GetString();
            string waterflowPointFullName = $"{waterflowPointName}_{modelCounter}";

            // Create mesh and add arrow
            MeshBuilder<VertexPosition> mesh = new(waterflowPointFullName);
            PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexEmpty, VertexEmpty> primitive = mesh.UsePrimitive(material, 2);

            // Point arrow towards x+
            Vector3 pointTowardsPositiveXVector = MatrixMath.ConvertDxToOpenGl(new Vector3(20, 0, 0));
            _ = primitive.AddLine(new VertexPosition(0, 0, 0), new VertexPosition(pointTowardsPositiveXVector.X, pointTowardsPositiveXVector.Y, pointTowardsPositiveXVector.Z));

            // Set matrix for point
            Matrix4x4 dxTransform = Matrix4x4.CreateFromQuaternion(point.Rotation);
            dxTransform.Translation = new Vector3(point.Position.X, point.Position.Y, point.Position.Z);

            // Add mesh to scene
            _ = scene.AddRigidMesh(mesh, MatrixMath.ConvertDxToOpenGl(dxTransform));
            modelCounter += 1;
        }

        SharpGLTF.Schema2.ModelRoot model = scene.ToGltf2();
        model.SaveGLTF(Path.Combine(outputPath, "waterflow.gltf"), new()
        {
            Validation = SharpGLTF.Validation.ValidationMode.Strict
        });
    }
}
