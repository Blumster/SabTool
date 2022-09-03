using System;
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

    public (int X, int Z) CalculateGrid(PaletteBlock paletteBlock)
    {
        return CalculateGrid(paletteBlock.X, paletteBlock.Z, paletteBlock.Index);
    }

    public (int X, int Z) CalculateGrid(float x, float z, ushort resolution)
    {
        if (resolution <= 3)
        {
            var xVal = (x - Extents[resolution][0].X) / GridLimits[resolution];
            var zVal = (z - Extents[resolution][0].Z) / GridLimits[resolution];

            return ((int)Math.Floor(xVal), (int)Math.Floor(zVal));
        }

        return (0, 0);
    }
}
