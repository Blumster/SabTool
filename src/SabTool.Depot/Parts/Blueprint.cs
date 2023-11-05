namespace SabTool.Depot;

using SabTool.GameData;
using SabTool.Serializers.Blueprints;
using SabTool.Utils;

public sealed partial class ResourceDepot
{
    private Dictionary<Crc, Blueprint> BlueprintsByNameCrc { get; set; }

    private bool LoadBlueprints()
    {
        Console.WriteLine("Loading Blueprints...");

        using var gameTemplatesStream = GetLooseFile(@"GameTemplates.wsd") ?? throw new Exception("GameTemplates.wsd is missing from the loosefiles!");

        var blueprints = BlueprintSerializer.DeserializeRaw(gameTemplatesStream);

        BlueprintsByNameCrc = blueprints.ToDictionary(b => b.Name);

        Console.WriteLine("Blueprints loaded!");

        LoadedResources |= Resource.Blueprints;

        return true;
    }

    public object? GetBlueprintByNameCrc(Crc nameCrc)
    {
        return BlueprintsByNameCrc.TryGetValue(nameCrc, out var blueprint) ? blueprint : null;
    }

    public IEnumerable<Blueprint> GetAllBlueprints() => BlueprintsByNameCrc.Values;
}
