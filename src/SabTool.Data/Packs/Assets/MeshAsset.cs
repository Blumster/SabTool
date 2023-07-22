using SabTool.Data.Graphics;

using SharpGLTF.Schema2;

namespace SabTool.Data.Packs.Assets;
public sealed class MeshAsset
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
        using (StreamWriter sb = new(new FileStream(Path.Combine(outputPath, $"{ModelName}-dump.txt"), FileMode.Create, FileAccess.Write, FileShare.Read)))
            sb.WriteLine(Model.DumpString());

        ModelRoot model = ModelRoot.CreateModel();
        Scene scene = model.UseScene(ModelName);

        SharpGLTF.Schema2.Material material = model.CreateMaterial("Default").WithDoubleSide(true);

        Skin skin = model.CreateSkin("Skeleton");

        Node skeleton = skin.Skeleton = scene.CreateNode("Skeleton");

        Skeleton skeletonData = Model.Mesh.Skeleton;

        Dictionary<short, Node> skeletonNodes = new()
        {
            { -1, skeleton }
        };

        for (short i = 0; i < skeletonData.NumBones; ++i)
        {
            short parentIndex = skeletonData.Indices[i];
            Node parent = skeletonNodes[parentIndex];

            Bone boneData = skeletonData.Bones[i];
            string boneName = boneData.Crc.GetString();

            skeletonNodes[i] = parent.CreateNode(string.IsNullOrEmpty(boneName) ? $"0x{boneData.Crc.Value:X8}" : boneName).WithLocalTransform(skeletonData.UnkBasePoses[i]);
        }

        Node[] joints = skeletonNodes.Where(p => p.Key != -1).Select(p => p.Value).ToArray();

        skin.BindJoints(joints);

        Node node = scene.CreateNode("Root");

        for (int i = 0; i < Model.Mesh.NumPrimitives; ++i)
        {
            Primitive prim = Model.Mesh.Primitives[i];

            Node primNode = node.CreateNode();

            SharpGLTF.Schema2.Mesh mesh = primNode.Mesh = model.CreateMesh();

            for (int j = 0; j < prim.VertexHolder.ArrayCount; ++j)
            {
                MeshPrimitive primitive = mesh.CreatePrimitive()
                    .WithVertexAccessor("POSITION", prim.GetVertexVector3(j, VDUsage.Position));

                IReadOnlyList<Vector3> normals = prim.GetVertexVector3(j, VDUsage.Normal);
                if (normals.Count > 0)
                    _ = primitive.WithVertexAccessor("NORMAL", normals);

                IReadOnlyList<Vector4> tangents = prim.GetVertexVector4(j, VDUsage.Tangent);
                if (tangents.Count > 0)
                    _ = primitive.WithVertexAccessor("TANGENT", tangents);

                IReadOnlyList<Vector2> texCoords = prim.GetVertexVector2(j, VDUsage.TexCoord);
                if (texCoords.Count > 0)
                    _ = primitive.WithVertexAccessor("TEXCOORD_0", texCoords);

                _ = primitive.WithIndicesAccessor(PrimitiveType.TRIANGLES, prim.GetIndices(j))
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
