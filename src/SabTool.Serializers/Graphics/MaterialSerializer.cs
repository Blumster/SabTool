using System.Numerics;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using SabTool.Data.Graphics;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Graphics;
public static class MaterialSerializer
{
    public static List<Material> DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream);

        if (!reader.CheckHeaderString("WSAO", reversed: true))
            throw new Exception("Invalid magic found!");

        int materialCapacity = reader.ReadInt32();
        int materialCount = reader.ReadInt32();
        int numRenderStates = reader.ReadInt32();
        int unk1 = reader.ReadInt32();
        int numTextureStates = reader.ReadInt32();
        int unk2 = reader.ReadInt32();
        int shaderParameters2Count = reader.ReadInt32();
        int unk3 = reader.ReadInt32();
        int unk4 = reader.ReadInt32();
        int unk5 = reader.ReadInt32();
        int shaderParameters1Count = reader.ReadInt32();
        int unk6 = reader.ReadInt32();
        int unk7 = reader.ReadInt32();
        int unk8 = reader.ReadInt32();
        int unk3Count = reader.ReadInt32();
        int unk9 = reader.ReadInt32();
        int numTextures = reader.ReadInt32();
        _ = reader.ReadInt32();
        int passCount = reader.ReadInt32();

        (Material.RenderState[][]? renderStates, Material.TextureState[][]? textureStates) = DeserializeStates(reader, numRenderStates, numTextureStates);
        Material.Unk3[] unk3Array = DeserializeUnk3Array(reader, unk3Count);
        Material.ShaderParameter[] shaderArray1 = DeserializeShaderParameters(reader, "WSPP", shaderParameters1Count);
        Material.ShaderParameter[] shaderArray2 = DeserializeShaderParameters(reader, "WSVP", shaderParameters2Count);
        Crc[] textures = DeserializeTextures(reader, numTextures);
        Pass[] passes = DeserializePassArray(reader, passCount);
        List<Material> materials = DeserializeMaterials(reader, materialCount, passes, textures, renderStates, textureStates, unk3Array, shaderArray2, shaderArray1);

        return reader.BaseStream.Position != reader.BaseStream.Length
            ? throw new Exception("Materials file wasn't properly read!")
            : materials;
    }

    private static (Material.RenderState[][], Material.TextureState[][]) DeserializeStates(BinaryReader reader, int numRenderStates, int numTextureStates)
    {
        if (!reader.CheckHeaderString("WSST", reversed: true))
            throw new Exception("Invalid magic found!");

        Material.RenderState[][] renderStates = new Material.RenderState[numRenderStates][];
        for (int i = 0; i < numRenderStates; ++i)
        {
            int count = reader.ReadInt32();

            renderStates[i] = new Material.RenderState[count];

            for (int j = 0; j < count; ++j)
            {
                renderStates[i][j] = new Material.RenderState
                {
                    Type = (Material.RenderStateType)reader.ReadInt32(),
                    Value = reader.ReadUInt32()
                };

                if (renderStates[i][j].Type == Material.RenderStateType.UnkC3)
                {
                    renderStates[i][j].Value = (uint)(250.0f / 16777215.0f);
                }
            }
        }

        Material.TextureState[][] textureStates = new Material.TextureState[numTextureStates][];
        for (int i = 0; i < numTextureStates; ++i)
        {
            int count = reader.ReadInt32();

            textureStates[i] = new Material.TextureState[count];

            for (int j = 0; j < count; ++j)
            {
                textureStates[i][j] = new Material.TextureState
                {
                    Type = (Material.TextureStateType)reader.ReadUInt32(),
                    Value = reader.ReadUInt32()
                };
            }
        }

        return (renderStates, textureStates);
    }

    private static Material.Unk3[] DeserializeUnk3Array(BinaryReader reader, int unkCount)
    {
        if (!reader.CheckHeaderString("WSCP", reversed: true))
            throw new Exception("Invalid magic found!");

        Material.Unk3[] unk3s = new Material.Unk3[unkCount];

        for (int i = 0; i < unkCount; ++i)
        {
            byte count = (byte)reader.ReadInt32();

            unk3s[i] = new Material.Unk3
            {
                Count = count,
                UnkArray = new byte[count],
                UnkVectors = new Vector4[count]
            };

            for (int j = 0; j < unk3s[i].Count; ++j)
            {
                unk3s[i].UnkArray[j] = (byte)reader.ReadInt32();
                unk3s[i].UnkVectors[j] = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
        }

        return unk3s;
    }

    private static Material.ShaderParameter[] DeserializeShaderParameters(BinaryReader reader, string magic, int shaderCount)
    {
        if (!reader.CheckHeaderString(magic, reversed: true))
            throw new Exception("Invalid magic found!");

        Material.ShaderParameter[] shaders = new Material.ShaderParameter[shaderCount];

        for (int i = 0; i < shaderCount; ++i)
        {
            byte vectorCount = (byte)reader.ReadInt32();

            shaders[i] = new Material.ShaderParameter
            {
                VectorCount = vectorCount,
                VectorParameters = new Vector4[vectorCount]
            };

            for (int j = 0; j < vectorCount; ++j)
            {
                shaders[i].VectorParameters[j] = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }

            byte intCount = (byte)reader.ReadInt32();

            shaders[i].IntCount = intCount;
            shaders[i].IntParameters = new int[intCount][];

            for (int j = 0; j < intCount; ++j)
            {
                shaders[i].IntParameters[j] = new int[4] { reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32() };
            }

            byte boolCount = (byte)reader.ReadInt32();

            shaders[i].BoolCount = boolCount;
            shaders[i].BoolParameters = new bool[boolCount];

            for (int j = 0; j < boolCount; ++j)
            {
                shaders[i].BoolParameters[j] = reader.ReadUInt32() != 0;
            }
        }

        return shaders;
    }

    private static Crc[] DeserializeTextures(BinaryReader reader, int textureCount)
    {
        return !reader.CheckHeaderString("WSTX", reversed: true)
            ? throw new Exception("Invalid magic found!")
            : reader.ReadConstArray(textureCount, r => new Crc(r.ReadUInt32()));
    }

    private static Pass[] DeserializePassArray(BinaryReader reader, int passCount)
    {
        if (!reader.CheckHeaderString("WSPA", reversed: true))
            throw new Exception("Invalid magic found!");

        Pass[] passes = new Pass[passCount];

        for (int i = 0; i < passCount; ++i)
        {
            passes[i] = new Pass(new(reader.ReadUInt32()))
            {
                Flags = (Material.MaterialFlags)reader.ReadUInt32(),
                RenderStateIndex = reader.ReadInt32(),
                TextureStateIndex = reader.ReadInt32(),
                Unk3Index = reader.ReadInt32(),
                PixelParameterIndex = reader.ReadInt32(),
                VertexParameterIndex = reader.ReadInt32(),
                PixelShader = new(reader.ReadUInt32()),
                VertexShader = new(reader.ReadUInt32())
            };
        }

        return passes;
    }

    private static List<Material> DeserializeMaterials(BinaryReader reader, int materialCount, Pass[] passes, Crc[] textures, Material.RenderState[][] renderStates, Material.TextureState[][] textureStates, Material.Unk3[] unk3s, Material.ShaderParameter[] vertexParameters, Material.ShaderParameter[] pixelParameters)
    {
        Material.Container.Clear();

        uint ZPassCrc = Hash.StringToHash("Z-Pass");
        uint CastShadowPassCrc = Hash.StringToHash("Cast-Shadow-Pass");
        uint ReceiveShadowPassCrc = Hash.StringToHash("Receive-Shadow-Pass");
        uint ReceiveShaderPassCrc = Hash.StringToHash("Receive-Shader-Pass");
        uint HairDepthPassCrc = Hash.StringToHash("Hair-Depth-Pass");
        uint HairSkyPassCrc = Hash.StringToHash("Hair-Sky-Pass");
        uint HairColorPassCrc = Hash.StringToHash("Hair-Color-Pass");
        uint DetailObjectPassCrc = Hash.StringToHash("DetailObject-Pass");

        if (!reader.CheckHeaderString("WSMA", reversed: true))
            throw new Exception("Invalid magic found!");

        List<Material> materials = new();

        for (int i = 0; i < materialCount; ++i)
        {
            Material material = new();

            Crc key = new(reader.ReadUInt32());

            int keyCount = reader.ReadInt32();
            if (keyCount > 0)
            {
                for (int j = 0; j < keyCount; ++j)
                {
                    Crc k = new(reader.ReadUInt32());

                    Material.Container.Add(k, material);

                    material.Keys.Add(k);
                }
            }
            else
            {
                Material.Container.Add(key, material);

                material.Keys.Add(key);
            }

            material.Id = reader.ReadInt32();
            material.TextureCount = (byte)reader.ReadInt32();

            int textureIndex = reader.ReadInt32();
            if (textureIndex != -1)
            {
                material.Textures = new Crc[material.TextureCount];
                for (int j = 0; j < material.TextureCount; ++j)
                    material.Textures[j] = textures[textureIndex + j];

                if (material.TextureCount == 1 && material.Textures[0] == new Crc(0x35AF3A0E))
                    material.Id |= 0x8000000;
            }

            int passIndex = reader.ReadInt32();
            if (passIndex != -1)
            {
                Pass pass = passes[passIndex];

                material.PassNameCrc = pass.PassNameCrc;
                material.Flags = pass.Flags | (Material.MaterialFlags)0x1000000;

                if (pass.RenderStateIndex != -1)
                    material.RenderStates = renderStates[pass.RenderStateIndex];

                if (pass.TextureStateIndex != -1)
                    material.TextureStates = textureStates[pass.TextureStateIndex];

                if (pass.Unk3Index != -1)
                    material.Unk3Val = unk3s[pass.Unk3Index];

                if (pass.VertexParameterIndex != -1)
                    material.VertexShaderParameters = vertexParameters[pass.VertexParameterIndex];

                if (pass.PixelParameterIndex != -1)
                    material.PixelShaderParameters = pixelParameters[pass.PixelParameterIndex];

                material.VertexShader = pass.VertexShader;
                material.PixelShader = pass.PixelShader;
            }

            if ((material.Flags & (Material.MaterialFlags)0x800) != 0)
            {
                // TODO
            }
            else if (material.PassNameCrc.Value != 0)
            {
                if (material.PassNameCrc.Value == ZPassCrc)
                {
                    material.Unk1 = 18;
                }
                else if (material.PassNameCrc.Value == CastShadowPassCrc)
                {
                    material.Unk1 = 2;
                }
                else
                {
                    material.Unk1 = material.PassNameCrc.Value == ReceiveShadowPassCrc || material.PassNameCrc.Value == ReceiveShaderPassCrc
                        ? (byte)28
                        : material.PassNameCrc.Value == HairDepthPassCrc
                                            ? (byte)76
                                            : material.PassNameCrc.Value == HairSkyPassCrc
                                                                ? (byte)77
                                                                : material.PassNameCrc.Value == HairColorPassCrc
                                                                                    ? (byte)75
                                                                                    : (material.Flags & Material.MaterialFlags.AlphaBlend) == 0 || (material.Flags & Material.MaterialFlags.Decal) != 0
                                                                                                        ? (byte)(material.PassNameCrc.Value != DetailObjectPassCrc ? 47 : 70)
                                                                                                        : (byte)((material.Flags & (Material.MaterialFlags)0x8000) != 0 ? 109 : 102);
                }
            }
            else
            {
                material.Unk1 = 47;
            }

            materials.Add(material);
        }

        return materials;
    }

    public static void SerializeRaw(List<Material> materials, Stream stream)
    {

    }

    public static List<Material> DeserialzieJSON(Stream stream)
    {
        List<Material> materials = new();

        return materials;
    }

    public static void SerializeJSON(List<Material> materials, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(materials, Formatting.Indented, new CrcConverter(), new StringEnumConverter()));
    }

    private class Pass
    {
        public Crc PassNameCrc { get; set; }
        public Material.MaterialFlags Flags { get; set; }
        public int RenderStateIndex { get; set; }
        public int TextureStateIndex { get; set; }
        public int Unk3Index { get; set; }
        public int PixelParameterIndex { get; set; }
        public int VertexParameterIndex { get; set; }
        public Crc PixelShader { get; set; } = new(0);
        public Crc VertexShader { get; set; } = new(0);

        public Pass(Crc passNameCrc)
        {
            PassNameCrc = passNameCrc;
        }
    }
}
