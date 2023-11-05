namespace SabTool.Data.Interfaces;

public interface IBlueprintDepot
{
    object? GetBlueprintByNameCrc(Crc nameCrc);
}
