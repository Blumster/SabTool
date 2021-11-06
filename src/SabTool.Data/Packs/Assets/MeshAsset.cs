using System;
using System.IO;
using System.Numerics;

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
        public Mesh Mesh { get; set; }

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
            Model = new Model();
            if (!Model.Read(reader))
                return false;

            Mesh = new Mesh();
            if (!Mesh.Read(reader))
                return false;

            if (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                Console.WriteLine($"Under read of the whole file of the mesh asset! Pos: {reader.BaseStream.Position} | Expected: {reader.BaseStream.Length}");
                return false;
            }

            Console.WriteLine();

            return true;
        }

        private bool ReadVertices(BinaryReader reader)
        {
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
