using System.Collections.Generic;
using System.Numerics;

namespace SabTool.Data.Graphics
{
    using Utils;

    public class Material
    {
        public static Dictionary<Crc, Material> Container { get; } = new();

        public int Id { get; set; }
        public byte Unk1 { get; set; }
        public byte TextureCount { get; set; }
        public Crc[] Textures { get; set; }
        public Crc PassNameCrc { get; set; }
        public uint Flags { get; set; }
        public RenderState[] RenderStates { get; set; }
        public TextureState[] TextureStates { get; set; }
        public Unk3 Unk3Val { get; set; }
        public int VertexShader { get; set; }
        public ShaderParameter VertexShaderParameters { get; set; }
        public int PixelShader { get; set; }
        public ShaderParameter PixelShaderParameters { get; set; }

        public List<Crc> Keys { get; set; } = new();

        #region Subclasses
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

        public class Pass
        {
            public Crc PassNameCrc { get; set; }
            public uint Flags { get; set; }
            public int RenderStateIndex { get; set; }
            public int TextureStateIndex { get; set; }
            public int Unk3Index { get; set; }
            public int PixelParameterIndex { get; set; }
            public int VertexParameterIndex { get; set; }
            public int PixelShaderUnk { get; set; }
            public int VertexShaderUnk { get; set; }
        }
        #endregion
    }
}
