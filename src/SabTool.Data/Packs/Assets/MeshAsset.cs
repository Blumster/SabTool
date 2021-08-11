using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var mesh_int_18 = reader.ReadUInt16();
            var mesh_int_1A = reader.ReadUInt16();
            var mesh_int_1C = reader.ReadUInt16();
            var mesh_int_1E = reader.ReadUInt16();

            reader.BaseStream.Position += 0x8;

            var mesh_int_28 = reader.ReadUInt16();
            var mesh_int_2A = reader.ReadByte();
            var mesh_int_2B = reader.ReadByte();

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
                // PclSkeleton
                currentStart = reader.BaseStream.Position;

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
                    Console.WriteLine($"Under or orver read of the model part of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {currentStart + 0x2C}");
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
        }
    }
}
