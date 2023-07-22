
using SabTool.Data.Graphics;
using SabTool.Serializers.Graphics;
using SabTool.Utils;

namespace SabTool.Depot;
public sealed partial class ResourceDepot
{
    private const string MaterialsFileName = "France.materials";
    private Dictionary<Crc, Material> Materials { get; } = new();

    private bool LoadMaterials()
    {
        try
        {
            using FileStream fs = new(GetGamePath(MaterialsFileName), FileMode.Open, FileAccess.Read, FileShare.Read);

            Console.WriteLine($"Loading Materials from {MaterialsFileName}...");

            List<Material> materials = MaterialSerializer.DeserializeRaw(fs);

            foreach (Material material in materials)
            {
                for (int i = 0; i < material.Keys.Count; ++i)
                {
                    Materials.Add(material.Keys[i], material);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while loading materials: {ex}");
            return false;
        }

        Console.WriteLine("Materials loaded!");
        return true;
    }

    public Material? GetMaterial(Crc key)
    {
        return Materials.TryGetValue(key, out Material? value) ? value : null;
    }

    public IEnumerable<KeyValuePair<Crc, Material>> GetMaterials()
    {
        foreach (KeyValuePair<Crc, Material> material in Materials)
            yield return material;

        yield break;
    }
}
