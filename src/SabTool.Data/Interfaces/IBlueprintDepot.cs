namespace SabTool.Data.Interfaces;

using SabTool.Data.Blueprints;

public interface IBlueprintDepot
{
    Blueprint? GetBlueprintByName(string name);
    Blueprint? GetBlueprintByNameCrc(Crc nameCrc);
}
