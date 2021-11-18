using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SharpGLTF.Schema2;

namespace SabTool.Data.Packs.Assets
{
    using Graphics;
    using Packs;
    using Utils;
    using Utils.Extensions;

    public class MeshAsset : IStreamBlockAsset
    {
        public Crc Name { get; }
        public Model Model { get; set; }
        public string ModelName { get; set; }

        public MeshAsset(Crc name)
        {
            Name = name;
        }

        public bool Read(MemoryStream data)
        {
            using var reader = new BinaryReader(data);

            if (!reader.CheckHeaderString("MSHA", reversed: true))
            {
                Console.WriteLine("Invalid mesh header string!");
                return false;
            }

            var headerRealSize = reader.ReadInt32();
            var vertexRealSize = reader.ReadInt32();
            var headerCompressedSize = reader.ReadInt32();
            var vertexCompressedSize = reader.ReadInt32();
            ModelName = reader.ReadStringFromCharArray(256);

            var hash = Hash.StringToHash(ModelName);

            Console.WriteLine($"Reading mesh {ModelName}...");

            using (var modelStream = new MemoryStream(reader.ReadDecompressedBytes(headerCompressedSize), false))
            {
                // TODO
                //Model = ModelSerializer.DeserializeRaw(modelStream);

                if (modelStream.Position != modelStream.Length)
                {
                    Console.WriteLine($"Under read of the header data of the mesh asset! Pos: {modelStream.Position} | Expected: {modelStream.Length}");
                    return false;
                }
            }

            using (var vertexStream = new MemoryStream(reader.ReadDecompressedBytes(vertexCompressedSize), false))
            {
                // TODO
                //MeshSerializer.DeserializeVerticesRaw(Model.Mesh, vertexStream);

                if (vertexStream.Position != vertexStream.Length)
                {
                    Console.WriteLine($"Under read of the vertex data of the mesh asset! Pos: {vertexStream.Position} | Expected: {vertexStream.Length}");
                    return false;
                }
            }

            return true;
        }

        public bool Write(MemoryStream writer)
        {
            throw new NotImplementedException();
        }

        public void Import(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Export(string outputPath)
        {
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
}
