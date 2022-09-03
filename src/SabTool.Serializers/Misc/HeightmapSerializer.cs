using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using Newtonsoft.Json;
using SharpGLTF.Schema2;

namespace SabTool.Serializers.Misc;

using SabTool.Data.Misc;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class HeightmapSerializer
{
    public static Heightmap DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("HEI5", reversed: true))
            throw new Exception("Invalid HEI5 header found!");

        int cellCount = reader.ReadInt32();

        Heightmap heightmap = new()
        {
            CellCountMaxX = reader.ReadInt32(),
            CellCountMaxY = reader.ReadInt32(),
            CellSize = reader.ReadSingle(),
            MinX = reader.ReadSingle(),
            MinY = reader.ReadSingle(),
            MinZ = reader.ReadSingle()
        };
        // Reserve capacity for entries
        heightmap.Cells.Capacity = cellCount;

        foreach (int cellIndex in Enumerable.Range(0, cellCount))
        {
            heightmap.Cells.Add(ReadHeightmapCell(reader));
        }

        if (stream.Position != stream.Length)
        {
            throw new Exception("Under reading HEI5 file!");
        }
        return heightmap;
    }

    public static HeightmapCellData ReadHeightmapCellData(BinaryReader reader)
    {
        if (!reader.CheckHeaderString("HEI1", reversed: true))
            throw new Exception("Invalid HEI1 header found!");

        HeightmapCellData hei1 = new()
        {
            PointCountX = reader.ReadInt32(),
            PointCountY = reader.ReadInt32(),
            HeightRangeMax = reader.ReadSingle(),
            HeightRangeMin = reader.ReadSingle()
        };
        hei1.PointData = reader.ReadBytes(hei1.PointCountX * hei1.PointCountY);
        return hei1;
    }

    public static HeightmapCell ReadHeightmapCell(BinaryReader reader)
    {
        if (!reader.CheckHeaderString("HEI1", reversed: true))
        {
            throw new Exception("Invalid HEI1 header found!");
        }
        HeightmapCell cell = new()
        {
            Data = ReadHeightmapCellData(reader)
        };
        cell.MinX = reader.ReadSingle();
        if (reader.ReadSingle() != HeightmapCell.MinY)
        {
            throw new Exception("Unexpected MinY");
        }
        cell.MinZ = reader.ReadSingle();
        cell.MaxX = reader.ReadSingle();
        if (reader.ReadSingle() != HeightmapCell.MaxY)
        {
            throw new Exception("Unexpected MaxY");
        }
        cell.MaxZ = reader.ReadSingle();
        return cell;
    }

    private static void ExportHightmapCellData(Dictionary<int, HeightmapCellData> packDictionary, Dictionary<int, string> isFranceChunkDictionary, Node node, ModelRoot model, SharpGLTF.Schema2.Material material)
    {
        int modelCounter = 0;

        List<KeyValuePair<int, HeightmapCellData>> orderedEntries = packDictionary.ToList();
        orderedEntries = orderedEntries.OrderBy(x => (((UInt32)x.Key) << 16) >> 16).ToList();
        foreach (KeyValuePair<int, HeightmapCellData> entry in orderedEntries)
        {
            HeightmapCellData cellData = entry.Value;
            string highByte = (((UInt32)entry.Key) >> 16).ToString();
            string realNumber = ((((UInt32)entry.Key) << 16) >> 16).ToString().PadLeft(6, '0').Substring(1);
            string cellName = $"HCell_{realNumber}_{highByte}_{(entry.Key << 8) + 1}.pack{(isFranceChunkDictionary[entry.Key].Length == 0 ? string.Empty : "_France_Chunk" + isFranceChunkDictionary[entry.Key])}";
            UInt32 originalValue = (((UInt32)entry.Key) << 8) + 1;
            // Get values (original Value is Uint32)
            int x_block = ((int)originalValue & 0x03FE0000) >> 17;
            int z_block = ((int)originalValue & 0x0001FF00) >> 8;
            // Check sign bits
            if ((x_block & 0x00000100) != 0)
            {
                x_block |= -512 /*0xFFFFFE00*/;
            }
            if ((z_block & 0x00000100) != 0)
            {
                z_block |= -512;
            }
            // Get resolution
            int resolution = (int)originalValue & 0x000000FF;
            var primNode = node.CreateNode(cellName);

            var mesh = primNode.Mesh = model.CreateMesh();

            // Get coordinates
            List<Vector3> coordinates = new();
            coordinates.Capacity = 10 * 10; // Always contains 10*10 coordinates
            foreach (int y_entry in Enumerable.Range(0, cellData.PointCountY))
            {
                foreach (int x_entry in Enumerable.Range(0, cellData.PointCountX))
                {
                    float x = (x_block * 60.0f) + (x_entry * (60.0f / 9));
                    float y = Lerp(cellData.HeightRangeMin, cellData.HeightRangeMax, cellData.PointData[y_entry * cellData.PointCountY + x_entry] / 255.0f);
                    float z = (z_block * 60.0f) + (y_entry * (60.0f / 9));
                    coordinates.Add(MatrixMath.ConvertDxToOpenGl(new Vector3(x, y, z)));
                }
            }
            // Get indices
            List<int> triangleIndices = new()
            {
                Capacity = 81 * 2 // 9*9 for each quad * 2 for triangles
            };
            foreach (int y in Enumerable.Range(0, 9))
            {
                foreach (int x in Enumerable.Range(0, 9))
                {
                    int firstVertexInFace = (10 * y) + x;
                    // First triangle of quad
                    triangleIndices.Add(firstVertexInFace + 1);
                    triangleIndices.Add(firstVertexInFace);
                    triangleIndices.Add(firstVertexInFace + 10);
                    // Second triangle of quad
                    triangleIndices.Add(firstVertexInFace + 10);
                    triangleIndices.Add(firstVertexInFace + 11);
                    triangleIndices.Add(firstVertexInFace + 1);
                }
            }
            var primitive = mesh.CreatePrimitive()
                .WithVertexAccessor("POSITION", coordinates);
            primitive.WithIndicesAccessor(PrimitiveType.TRIANGLES, triangleIndices)
                .WithMaterial(material);
            modelCounter += 1;
        }
    }

    public static void SerializeRaw(Heightmap heightmap, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
    }

    public static Heightmap? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(Heightmap heightmap, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(heightmap, Formatting.Indented));
    }

    public static void ExportPly(Heightmap heightmap, Stream stream)
    {
        CultureInfo enCi = CultureInfo.GetCultureInfo("en-US");
        using StreamWriter writer = new(stream, Encoding.ASCII);
        // Write ply header
        writer.Write("ply\nformat ascii 1.0\nelement vertex ");
        writer.Write($"{(heightmap.Cells.Count * 10 * 10).ToString(enCi)}\n");  // each Cell is always 10 * 10
        writer.Write("property float x\nproperty float y\nproperty float z\nelement face ");
        writer.Write($"{(heightmap.Cells.Count * 9 * 9).ToString(enCi)}\n");
        writer.Write("property list uchar int vertex_index\nend_header\n");
        // Write vertex data
        foreach (HeightmapCell cell in heightmap.Cells)
        {
            foreach (int y_entry in Enumerable.Range(0, cell.Data.PointCountY))
            {
                foreach (int x_entry in Enumerable.Range(0, cell.Data.PointCountX))
                {
                    // x / y / z in this block are in Blender coordinates where z is height instead of y and x is -x
                    float x = cell.MinX + (x_entry * (heightmap.CellSize / 9));
                    x *= -1;
                    float y = cell.MinZ + (y_entry * (heightmap.CellSize / 9));
                    float z = Lerp(cell.Data.HeightRangeMin, cell.Data.HeightRangeMax, cell.Data.PointData[y_entry * cell.Data.PointCountY + x_entry] / 255.0f);
                    writer.Write($"{x.ToString(enCi)} {y.ToString(enCi)} {z.ToString(enCi)}\n");
                }
            }
        }
        // Write face data
        int verticesPerCell = 10 * 10;
        foreach (int cellIndex in Enumerable.Range(0, heightmap.Cells.Count))
        {
            foreach (int y in Enumerable.Range(0, 9))
            {
                foreach (int x in Enumerable.Range(0, 9))
                {
                    int firstVertexInFace = (cellIndex * verticesPerCell) + (10 * y) + x;
                    writer.Write($"4 {firstVertexInFace + 1} {firstVertexInFace} {firstVertexInFace + 10} {firstVertexInFace + 11}\n");
                }
            }
        }
    }

    public static void ExportGltf(Heightmap heightmap, string outputPath, bool singleMesh)
    {
        ModelRoot model = ModelRoot.CreateModel();
        Scene scene = model.UseScene("Heightmap");

        var material = model.CreateMaterial("Default");

        var node = scene.CreateNode("Heightmap");

        // Gltf doesn't support quads, the meshes are triangulated
        string filename;
        if (singleMesh)
        {
            filename = "heightmap_merged.gltf";
            CreateGltfSingleMesh(heightmap, node, model, material);
        }
        else
        {
            filename = "heightmap.gltf";
            CreateGltfIndividualCells(heightmap, node, model, material);
        }

        model.SaveGLTF(Path.Combine(outputPath, filename), new WriteSettings
        {
            Validation = SharpGLTF.Validation.ValidationMode.Skip
        });
    }

    private static float Lerp(float begin, float end, float factor)
    {
        return (1.0f - factor) * begin + factor * end;
    }

    private static void CreateGltfSingleMesh(Heightmap heightmap, Node node, ModelRoot model, SharpGLTF.Schema2.Material material)
    {
        List<Vector3> coordinates = new()
        {
            Capacity = heightmap.Cells.Count * 10 * 10 // Always contains 10 * 10 coordinates
        };
        List<int> triangleIndices = new()
        {
            Capacity = heightmap.Cells.Count * 81 * 2 // 9*9 for each quad * 2 for triangles
        };
        var primNode = node.CreateNode("HeightmapMerged");
        var mesh = primNode.Mesh = model.CreateMesh();

        foreach (HeightmapCell cell in heightmap.Cells)
        {
            // Get coordinates           
            foreach (int y_entry in Enumerable.Range(0, cell.Data.PointCountY))
            {
                foreach (int x_entry in Enumerable.Range(0, cell.Data.PointCountX))
                {
                    float x = cell.MinX + (x_entry * (heightmap.CellSize / 9));
                    float y = Lerp(cell.Data.HeightRangeMin, cell.Data.HeightRangeMax, cell.Data.PointData[y_entry * cell.Data.PointCountY + x_entry] / 255.0f);
                    float z = cell.MinZ + (y_entry * (heightmap.CellSize / 9));
                    coordinates.Add(MatrixMath.ConvertDxToOpenGl(new Vector3(x, y, z)));
                }
            }
        }
        // Get indices        
        int verticesPerCell = 10 * 10;
        foreach (int cellIndex in Enumerable.Range(0, heightmap.Cells.Count))
        {
            foreach (int y in Enumerable.Range(0, 9))
            {
                foreach (int x in Enumerable.Range(0, 9))
                {
                    int firstVertexInFace = (cellIndex * verticesPerCell) + (10 * y) + x;                    
                    // First triangle of quad
                    triangleIndices.Add(firstVertexInFace + 1);
                    triangleIndices.Add(firstVertexInFace);
                    triangleIndices.Add(firstVertexInFace + 10);
                    // Second triangle of quad
                    triangleIndices.Add(firstVertexInFace + 10);
                    triangleIndices.Add(firstVertexInFace + 11);
                    triangleIndices.Add(firstVertexInFace + 1);
                }
            }
        }

        var primitive = mesh.CreatePrimitive()
            .WithVertexAccessor("POSITION", coordinates);
        primitive.WithIndicesAccessor(PrimitiveType.TRIANGLES, triangleIndices)
            .WithMaterial(material);
    }

    private static void CreateGltfIndividualCells(Heightmap heightmap, Node node, ModelRoot model, SharpGLTF.Schema2.Material material)
    {
        int modelCounter = 0;
        foreach (HeightmapCell cell in heightmap.Cells)
        {
            string cellName = $"HCell_{modelCounter}";
            var primNode = node.CreateNode(cellName);

            var mesh = primNode.Mesh = model.CreateMesh();

            // Get coordinates
            List<Vector3> coordinates = new();
            coordinates.Capacity = 10 * 10; // Always contains 10*10 coordinates
            foreach (int y_entry in Enumerable.Range(0, cell.Data.PointCountY))
            {
                foreach (int x_entry in Enumerable.Range(0, cell.Data.PointCountX))
                {
                    float x = cell.MinX + (x_entry * (heightmap.CellSize / 9));
                    float y = Lerp(cell.Data.HeightRangeMin, cell.Data.HeightRangeMax, cell.Data.PointData[y_entry * cell.Data.PointCountY + x_entry] / 255.0f);
                    float z = cell.MinZ + (y_entry * (heightmap.CellSize / 9));
                    coordinates.Add(MatrixMath.ConvertDxToOpenGl(new Vector3(x, y, z)));
                }
            }
            // Get indices
            List<int> triangleIndices = new();
            triangleIndices.Capacity = 81 * 2; // 9*9 for each quad * 2 for triangles
            foreach (int y in Enumerable.Range(0, 9))
            {
                foreach (int x in Enumerable.Range(0, 9))
                {
                    int firstVertexInFace = (10 * y) + x;
                    // First triangle of quad
                    triangleIndices.Add(firstVertexInFace + 1);
                    triangleIndices.Add(firstVertexInFace);
                    triangleIndices.Add(firstVertexInFace + 10);
                    // Second triangle of quad
                    triangleIndices.Add(firstVertexInFace + 10);
                    triangleIndices.Add(firstVertexInFace + 11);
                    triangleIndices.Add(firstVertexInFace + 1);
                }
            }
            var primitive = mesh.CreatePrimitive()
                .WithVertexAccessor("POSITION", coordinates);
            primitive.WithIndicesAccessor(PrimitiveType.TRIANGLES, triangleIndices)
                .WithMaterial(material);
            modelCounter += 1;
        }
    }

    public static void ExportAllCellsInPacks(string inputPath, string outputPath)
    {
        string[] paths = Directory.GetFiles(inputPath, "*.pack", SearchOption.AllDirectories);
        Dictionary<int, HeightmapCellData> dictionaryEntries = new();
        Dictionary<int, string> dictionaryIsFrance_Chunk = new();
        foreach (string path in paths)
        {
            FileInfo info = new FileInfo(path);
            using FileStream packStream = new(path, FileMode.Open, FileAccess.Read, FileShare.None);
            using BinaryReader reader = new BinaryReader(packStream);

            if (!reader.CheckHeaderString("SBLA", reversed: true))
                throw new Exception("Invalid SBLA header found!");

            int unknownSblaHeaderValue = reader.ReadInt32();
            if (unknownSblaHeaderValue % 4 != 0)
            {
                throw new Exception();
            }
            // Skip pack if it doesn't have a HEI1 block
            if (!reader.CheckHeaderString("HEI1", reversed: true))
            {
                continue;
            }
            packStream.Position -= 4;

            // Get pack number
            int packNumber;
            try
            {
                packNumber = int.Parse(Path.GetFileNameWithoutExtension(path));
            }
            catch (FormatException ex)
            {
                continue;
            }
            
            dictionaryEntries[(packNumber) >> 8] = ReadHeightmapCellData(reader); // "real" number
            reader.Close();
            packStream.Close();
            // Check if it's a france chunk
            byte[] fileData = File.ReadAllBytes(path);
            int valueOffset = fileData.Locate(Encoding.ASCII.GetBytes("France_Chunk"));
            string chunkNumber = string.Empty;
            if (valueOffset != -1)
            {
                using FileStream packStream2 = new(path, FileMode.Open, FileAccess.Read, FileShare.None);
                using BinaryReader reader2 = new BinaryReader(packStream2);
                packStream2.Position = valueOffset + 12;
                chunkNumber = Encoding.ASCII.GetString(reader2.ReadBytes(7));
            }
            dictionaryIsFrance_Chunk[(packNumber) >> 8] = chunkNumber;       
        }

        // Export
        ModelRoot model = ModelRoot.CreateModel();
        Scene scene = model.UseScene("Heightmap");

        var material = model.CreateMaterial("Default");

        var node = scene.CreateNode("Heightmap");

        ExportHightmapCellData(dictionaryEntries, dictionaryIsFrance_Chunk, node, model, material);

        model.SaveGLTF(Path.Combine(outputPath, "heightmap_from_packs.gltf"), new WriteSettings
        {
            Validation = SharpGLTF.Validation.ValidationMode.Skip
        });

    }

    public static int Locate(this byte[] self, byte[] candidate)
    {
        if (IsEmptyLocate(self, candidate))
            return -1;

        for (int i = 0; i < self.Length; i++)
        {
            if (!IsMatch(self, i, candidate))
                continue;

            return i;
        }
        return -1;
    }
    static bool IsMatch(byte[] array, int position, byte[] candidate)
    {
        if (candidate.Length > (array.Length - position))
            return false;

        for (int i = 0; i < candidate.Length; i++)
            if (array[position + i] != candidate[i])
                return false;

        return true;
    }

    static bool IsEmptyLocate(byte[] array, byte[] candidate)
    {
        return array == null
            || candidate == null
            || array.Length == 0
            || candidate.Length == 0
            || candidate.Length > array.Length;
    }
}
