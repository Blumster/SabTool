using System.Collections.Generic;
using System.Numerics;

namespace SabTool.Data.Packs;

using SabTool.Utils;

public class FranceMap
{
    public static readonly float[] GridLimits = new float[3] { 500.0f, 200.0f, 25.0f };

    public int NumStreamBlocks { get; set; }
    public int FieldDA84 { get; set; }
    public int FieldDA88 { get; set; }
    public Vector3[][] Extents { get; } = new Vector3[3][];
    public int[] GridCountX { get; } = new int[3];
    public int[] GridCountZ { get; } = new int[3];
    public int[][] UnkArray1 { get; } = new int[2][];
    public Dictionary<(int X, int Y), PaletteBlock> Palettes { get; set; } = new();
    public Dictionary<Crc, StreamBlock> Interiors { get; set; }
    public Dictionary<Crc, StreamBlock> CinematicBlocks { get; set; }

    public PaletteBlock GetPaletteBlock(int x, int y)
    {
        return Palettes.TryGetValue((x, y), out var paletteBlock) ? paletteBlock : null;
    }

    public (int X, int Y) CalculateGrid(PaletteBlock paletteBlock)
    {
        return CalculateGrid(paletteBlock.X, paletteBlock.Z, paletteBlock.Index);
    }

    public (int X, int Y) CalculateGrid(float x, float z, short index)
    {
        if (index <= 3)
        {
            var xVal = (x - Extents[index][0].X) / GridLimits[index];
            var zVal = (z - Extents[index][0].Z) / GridLimits[index];

            return ((int)xVal, (int)zVal);
        }

        return (0, 0);
    }
}
