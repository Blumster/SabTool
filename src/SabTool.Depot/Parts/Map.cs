using System.Numerics;

using SabTool.Data.Packs;
using SabTool.Serializers.Megapacks;
using SabTool.Serializers.Packs;
using SabTool.Utils;

namespace SabTool.Depot;
public sealed partial class ResourceDepot
{
    public GlobalMap? GlobalMap { get; set; }
    public GlobalMap? DLCGlobalMap { get; set; }
    public FranceMap? FranceMap { get; set; }
    public Dictionary<(short, short, ushort), StreamBlock> MapBlocks { get; } = new();

    private bool LoadMaps()
    {
        Console.WriteLine("Loading Maps...");

        LoadGlobalMap();
        LoadDLCGlobalMap();
        LoadFranceMap();
        LoadDLCFranceMap();

        LoadedResources |= Resource.Maps;

        Console.WriteLine("Maps loaded!");

        return true;
    }

    private void LoadGlobalMap()
    {
        Console.WriteLine("  Loading Global Map...");

        using MemoryStream globalMapStream = GetLooseFile("global.map") ?? throw new Exception($"global.map is missing from {LooseFilesFileName}!");

        GlobalMap = GlobalMapSerializer.DeserializeRaw(globalMapStream);

        Console.WriteLine("  Global Map loaded!");
    }

    private void LoadDLCGlobalMap()
    {
        Console.WriteLine("  Loading DLC Global Map...");

        string DLCGlobalMapPath = GetGamePath(@"DLC\01\Global.map");

        using FileStream DLCGlobalMapStream = new(DLCGlobalMapPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        DLCGlobalMap = GlobalMapSerializer.DeserializeRaw(DLCGlobalMapStream);

        Console.WriteLine("  DLC Global Map loaded!");
    }

    private void LoadFranceMap()
    {
        Console.WriteLine("  Loading France Map...");

        using MemoryStream franceMapStream = GetLooseFile("France.map") ?? throw new Exception($"france.map is missing from {LooseFilesFileName}!");

        FranceMap = FranceMapSerializer.DeserializeRaw(franceMapStream);

        Console.WriteLine("  France Map loaded!");
    }

    private void LoadDLCFranceMap()
    {
        Console.WriteLine("Loading DLC France Map...");

        string DLCFranceMapPath = GetGamePath(@"DLC\01\FRANCE.map");

        using FileStream DLCFranceMapStream = new(DLCFranceMapPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        _ = FranceMapSerializer.DeserializeRaw(DLCFranceMapStream, FranceMap);

        Console.WriteLine("  DLC France Map loaded!");
    }

    public StreamBlock? GetStreamBlock(Crc crc)
    {
        if (!IsResourceLoaded(Resource.Maps))
            _ = Load(Resource.Maps);

        StreamBlock? dynBlock = GlobalMap!.GetDynamicBlock(crc);
        if (dynBlock != null)
            return dynBlock;

        dynBlock = DLCGlobalMap!.GetDynamicBlock(crc);
        if (dynBlock != null)
            return dynBlock;

        StreamBlock? staticBlock = GlobalMap.GetStaticBlock(crc);
        if (staticBlock != null)
            return staticBlock;

        if (FranceMap!.Interiors.TryGetValue(crc, out StreamBlock? interiorBlock))
            return interiorBlock;

        if (FranceMap!.CinematicBlocks.TryGetValue(crc, out StreamBlock? cinematicBlock))
            return cinematicBlock;

        uint value = crc.Value;

        byte resolution = (byte)(value & 0xFF);
        if (resolution <= 2)
        {
            ushort xOffu = (ushort)((value >> 17) & 0x1FF);
            if ((xOffu & 0x100) != 0)
                xOffu |= 0xFE00;

            ushort zOffu = (ushort)((value >> 8) & 0x1FF);
            if ((zOffu & 0x100) != 0)
                zOffu |= 0xFE00;

            short xOff = (short)xOffu;
            short zOff = (short)zOffu;

            (short xOff, short zOff, byte resolution) blockKey = (xOff, zOff, resolution);

            if (MapBlocks.ContainsKey(blockKey))
                return MapBlocks[blockKey];

            lock (MapBlocks)
            {
                if (MapBlocks.ContainsKey(blockKey))
                    return MapBlocks[blockKey];

                float x = xOff * FranceMap.GridLimits[resolution];
                float z = zOff * FranceMap.GridLimits[resolution];

                (int X, int Z) grid = FranceMap.CalculateGrid(x, z, resolution);

                StreamBlock block = new()
                {
                    Id = crc.Value,
                    FileName = crc.Value <= 9 ? $"France\\{crc.Value}" : $"France\\{crc.Value.ToString()[..2]}\\{crc.Value}"
                };

                block.Extents[0] = new Vector3(
                    (grid.X * FranceMap.GridLimits[resolution]) + FranceMap.Extents[resolution][0].X,
                    FranceMap.Extents[resolution][0].Y,
                    (grid.Z * FranceMap.GridLimits[resolution]) + FranceMap.Extents[resolution][0].Z);

                block.Extents[1] = new Vector3(
                    block.Extents[0].X + FranceMap.GridLimits[resolution],
                    0.0f,
                    block.Extents[0].Z * FranceMap.GridLimits[resolution]);

                block.Midpoint = new Vector3(
                    ((block.Extents[1].X - block.Extents[0].X) * 0.5f) + block.Extents[0].X,
                    ((block.Extents[1].Y - block.Extents[0].Y) * 0.5f) + block.Extents[0].Y,
                    ((block.Extents[1].Z - block.Extents[0].Z) * 0.5f) + block.Extents[0].Z);

                block.Flags ^= (block.Flags ^ (uint)(resolution << 10)) & 0x1C00u;
                block.Flags &= 0xFFFFFF7F; // ~0x80
                block.Flags &= 0xFFFFFFBF; // ~0x40
                block.Flags &= 0xFFFFFFFE; // ~0x01
                block.Flags &= 0xFFFFFFFD; // ~0x02
                block.Flags &= 0xFFFFFFFB; // ~0x04
                block.Flags &= 0xFFFFFEFF; // ~0x100
                block.Flags |= 0x20;

                MapBlocks[blockKey] = block;

                return block;
            }
        }

        return null;
    }
}
