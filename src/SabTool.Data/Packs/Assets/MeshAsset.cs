using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SharpGLTF.Schema2;

namespace SabTool.Data.Packs.Assets;

using SabTool.Data.Graphics;
using SabTool.Utils;

public class MeshAsset
{
    public Crc Name { get; }
    public Model Model { get; set; }
    public string ModelName { get; set; }

    public void Import(string filePath)
    {
        throw new NotImplementedException();
    }

    public void Export(string outputPath)
    {
        using (var sb = new StreamWriter(new FileStream(Path.Combine(outputPath, $"{ModelName}-dump.txt"), FileMode.Create, FileAccess.Write, FileShare.Read)))
        {
            sb.WriteLine(Model.DumpString());
            sb.WriteLine(Model.Mesh.DumpString());
        }

        var model = ModelRoot.CreateModel();
        var scene = model.UseScene(ModelName);

        var material = model.CreateMaterial("Default").WithDoubleSide(true);

        var skin = model.CreateSkin("Skeleton");

        var skeleton = skin.Skeleton = scene.CreateNode("Skeleton");
        
        var skeletonData = Model.Mesh.Skeleton;

        var skeletonNodes = new Dictionary<short, Node>()
        {
            { -1, skeleton }
        };

        for (short i = 0; i < skeletonData.NumBones; ++i)
        {
            var parentIndex = skeletonData.Indices[i];
            var parent = skeletonNodes[parentIndex];

            var boneData = skeletonData.Bones[i];
            var boneName = boneData.Crc.GetString();

            skeletonNodes[i] = parent.CreateNode(string.IsNullOrEmpty(boneName) ? $"0x{boneData.Crc.Value:X8}" : boneName).WithLocalTransform(skeletonData.UnkBasePoses[i]);
        }

        var joints = skeletonNodes.Where(p => p.Key != -1).Select(p => p.Value).ToArray();

        skin.BindJoints(joints);

        var node = scene.CreateNode("Root");

        for (var i = 0; i < Model.Mesh.NumPrimitives; ++i)
        {
            var prim = Model.Mesh.Primitives[i];

            var primNode = node.CreateNode();
            
            var mesh = primNode.Mesh = model.CreateMesh();

            for (var j = 0; j < prim.VertexHolder.ArrayCount; ++j)
            {
                var primitive = mesh.CreatePrimitive()
                    .WithVertexAccessor("POSITION", prim.GetVertexVector3(j, VDUsage.Position));

                var normals = prim.GetVertexVector3(j, VDUsage.Normal);
                if (normals.Count > 0)
                    primitive.WithVertexAccessor("NORMAL", normals);

                var tangents = prim.GetVertexVector4(j, VDUsage.Tangent);
                if (tangents.Count > 0)
                    primitive.WithVertexAccessor("TANGENT", tangents);

                var texCoords = prim.GetVertexVector2(j, VDUsage.TexCoord);
                if (texCoords.Count > 0)
                    primitive.WithVertexAccessor("TEXCOORD_0", texCoords);

                primitive.WithIndicesAccessor(PrimitiveType.TRIANGLES, prim.GetIndices(j))
                    .WithMaterial(material);
            }
        }

        //model.SaveAsWavefront(Path.Combine(outputPath, $"{ModelName}.obj"));
        model.SaveGLTF(Path.Combine(outputPath, $"{ModelName}.gltf"), new WriteSettings
        {
            Validation = SharpGLTF.Validation.ValidationMode.TryFix,
            JsonIndented = true
        });
    }
}
