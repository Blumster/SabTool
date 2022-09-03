using System.Collections.Generic;

namespace SabTool.Data.Misc;

public class Heightmap // HEI5
{
    public List<HeightmapCell> Cells { get; } = new();
    public int CellCountMaxX { get; set; }
    public int CellCountMaxY { get; set; }
    public float CellSize { get; set; } // Cells are rectangular
    public float MinX { get; set; }
    public float MinY { get; set; }
    public float MinZ { get; set; }
}

public class HeightmapCell // HEI1 + other data
{
    public HeightmapCellData Data = new();
    public float MinX { get; set; }
    public const float MinY = 0.0f; // MinY is always 0.0f
    public float MinZ { get; set; }
    public float MaxX { get; set; }
    public const float MaxY = 0.0f; // MaxY is always 0.0f
    public float MaxZ { get; set; }
}

public class HeightmapCellData
{
    // HEI1
    public int PointCountX { get; set; }
    public int PointCountY { get; set; }
    public float HeightRangeMax { get; set; }
    public float HeightRangeMin { get; set; }
    public byte[] PointData { get; set; }
}
