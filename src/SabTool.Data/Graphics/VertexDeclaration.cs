using System.Collections.Generic;
using System.Diagnostics;

namespace SabTool.Data.Graphics
{
    public class VertexDeclaration
    {
        public short StreamId { get; set; }
        public short Offset { get; set; }
        public VDType Type { get; set; }
        public VDMethod Method { get; set; }
        public VDUsage Usage { get; set; }
        public byte UsageIndex { get; set; }

        public VertexDeclaration()
        {
        }

        public VertexDeclaration(short streamId, short offset, VDType type, VDMethod method = VDMethod.Default, VDUsage usage = VDUsage.Position, byte usageIndex = 0)
        {
            StreamId = streamId;
            Offset = offset;
            Type = type;
            Method = method;
            Usage = usage;
            UsageIndex = usageIndex;
        }

        public override string ToString()
        {
            return $"VertexDeclaration({StreamId}, {Offset}, {Type}, {Method}, {Usage}, {UsageIndex})";
        }

        public static List<VertexDeclaration> Build(byte[] flags, int[] formats, int arrayCount, byte index)
        {
            var declarations = new List<VertexDeclaration>();

            if (arrayCount == 0)
            {
                declarations.Add(new(0xFF, 0, VDType.Unused));

                return declarations;
            }

            short streamId = 0;
            for (; streamId < arrayCount; ++streamId)
            {
                var format = formats[streamId];
                short offset = 0;

                switch (format & 3)
                {
                    case 1:
                        if (format >= 0)
                        {
                            declarations.Add(new(streamId, 0, VDType.Float2, VDMethod.Default, VDUsage.Position, 0));
                            offset = 8;
                        }
                        else
                        {
                            declarations.Add(new(streamId, 0, VDType.Short2, VDMethod.Default, VDUsage.BlendIndices, 0));
                            offset = 4;
                        }
                        break;

                    case 2:
                        if ((format & 0x1000000) != 0)
                        {
                            if (format >= 0)
                                declarations.Add(new(streamId, 0, VDType.Float16_4, VDMethod.Default, VDUsage.Position, 0));
                            else
                                declarations.Add(new(streamId, 0, VDType.Short4, VDMethod.Default, VDUsage.Position, 0));

                            offset = 8;
                        }
                        else
                        {
                            declarations.Add(new(streamId, 0, VDType.Float3, VDMethod.Default, VDUsage.Position, 0));
                            offset = 12;
                        }
                        break;

                    case 3:
                        if (format >= 0)
                        {
                            if ((format & 0x80000) != 0)
                                declarations.Add(new(streamId, 0, VDType.Float4, VDMethod.Default, VDUsage.PositionT, 0));
                            else
                                declarations.Add(new(streamId, 0, VDType.Float4, VDMethod.Default, VDUsage.Position, 0));

                            offset = 16;
                        }
                        else
                        {
                            declarations.Add(new(streamId, 0, VDType.Short4, VDMethod.Default, VDUsage.Position, 0));
                            offset = 8;
                        }
                        break;

                    default:
                        break;
                }

                if ((format & 4) != 0)
                {
                    declarations.Add(new(streamId, offset, VDType.UByte4N, VDMethod.Default, VDUsage.BlendWeight, 0));

                    offset += 4;

                    declarations.Add(new(streamId, offset, VDType.UByte4, VDMethod.Default, VDUsage.BlendIndices, 0));

                    offset += 4;
                }

                if ((format & 0x10) != 0)
                {
                    declarations.Add(new(streamId, offset, VDType.UByte4N, VDMethod.Default, VDUsage.Color, 0));

                    offset += 4;
                }

                if ((format & 0x20) != 0)
                {
                    declarations.Add(new(streamId, offset, VDType.UByte4N, VDMethod.Default, VDUsage.Color, 1));

                    offset += 4;
                }

                if ((format & 0xF00) != 0)
                {
                    while (true)
                    {
                        //Debugger.Break();
                        // TODO: unused?
                        break;
                    }

                    if ((format & 0x2000000) != 0)
                    {
                        declarations.Add(new(streamId, offset, VDType.Float16_2, VDMethod.Default, VDUsage.TexCoord, 0));

                        offset += 4;
                    }
                    else
                    {
                        declarations.Add(new(streamId, offset, VDType.Float2, VDMethod.Default, VDUsage.TexCoord, 0));

                        offset += 8;
                    }
                }

                if ((format & 0x4001000) == 0x4001000)
                {
                    declarations.Add(new(streamId, offset, VDType.Dec3N, VDMethod.Default, VDUsage.Normal, 0));

                    offset += 4;
                }
                else if ((format & 0x1000) == 0x1000)
                {
                    declarations.Add(new(streamId, offset, VDType.Float3, VDMethod.Default, VDUsage.Normal, 0));

                    offset += 12;
                }

                if ((format & 0x10002000) == 0x10002000)
                {
                    declarations.Add(new(streamId, offset, VDType.UByte4N, VDMethod.Default, VDUsage.Tangent, 0));

                    offset += 4;
                }
                else if ((format & 0x4002000) == 0x4002000)
                {
                    declarations.Add(new(streamId, offset, VDType.UByte4N, VDMethod.Default, VDUsage.Tangent, 0));

                    offset += 4;
                }
                else if ((format & 0x2000) == 0x2000)
                {
                    declarations.Add(new(streamId, offset, VDType.Float4, VDMethod.Default, VDUsage.Tangent, 0));

                    offset += 16;
                }

                if ((format & 0x10004000) == 0x10004000)
                {
                    declarations.Add(new(streamId, offset, VDType.Dec3N, VDMethod.Default, VDUsage.Binormal, 0));
                }
                else if ((format & 0x4000) != 0)
                {
                    declarations.Add(new(streamId, offset, VDType.Float3, VDMethod.Default, VDUsage.Binormal, 0));
                }
            }

            if (index == 1 && streamId == 1)
            {
                declarations.Add(new(1, 0, VDType.UByte4, VDMethod.Default, VDUsage.TexCoord, 5));
                declarations.Add(new(1, 4, VDType.UByte4, VDMethod.Default, VDUsage.TexCoord, 6));
                declarations.Add(new(2, 0, VDType.Short2, VDMethod.Default, VDUsage.BlendIndices, 1));
            }

            declarations.Add(new(0xFF, 0, VDType.Unused));

            return declarations;
        }
    }

    public enum VDType
    {
        Float1,
        Float2,
        Float3,
        Float4,
        D3DColor,
        UByte4,
        Short2,
        Short4,
        UByte4N,
        Short2N,
        Short4N,
        UShort2N,
        UShort4N,
        UDec3,
        Dec3N,
        Float16_2,
        Float16_4,
        Unused
    }

    public enum VDMethod
    {
        Default,
        PartialU,
        PartialV,
        CrossUV,
        UV,
        Lookup,
        LookupPresampled
    }

    public enum VDUsage
    {
        Position,
        BlendWeight,
        BlendIndices,
        Normal,
        PSize,
        TexCoord,
        Tangent,
        Binormal,
        TessFactor,
        PositionT,
        Color,
        Fog,
        Depth,
        Sample
    }
}
