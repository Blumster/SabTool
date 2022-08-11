namespace SabTool.Depot;

using SabTool.Data.Lua;
using SabTool.Serializers.Lua;

public partial class ResourceDepot
{
    private LuaPackage? LuaScriptsPackage { get; set; }

    public bool LoadLua()
    {
        Console.WriteLine("Loading Lua...");

        var conversationsFilePath = GetGamePath("LuaScripts.luap");

        using var fs = new FileStream(conversationsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

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
