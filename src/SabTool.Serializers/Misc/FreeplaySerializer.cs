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
public static class FreeplaySerializer
{
    public static Freeplay DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.ASCII, true);

        if (!reader.CheckHeaderString("FP04", reversed: true))
            throw new Exception("Invalid FP04 header found!");

        int pointCount = reader.ReadInt32();

        Freeplay freeplay = new();
        freeplay.Points.Capacity = pointCount;

        // Always pointCount + 1
        if (reader.ReadInt32() != (pointCount + 1))
            throw new Exception("Not 0");

        foreach (int freeplayIndex in Enumerable.Range(0, pointCount))
        {
            FreeplayPoint point = new()
            {
                Unknown1 = reader.ReadUInt32(), // Maybe CRC, maybe not, TODO check for uniqueness in file
                Unknown2 = reader.ReadUInt32(), // Maybe CRC, maybe not, TODO check for uniqueness in file
                Crc = new(reader.ReadUInt32()),
                Unknown3 = reader.ReadInt32(),
                X = reader.ReadSingle(),
                Y = reader.ReadSingle(),
                Z = reader.ReadSingle()
            };

            int unknown4Count = reader.ReadInt32();

            foreach (int unknown4Index in Enumerable.Range(0, unknown4Count))
                point.Unknown4.Add(reader.ReadUInt32());

            freeplay.Points.Add(point);
        }

        return freeplay;
    }

    public static void ExportGltf(Freeplay freeplay, string outputPath)
    {
        SceneBuilder scene = new();
        MaterialBuilder material = new();

        int modelCounter = 0;

        foreach (FreeplayPoint? point in freeplay.Points)
        {
            string freeplayPointName = point.Crc.GetStringOrHexString();
            string freeplayPointFullName = $"{freeplayPointName}_{modelCounter}";

            MeshBuilder<VertexPosition> mesh = new(freeplayPointFullName);

            PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexEmpty, VertexEmpty> primitive = mesh.UsePrimitive(material, 1);
            _ = primitive.AddPoint(new VertexPosition(0, 0, 0));

            Matrix4x4 dxTransform = Matrix4x4.Identity;
            dxTransform.Translation = new Vector3(point.X, point.Y, point.Z);

            _ = scene.AddRigidMesh(mesh, MatrixMath.ConvertDxToOpenGl(dxTransform));
            modelCounter += 1;
        }

        SharpGLTF.Schema2.ModelRoot model = scene.ToGltf2();
        model.SaveGLTF(Path.Combine(outputPath, "freeplay.gltf"), new()
        {
            Validation = SharpGLTF.Validation.ValidationMode.Strict
        });
    }
}
