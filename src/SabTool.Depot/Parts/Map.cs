using System.Numerics;

namespace SabTool.Depot;

using SabTool.Data.Packs;
using SabTool.Serializers.Megapacks;
using SabTool.Serializers.Packs;
using SabTool.Utils;

public sealed partial class ResourceDepot
{
    public GlobalMap? GlobalMap { get; set; }
    public GlobalMap? DLCGlobalMap { get; set; }
    public FranceMap? FranceMap { get; set; }
    public Dictionary<(short, short, ushort), StreamBlock> MapBlocks { get; } = new();

    public EditNodes EditNodes { get; private set; }
    public EditNodes DLCEditNodes { get; private set; }

    private bool LoadMaps()
    {
        Console.WriteLine("Loading Map data...");

        LoadGlobalMap();
        LoadDLCGlobalMap();
        LoadFranceMap();
        LoadDLCFranceMap();
        LoadEditNodes();

        LoadedResources |= Resource.Maps;

        Console.WriteLine("Map data loaded!");

        return true;
    }

    private void LoadGlobalMap()
    {
        Console.WriteLine("  Loading Global Map...");

        using var globalMapStream = GetLooseFile("global.map") ?? throw new Exception($"global.map is missing from {LooseFilesFileName}!");

        GlobalMap = GlobalMapSerializer.DeserializeRaw(globalMapStream);

        Console.WriteLine("  Global Map loaded!");
    }

    private void LoadDLCGlobalMap()
    {
        Console.WriteLine("  Loading DLC Global Map...");

        var DLCGlobalMapPath = GetGamePath(@"DLC\01\Global.map");

        using var DLCGlobalMapStream = new FileStream(DLCGlobalMapPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        DLCGlobalMap = GlobalMapSerializer.DeserializeRaw(DLCGlobalMapStream);

        Console.WriteLine("  DLC Global Map loaded!");
    }

    private void LoadFranceMap()
    {
        Console.WriteLine("  Loading France Map...");

        using var franceMapStream = GetLooseFile("France.map") ?? throw new Exception($"france.map is missing from {LooseFilesFileName}!");

        FranceMap = FranceMapSerializer.DeserializeRaw(franceMapStream);

        Console.WriteLine("  France Map loaded!");
    }

    private void LoadDLCFranceMap()
    {
        Console.WriteLine("  Loading DLC France Map...");

        var DLCFranceMapPath = GetGamePath(@"DLC\01\FRANCE.map");

        using var DLCFranceMapStream = new FileStream(DLCFranceMapPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        FranceMapSerializer.DeserializeRaw(DLCFranceMapStream, FranceMap);

        Console.WriteLine("  DLC France Map loaded!");
    }

    private void LoadEditNodes()
    {
        Console.WriteLine("  Loading EditNodes...");

        using var editNodesPack = GetLooseFile(@"France\EditNodes\EditNodes.pack") ?? throw new Exception("EditNodes.pack is missing from the loosefiles!");
        EditNodes = EditNodesSerializer.DeserializeRaw(editNodesPack);

        using var dlcEditNodesPack = new FileStream(GetGamePath(@"DLC\01\France\EditNodes\EditNodes.pack"), FileMode.Open, FileAccess.Read, FileShare.Read);
        DLCEditNodes = EditNodesSerializer.DeserializeRaw(dlcEditNodesPack);

        Console.WriteLine($"  Loaded EditNodes: Base: {EditNodes.Nodes.Count} DLC: {DLCEditNodes.Nodes.Count}");
    }

    public StreamBlock? GetStreamBlock(Crc crc)
    {
        if (!IsResourceLoaded(Resource.Maps))
            Load(Resource.Maps);

        var dynBlock = GlobalMap!.GetDynamicBlock(crc);
        if (dynBlock != null)
            return dynBlock;

        dynBlock = DLCGlobalMap!.GetDynamicBlock(crc);
        if (dynBlock != null)
            return dynBlock;

        var staticBlock = GlobalMap.GetStaticBlock(crc);
        if (staticBlock != null)
            return staticBlock;

        if (FranceMap!.Interiors.TryGetValue(crc, out var interiorBlock))
            return interiorBlock;

        if (FranceMap!.CinematicBlocks.TryGetValue(crc, out var cinematicBlock))
            return cinematicBlock;

        var value = crc.Value;

        var resolution = (byte)(value & 0xFF);
        if (resolution <= 2)
        {
            var xOffu = (ushort)((value >> 17) & 0x1FF);
            if ((xOffu & 0x100) != 0)
                xOffu |= 0xFE00;

            var zOffu = (ushort)((value >> 8) & 0x1FF);
            if ((zOffu & 0x100) != 0)
                zOffu |= 0xFE00;

            var xOff = (short)xOffu;
            var zOff = (short)zOffu;

            var blockKey = (xOff, zOff, resolution);

            if (MapBlocks.ContainsKey(blockKey))
                return MapBlocks[blockKey];

            lock (MapBlocks)
            {
                if (MapBlocks.ContainsKey(blockKey))
                    return MapBlocks[blockKey];

                var x = xOff * FranceMap.GridLimits[resolution];
                var z = zOff * FranceMap.GridLimits[resolution];

                var grid = FranceMap.CalculateGrid(x, z, resolution);

                var block = new StreamBlock
                {
                    Id = crc.Value,
                    FileName = crc.Value <= 9 ? $"France\\{crc.Value}" : $"France\\{crc.Value.ToString()[..2]}\\{crc.Value}"
                };

                block.Extents[0] = new Vector3(
                    grid.X * FranceMap.GridLimits[resolution] + FranceMap.Extents[resolution][0].X,
                    FranceMap.Extents[resolution][0].Y,
                    grid.Z * FranceMap.GridLimits[resolution] + FranceMap.Extents[resolution][0].Z);

                block.Extents[1] = new Vector3(
                    block.Extents[0].X + FranceMap.GridLimits[resolution],
                    0.0f,
                    block.Extents[0].Z * FranceMap.GridLimits[resolution]);

                block.Midpoint = new Vector3(
                    (block.Extents[1].X - block.Extents[0].X) * 0.5f + block.Extents[0].X,
                    (block.Extents[1].Y - block.Extents[0].Y) * 0.5f + block.Extents[0].Y,
                    (block.Extents[1].Z - block.Extents[0].Z) * 0.5f + block.Extents[0].Z);

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
