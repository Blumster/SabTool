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
            if (!reader.CheckHeaderString("HEI1", reversed: true))
            {
                throw new Exception("Invalid HEI1 header found!");
            }

            HeightmapCell hei1 = new()
            {
                PointCountX = reader.ReadInt32(),
                PointCountY = reader.ReadInt32(),
                HeightRangeMax = reader.ReadSingle(),
                HeightRangeMin = reader.ReadSingle()
            };
            hei1.PointData = reader.ReadBytes(hei1.PointCountX * hei1.PointCountY);
            hei1.MinX = reader.ReadSingle();
            if (reader.ReadSingle() != HeightmapCell.MinY)
            {
                throw new Exception("Unexpected MinY");
            }
            hei1.MinZ = reader.ReadSingle();
            hei1.MaxX = reader.ReadSingle();
            if (reader.ReadSingle() != HeightmapCell.MaxY)
            {
                throw new Exception("Unexpected MaxY");
            }
            hei1.MaxZ = reader.ReadSingle();

            heightmap.Cells.Add(hei1);
        }

        if (stream.Position != stream.Length)
        {
            throw new Exception("Under reading HEI5 file!");
        }
        return heightmap;
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
            foreach (int y_entry in Enumerable.Range(0, cell.PointCountY))
            {
                foreach (int x_entry in Enumerable.Range(0, cell.PointCountX))
                {
                    // x / y / z in this block are in Blender coordinates where z is height instead of y and x is -x
                    float x = cell.MinX + (x_entry * (heightmap.CellSize / 9));
                    x *= -1;
                    float y = cell.MinZ + (y_entry * (heightmap.CellSize / 9));
                    float z = Lerp(cell.HeightRangeMin, cell.HeightRangeMax, cell.PointData[y_entry * cell.PointCountY + x_entry] / 255.0f);
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
            foreach (int y_entry in Enumerable.Range(0, cell.PointCountY))
            {
                foreach (int x_entry in Enumerable.Range(0, cell.PointCountX))
                {
                    float x = cell.MinX + (x_entry * (heightmap.CellSize / 9));
                    float y = Lerp(cell.HeightRangeMin, cell.HeightRangeMax, cell.PointData[y_entry * cell.PointCountY + x_entry] / 255.0f);
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
            foreach (int y_entry in Enumerable.Range(0, cell.PointCountY))
            {
                foreach (int x_entry in Enumerable.Range(0, cell.PointCountX))
                {
                    float x = cell.MinX + (x_entry * (heightmap.CellSize / 9));
                    float y = Lerp(cell.HeightRangeMin, cell.HeightRangeMax, cell.PointData[y_entry * cell.PointCountY + x_entry] / 255.0f);
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
}
