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

public static class FreeplaySerializer
{
    public static Freeplay DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII, true);

        if (!reader.CheckHeaderString("FP04", reversed: true))
            throw new Exception("Invalid FP04 header found!");

        var pointCount = reader.ReadInt32();

        var freeplay = new Freeplay();
        freeplay.Points.Capacity = pointCount;

        // Always pointCount + 1
        if (reader.ReadInt32() != (pointCount + 1))
            throw new Exception("Not 0");

        foreach (var freeplayIndex in Enumerable.Range(0, pointCount))
        {
            var point = new FreeplayPoint()
            {
                Unknown1 = reader.ReadUInt32(), // Maybe CRC, maybe not, TODO check for uniqueness in file
                Unknown2 = reader.ReadUInt32(), // Maybe CRC, maybe not, TODO check for uniqueness in file
                Crc = new(reader.ReadUInt32()),
                Unknown3 = reader.ReadInt32(),
                X = reader.ReadSingle(),
                Y = reader.ReadSingle(),
                Z = reader.ReadSingle()
            };

            var unknown4Count = reader.ReadInt32();

            foreach (var unknown4Index in Enumerable.Range(0, unknown4Count))
                point.Unknown4.Add(reader.ReadUInt32());

            freeplay.Points.Add(point);
        }

        return freeplay;
    }

    public static void ExportGltf(Freeplay freeplay, string outputPath)
    {
        var scene = new SceneBuilder();
        var material = new MaterialBuilder();

        var modelCounter = 0;

        foreach (var point in freeplay.Points)
        {
            var freeplayPointName = point.Crc.GetStringOrHexString();
            var freeplayPointFullName = $"{freeplayPointName}_{modelCounter}";

            var mesh = new MeshBuilder<VertexPosition>(freeplayPointFullName);

            var primitive = mesh.UsePrimitive(material, 1);
            primitive.AddPoint(new VertexPosition(0, 0, 0));

            var dxTransform = Matrix4x4.Identity;
            dxTransform.Translation = new Vector3(point.X, point.Y, point.Z);

            scene.AddRigidMesh(mesh, MatrixMath.ConvertDxToOpenGl(dxTransform));
            modelCounter += 1;
        }

        var model = scene.ToGltf2();
        model.SaveGLTF(Path.Combine(outputPath, "freeplay.gltf"), new()
        {
            Validation = SharpGLTF.Validation.ValidationMode.Strict
        });
    }
}
