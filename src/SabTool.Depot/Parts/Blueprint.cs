namespace SabTool.Depot;

using SabTool.Data.Blueprints;
using SabTool.Serializers.Blueprints;
using SabTool.Utils;

public partial class ResourceDepot
{
    private Dictionary<string, Blueprint> BlueprintsByName { get; } = new();
    private Dictionary<Crc, Blueprint> BlueprintsByNameCrc { get; } = new();

    private bool LoadBlueprints()
    {
        Console.WriteLine("Loading Blueprints...");

        using var gameTemplatesStream = GetLooseFile(@"GameTemplates.wsd") ?? throw new Exception("GameTemplates.wsd is missing from the loosefiles!");

        var blueprints = BlueprintSerializer.DeserializeRaw(gameTemplatesStream);

        foreach (var blueprint in blueprints)
        {
            BlueprintsByName.Add(blueprint.Name, blueprint);
            BlueprintsByNameCrc.Add(new(Hash.StringToHash(blueprint.Name)), blueprint);
        }

        Console.WriteLine("Blueprints loaded!");

        LoadedResources |= Resource.Blueprints;

        return true;
    }

    public Blueprint? GetBlueprintByName(string name)
    {
        return BlueprintsByName.TryGetValue(name, out var blueprint) ? blueprint : null;
    }

    public Blueprint? GetBlueprintByNameCrc(Crc nameCrc)
    {
        return BlueprintsByNameCrc.TryGetValue(nameCrc, out var blueprint) ? blueprint : null;
    }

    public IEnumerable<Blueprint> GetAllBlueprints() => BlueprintsByName.Values;
}
