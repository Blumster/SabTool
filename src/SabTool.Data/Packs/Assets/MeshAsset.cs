using System;
using System.Diagnostics;
using System.IO;

namespace SabTool.Data.Packs.Assets
{
    using Packs;
    using Utils;
    using Utils.Extensions;

    public class MeshAsset : IStreamBlockAsset
    {
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
            var name = reader.ReadStringFromCharArray(256);

            var headerData = reader.ReadDecompressedBytes(headerCompressedSize);
            using (var headerReader = new BinaryReader(new MemoryStream(headerData, false)))
            {
                if (!ReadHeader(headerReader))
                {
                    Console.WriteLine("Unable to read mesh header!");
                    return false;
                }
            }

            var vertexData = reader.ReadDecompressedBytes(vertexCompressedSize);
            using (var vertexReader = new BinaryReader(new MemoryStream(vertexData, false)))
            {
                if (!ReadVertices(vertexReader))
                {
                    Console.WriteLine("Unable to read mesh vertices!");
                    return false;
                }
            }

            return true;
        }

        private bool ReadHeader(BinaryReader reader)
        {
            // WSModel
            var currentStart = reader.BaseStream.Position;

            reader.BaseStream.Position += 0x4C;

            var model_vector3_4C = new Vector3(reader);
            var boxAndRadius = new Vector4(reader);
            var model_int_68 = reader.ReadUInt32();

            reader.BaseStream.Position += 0xC;

            var model_int_78 = reader.ReadUInt32();

            reader.BaseStream.Position += 0x18;

            var model_crc_94 = reader.ReadUInt32();
            var name = Hash.HashToString(model_crc_94);

            reader.BaseStream.Position += 0x18;

            var model_int_B0 = reader.ReadUInt32();

            reader.BaseStream.Position += 0x5;

            var model_byte_B9 = reader.ReadByte();

            reader.BaseStream.Position += 0x1;

            var model_byte_BB = reader.ReadByte();

            reader.BaseStream.Position += 0x3;

            var model_byte_BF = reader.ReadByte();

            if (currentStart + 0xC0 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the model part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0xC0}");
                return false;
            }

            // Win32Mesh
            // OdinMesh
            currentStart = reader.BaseStream.Position;

            reader.BaseStream.Position += 0xC;

            var numBones = reader.ReadUInt32();

            var mesh_int_10 = reader.ReadUInt32();
            var mesh_int_14 = reader.ReadUInt32();
            var unk4_count = reader.ReadUInt16();
            var primitive_count = reader.ReadUInt16();
            var unk3_count = reader.ReadUInt16();
            var mesh_short_1E = reader.ReadUInt16();

            reader.BaseStream.Position += 0x8;

            var numSegments = reader.ReadUInt16();
            var mesh_byte_2A = reader.ReadByte();
            var mesh_byte_2B = reader.ReadByte();

            // Win32Mesh
            var mesh_int_2C = reader.ReadUInt32();
            var mesh_byte_30 = reader.ReadByte();
            var mesh_byte_31 = reader.ReadByte();
            var mesh_byte_32 = reader.ReadByte();
            var mesh_byte_33 = reader.ReadByte();

            if (currentStart + 0x34 != reader.BaseStream.Position)
            {
                Console.WriteLine($"Under or orver read of the model part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x34}");
                return false;
            }

            if (numBones > 1)
            {
                Console.WriteLine("TODO: numBones > 1 is not yet supported!");
                return false;

                // PclSkeleton
                /*currentStart = reader.BaseStream.Position;

                var skele_int_0 = reader.ReadUInt32(); // some extra offset to skip
                var skele_int_4 = reader.ReadUInt32();
                var skele_int_8 = reader.ReadUInt32();
                var skele_int_C = reader.ReadUInt32(); // some count
                var skele_int_10 = reader.ReadUInt32();
                var skele_int_14 = reader.ReadUInt32();
                var skele_int_18 = reader.ReadUInt32();
                var skele_int_1C = reader.ReadUInt32();
                var skele_int_20 = reader.ReadUInt32();
                var skele_int_24 = reader.ReadUInt32();

                reader.BaseStream.Position += 0x4;

                // TODO

                if (currentStart + 0x2C != reader.BaseStream.Position)
                {
                    Console.WriteLine($"Under or orver read of the skeleton part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x2C}");
                    return false;
                }*/
            }

            currentStart = reader.BaseStream.Position;

            if (mesh_int_10 > 0)
            {
                // TODO
                reader.BaseStream.Position += 0x8;

                reader.BaseStream.Position += 0x44 * mesh_int_10;

                // TODO: validate position
            }

            currentStart = reader.BaseStream.Position;

            for (var i = 0; i < unk3_count; ++i)
            {
                // TODO
                reader.BaseStream.Position += 0x4;

                var someSkip = reader.ReadInt32();
                var someCount1 = reader.ReadInt32();
                var someCount2 = reader.ReadInt32();
                var someCount3 = reader.ReadInt32();

                reader.BaseStream.Position += someSkip + 0x18;
                reader.BaseStream.Position += someCount3 * 0x30;
                reader.BaseStream.Position += someCount1 * 0xC;
                reader.BaseStream.Position += someCount2 * 0x10;

                // TODO: validate position
            }

