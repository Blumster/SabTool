using System;
using System.Collections.Generic;
using System.Numerics;

namespace SabTool.Data.Graphics;

using SabTool.Utils;

public class Material
{
    public static Dictionary<Crc, Material> Container { get; } = new();

    public int Id { get; set; }
    public byte Unk1 { get; set; }
    public byte TextureCount { get; set; }
    public Crc[] Textures { get; set; }
    public Crc PassNameCrc { get; set; }
    public MaterialFlags Flags { get; set; }
    public RenderState[] RenderStates { get; set; }
    public TextureState[] TextureStates { get; set; }
    public Unk3 Unk3Val { get; set; }
    public Crc VertexShader { get; set; }
    public ShaderParameter VertexShaderParameters { get; set; }
    public Crc PixelShader { get; set; }
    public ShaderParameter PixelShaderParameters { get; set; }

    public List<Crc> Keys { get; set; } = new();

    #region Subclasses
    [Flags]
    public enum MaterialFlags : uint
    {
        AlphaBlend = 0x01,
        AplhaTest = 0x02,
        Decal = 0x04,
        PostProcess = 0x08,
        Skinned = 0x10,
        ShadowPass = 0x20,
        ColorIndex1Reserved = 0x40,
        ColorIndexReserved = 0x80,
        NoShadow = 0x100,
    }

    public enum RenderStateType
    {
        UnkC3 = 0xC3
    }

    public class RenderState
    {
        public RenderStateType Type { get; set; }
        public uint Value { get; set; }
    }

    public enum TextureStateType
    {
        NextStage = 0x7FFFFFFF,
    }

    public class TextureState
    {
        public TextureStateType Type { get; set; }
        public uint Value { get; set; }
    }

    public class Unk3
    {
        public byte Count { get; set; }
        public byte[] UnkArray { get; set; }
        public Vector4[] UnkVectors { get; set; }
    }

    public class ShaderParameter
    {
        public byte VectorCount { get; set; }
        public byte IntCount { get; set; }
        public byte BoolCount { get; set; }
        public Vector4[] VectorParameters { get; set; }
        public int[][] IntParameters { get; set; }
        public bool[] BoolParameters { get; set; }
    }
    #endregion
}
