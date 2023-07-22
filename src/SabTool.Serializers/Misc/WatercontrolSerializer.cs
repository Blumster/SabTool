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
public class WatercontrolSerializer
{
    public static Watercontrol DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.ASCII, true);

        if (!reader.CheckHeaderString("WC07", reversed: true))
        {
            throw new Exception("Invalid WC07 header found!");
        }

        int pointCount = reader.ReadInt32();

        Watercontrol watercontrol = new();

        foreach (int pointIndex in Enumerable.Range(0, pointCount))
        {
            WatercontrolPoint point = new()
            {
                Name = new Crc(reader.ReadUInt32()),
                X = reader.ReadSingle(),
                Y = reader.ReadSingle(),
                Z = reader.ReadSingle()
            };
            if (reader.ReadInt32() != 0)
            {
                throw new Exception("Not 0"); // Always 0, according to research a "blua" structure
            }
            watercontrol.Points.Add(point);
        }
        return watercontrol;
    }

    public static void ExportGltf(Watercontrol watercontrol, string outputPath)
    {
        SceneBuilder scene = new();

        MaterialBuilder material = new();

        int modelCounter = 0;
        foreach (WatercontrolPoint point in watercontrol.Points)
        {
            string watercontrolPointName = point.Name.GetStringOrHexString();
            string watercontrolPointFullName = $"{watercontrolPointName}_{modelCounter}"; // Names are not unique in list
            MeshBuilder<VertexPosition> mesh = new(watercontrolPointFullName);
            PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexEmpty, VertexEmpty> primitive = mesh.UsePrimitive(material, 1);
            _ = primitive.AddPoint(new VertexPosition(0, 0, 0));
            Matrix4x4 dxTransform = Matrix4x4.Identity;
            dxTransform.Translation = new Vector3(point.X, point.Y, point.Z);
            _ = scene.AddRigidMesh(mesh, MatrixMath.ConvertDxToOpenGl(dxTransform));
            modelCounter += 1;
        }

        ModelRoot model = scene.ToGltf2();
        model.SaveGLTF(Path.Combine(outputPath, "watercontrol.gltf"), new WriteSettings
        {
            Validation = SharpGLTF.Validation.ValidationMode.Strict
        });
    }
}