            for (var i = 0; i < unk4_count; ++i)
            {
                currentStart = reader.BaseStream.Position;

                reader.BaseStream.Position += 0x18;

                var vertexCounts = new int[4];
                for (var j = 0; j < 4; ++j)
                    vertexCounts[j] = reader.ReadInt32();

                var vertexFormats = new int[4];
                for (var j = 0; j < 4; ++j)
                    vertexFormats[j] = reader.ReadInt32();

                var vertexUVFormats = new long[4];
                for (var j = 0; j < 4; ++j)
                    vertexUVFormats[j] = reader.ReadInt64();

                var vertexArrayOffsets = new int[4];
                for (var j = 0; j < 4; ++j)
                    vertexArrayOffsets[j] = reader.ReadInt32();

                var vertexArraySizes = new int[4];
                for (var j = 0; j < 4; ++j)
                    vertexArraySizes[j] = reader.ReadInt32();

                var vertexSizes = new byte[4];
                for (var j = 0; j < 4; ++j)
                    vertexSizes[j] = reader.ReadByte();

                reader.BaseStream.Position += 0x4;

                var indexArrayOffset = reader.ReadInt32();
                var indexArraySize = reader.ReadInt32();
                var someFlags = reader.ReadUInt32();
                var vertexArrayCount = reader.ReadInt32();
                var indexCount = reader.ReadInt32();

                reader.BaseStream.Position += 0x4;

                int flags = 0x21;
                var flagBytes = BitConverter.GetBytes(flags);

                Console.WriteLine($"Vertex array count: {vertexArrayCount}");
                for (var j = 0; j < vertexArrayCount; ++j)
                {
                    Console.WriteLine($"Vertex array: (Off: {vertexArrayOffsets[j]} | Size: {vertexArraySizes[j]} | Count: {vertexCounts[j]} | VertexSize: {vertexSizes[j]} | Format: {vertexFormats[j]:X8} | UVFormat: {vertexUVFormats[j]:X})");
                }

                DumpVertexDeclaration(flagBytes, vertexFormats, vertexArrayCount, 0);
                DumpVertexDeclaration(flagBytes, vertexFormats, vertexArrayCount, 1);

                if (indexCount > 0)
                {
                    if (vertexArrayCount > 1)
                    {
                        Console.WriteLine("Index array: 32bit");
                    }
                    else
                    {
                        Console.WriteLine("Index array: 16bit");
                    }
                }

                if (currentStart + 0x98 != reader.BaseStream.Position)
                {
                    Console.WriteLine($"Under or orver read of the unk4 part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x98}");
                    return false;
                }
            }

            for (var i = 0; i < primitive_count; ++i)
            {
                currentStart = reader.BaseStream.Position;

                // Win32Primitive
                // OdinPrimitive

                reader.BaseStream.Position += 0x4;

                var unk3StructIndex = reader.ReadInt32();

                reader.BaseStream.Position += 0x28;

                // Win32Primitive
                var float_30 = reader.ReadSingle();
                var float_34 = reader.ReadSingle();
                var float_38 = reader.ReadSingle();
                var int_3C = reader.ReadInt32();
                var float_40 = reader.ReadSingle();
                var float_44 = reader.ReadSingle();
                var float_48 = reader.ReadSingle();
                var int_4C = reader.ReadInt32();
                var unk4StructIndex = reader.ReadInt32();
                var int_54 = reader.ReadInt32();
                var int_58 = reader.ReadInt32();
                var int_5C = reader.ReadInt32();
                var numIndices = reader.ReadInt32();

                if (currentStart + 0x64 != reader.BaseStream.Position)
                {
                    Console.WriteLine($"Under or orver read of the primitive part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x64}");
                    return false;
                }
            }

            for (var i = 0; i < numSegments; ++i)
            {
                currentStart = reader.BaseStream.Position;

                // OdinSegment
                var primitiveIndex = reader.ReadInt32();
                var materialCrc = reader.ReadUInt32();

                var materialName = Hash.HashToString(materialCrc);

                reader.BaseStream.Position += 0x6;

                var flags = reader.ReadInt16();

                if (currentStart + 0x10 != reader.BaseStream.Position)
                {
                    Console.WriteLine($"Under or orver read of the segment part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x10}");
                    return false;
                }
            }

            if (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                Console.WriteLine($"Under read of the whole file of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {reader.BaseStream.Length}");
                return false;
            }

