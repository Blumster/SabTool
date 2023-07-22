
using SabTool.Data.Lua;
using SabTool.Serializers.Lua;

namespace SabTool.Depot;
public sealed partial class ResourceDepot
{
    private LuaPackage? LuaScriptsPackage { get; set; }

    private bool LoadLua()
    {
        Console.WriteLine("Loading Lua...");

        string conversationsFilePath = GetGamePath("LuaScripts.luap");

        using FileStream fs = new(conversationsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        LuaScriptsPackage = LuapSerializer.DeserializeRaw(fs);

        Console.WriteLine("Lua loaded!");

        LoadedResources |= Resource.Lua;

        return true;
    }

    public IEnumerable<LuaPackage.Entry> GetLuaEntries()
    {
        return LuaScriptsPackage != null ? LuaScriptsPackage.Entries : Enumerable.Empty<LuaPackage.Entry>();
    }
}
