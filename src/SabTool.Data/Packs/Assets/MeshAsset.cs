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

            Console.WriteLine($"Reading mesh {ModelName}...");

            var headerData = reader.ReadDecompressedBytes(headerCompressedSize);
            using (var headerReader = new BinaryReader(new MemoryStream(headerData, false)))
            {
                Model = new Model();
                if (!Model.Read(headerReader))
                    return false;

                Mesh = new Mesh();
                if (!Mesh.ReadHeader(headerReader))
                    return false;

                if (headerReader.BaseStream.Position != headerReader.BaseStream.Length)
                {
                    Console.WriteLine($"Under read of the header data of the mesh asset! Pos: {headerReader.BaseStream.Position} | Expected: {headerReader.BaseStream.Length}");
                    return false;
                }
            }

            var vertexData = reader.ReadDecompressedBytes(vertexCompressedSize);
            using (var vertexReader = new BinaryReader(new MemoryStream(vertexData, false)))
            {
                if (!Mesh.ReadVertices(vertexReader))
                {
                    Console.WriteLine("Unable to read Vertex data based on the Mesh header!");
                    return false;
                }

                if (vertexReader.BaseStream.Position != vertexReader.BaseStream.Length)
                {
                    Console.WriteLine($"Under read of the vertex data of the mesh asset! Pos: {vertexReader.BaseStream.Position} | Expected: {vertexReader.BaseStream.Length}");
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