            return true;
        }

        private bool ReadVertices(BinaryReader reader)
        {
            return true;
        }

        private void DumpVertexDeclaration(byte[] flags, int[] formats, int arrayCount, byte index)
        {
            if (arrayCount == 0)
            {
                DumpLine(0xFF, 0, "UNUSED");
                return;
            }

            var streamId = 0;
            for (;streamId < arrayCount; ++streamId)
            {
                var format = formats[streamId];
                var offset = 0;

                Console.WriteLine($"Vertex Decl for format 0x{format:X8}, stream {streamId}, index {index}");

                switch (format & 3)
                {
                    case 1:
                        if (format >= 0)
                        {
                            DumpLine(streamId, 0, "FLOAT2", "DEFAULT", "POSITION", 0);
                            offset = 8;
                        }
                        else
                        {
                            DumpLine(streamId, 0, "SHORT2", "DEFAULT", "BLENDINDICES", 0);
                            offset = 4;
                        }
                        break;

                    case 2:
                        if ((format & 0x1000000) != 0)
                        {
                            if (format >= 0)
                                DumpLine(streamId, 0, "FLOAT16_4", "DEFAULT", "POSITION", 0);
                            else
                                DumpLine(streamId, 0, "SHORT4", "DEFAULT", "POSITION", 0);

                            offset = 8;
                        }
                        else
                        {
                            DumpLine(streamId, 0, "FLOAT3", "DEFAULT", "POSITION", 0);
                            offset = 12;
                        }
                        break;

                    case 3:
                        if (format >= 0)
                        {
                            if ((format & 0x80000) != 0)
                                DumpLine(streamId, 0, "FLOAT4", "DEFAULT", "POSITIONT", 0);
                            else
                                DumpLine(streamId, 0, "FLOAT4", "DEFAULT", "POSITION", 0);

                            offset = 16;
                        }
                        else
                        {
                            DumpLine(streamId, 0, "SHORT4", "DEFAULT", "POSITION", 0);
                            offset = 8;
                        }
                        break;

                    default:
                        break;
                }

                if ((format & 4) != 0)
                {
                    DumpLine(streamId, offset, "UBYTE4N", "DEFAULT", "BLENDWEIGHT", 0);

                    offset += 4;

                    DumpLine(streamId, offset, "UBYTE4", "DEFAULT", "BLENDINDICES", 0);

                    offset += 4;
                }

                if ((format & 0x10) != 0)
                {
                    DumpLine(streamId, offset, "UBYTE4N", "DEFAULT", "COLOR", 0);

                    offset += 4;
                }

                if ((format & 0x20) != 0)
                {
                    DumpLine(streamId, offset, "UBYTE4N", "DEFAULT", "COLOR", 1);

                    offset += 4;
                }

                if ((format & 0xF00) != 0)
                {
                    while (true)
                    {
                        // TODO: unused?
                        break;
                    }

                    if ((format & 0x2000000) != 0)
                    {
                        DumpLine(streamId, offset, "FLOAT16_2", "DEFAULT", "TEXCOORD", 0);

                        offset += 4;
                    }
                    else
                    {
                        DumpLine(streamId, offset, "FLOAT2", "DEFAULT", "TEXCOORD", 0);

                        offset += 8;
                    }
                }

                if ((format & 0x4001000) == 0x4001000)
                {
                    DumpLine(streamId, offset, "DEC3N", "DEFAULT", "NORMAL", 0);

                    offset += 4;
                }
                else if ((format & 0x1000) == 0x1000)
                {
                    DumpLine(streamId, offset, "FLOAT3", "DEFAULT", "NORMAL", 0);

                    offset += 12;
                }

                if ((format & 0x10002000) == 0x10002000)
                {
                    DumpLine(streamId, offset, "UBYTE4N", "DEFAULT", "TANGENT", 0);

                    offset += 4;
                }
                else if ((format & 0x4002000) == 0x4002000)
                {
                    DumpLine(streamId, offset, "UBYTE4N", "DEFAULT", "TANGENT", 0);

                    offset += 4;
                }
                else if ((format & 0x2000) == 0x2000)
                {
                    DumpLine(streamId, offset, "FLOAT4", "DEFAULT", "TANGENT", 0);

                    offset += 16;
                }

                if ((format & 0x10004000) == 0x10004000)
                {
                    DumpLine(streamId, offset, "DEC3N", "DEFAULT", "BINORMAL", 0);
                }
                else if ((format & 0x4000) != 0)
                {
                    DumpLine(streamId, offset, "FLOAT3", "DEFAULT", "BINORMAL", 0);
                }
            }

            if (index == 1 && streamId == 1)
            {
                DumpLine(1, 0, "UBYTE4", "DEFAULT", "TEXCOORD", 5);
                DumpLine(1, 4, "UBYTE4", "DEFAULT", "TEXCOORD", 6);
                DumpLine(2, 0, "SHORT2", "DEFAULT", "BLENDINDICES", 1);
            }

            DumpLine(0xFF, 0, "UNUSED");

            Console.WriteLine();
        }

        private void DumpLine(int stream, int offset, string type, string method = "", string usage = "", byte usageIndex = 0) => Console.WriteLine($"Element({stream}, {offset}, {type}, {method}, {usage}, {usageIndex})");

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
        }
    }
}
