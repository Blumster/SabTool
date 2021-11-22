namespace SabTool.Depot
{
    using Data.Graphics;
    using Serializers.Graphics;
    using Utils;

    public partial class ResourceDepot
    {
        private const string MaterialsFileName = "France.materials";
        private Dictionary<Crc, Material> Materials { get; } = new();

        private bool LoadMaterials()
        {
            try
            {
                using var fs = new FileStream(GetGamePath(MaterialsFileName), FileMode.Open, FileAccess.Read, FileShare.Read);

                Console.WriteLine($"Loading Materials from {MaterialsFileName}...");

                var materials = MaterialSerializer.DeserializeRaw(fs);

                foreach (var material in materials)
                {
                    for (var i = 0; i < material.Keys.Count; ++i)
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
            return Materials.TryGetValue(key, out var value) ? value : null;
        }

        public IEnumerable<KeyValuePair<Crc, Material>> GetMaterials()
        {
            foreach (var material in Materials)
                yield return material;

            yield break;
        }
    }
}
