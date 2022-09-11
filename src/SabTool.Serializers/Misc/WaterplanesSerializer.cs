using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;

namespace SabTool.Serializers.Misc;

using SabTool.Data.Misc;

public class WaterplanesSerializer
{
    public static List<WaterQuad> DeserializeRaw(List<string> lines)
    {
        // Cheapest possible parsing, skip lines that are irrelevant and don't check for correct order of properties
        CultureInfo enCi = CultureInfo.GetCultureInfo("en-US");
        List<WaterQuad> result = new List<WaterQuad>();
        WaterQuad? currentWaterQuad = null;
        foreach (string line in lines)
        {
            string currentLine = line.Replace(" ", string.Empty);
            currentLine = currentLine.Replace("\t", string.Empty);
            if (currentLine == string.Empty ||
                currentLine == "{" ||
                currentLine == "}")
            {
                continue;
            }
            string content = GetStringInParenthesis(currentLine);
            if (currentLine.StartsWith("water_quad"))
            {
                content = content.Replace("\"", string.Empty);
                currentWaterQuad = new()
                {
                    Name = content
                };
                continue;
            }
            if (currentWaterQuad == null)
            {
                throw new ArgumentException("Wrong command order");
            }
            if (currentLine.StartsWith("slot"))
            {
                currentWaterQuad.Slot = int.Parse(content);
                continue;
            }
            if (currentLine.StartsWith("vertex_y"))
            {
                currentWaterQuad.VertexY = float.Parse(content, enCi);
                continue;
            }
            if (currentLine.StartsWith("vertex_xz"))
            {
                string[] values = content.Split(',');
                if (values.Length != 2)
                {
                    throw new ArgumentException("Invalid vertex_xz");
                }
                currentWaterQuad.VerticesXZ.Add(new Vector2(float.Parse(values[0], enCi), float.Parse(values[1], enCi)));
                if (currentWaterQuad.VerticesXZ.Count == 4)
                {
                    result.Add(currentWaterQuad);
                    currentWaterQuad = new WaterQuad();
                }
                continue;
            }
            throw new ArgumentException("Unknown content for waterplanes");
        }
        return result;
    }

    private static string GetStringInParenthesis(string source)
    {
        int start = source.IndexOf('(');
        int end = source.IndexOf(')');
        if (start == -1 || end == -1 || start > end)
        {
            throw new ArgumentException("Invalid string");
        }
        return source.Substring(start + 1, end - (start + 1));
    }

    public static void ExportGltf(List<WaterQuad> quads, string outputPath)
    {
        SceneBuilder scene = new();
        MaterialBuilder material = new();

        foreach (WaterQuad quad in quads)
        {
            MeshBuilder<VertexPosition> mesh = new(quad.Name);
            var primitive = mesh.UsePrimitive(material, 3);  // gltf doesn't support quads
            primitive.AddTriangle(new VertexPosition(quad.VerticesXZ[0].X, quad.VertexY, quad.VerticesXZ[0].Y),
                                  new VertexPosition(quad.VerticesXZ[1].X, quad.VertexY, quad.VerticesXZ[1].Y),
                                  new VertexPosition(quad.VerticesXZ[2].X, quad.VertexY, quad.VerticesXZ[2].Y));
            primitive.AddTriangle(new VertexPosition(quad.VerticesXZ[0].X, quad.VertexY, quad.VerticesXZ[0].Y),
                                  new VertexPosition(quad.VerticesXZ[2].X, quad.VertexY, quad.VerticesXZ[2].Y),
                                  new VertexPosition(quad.VerticesXZ[3].X, quad.VertexY, quad.VerticesXZ[3].Y));
            scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        ModelRoot model = scene.ToGltf2();
        model.SaveGLTF(Path.Combine(outputPath, "waterplanes.gltf"), new WriteSettings
        {
            Validation = SharpGLTF.Validation.ValidationMode.Strict
        });
    }
}
