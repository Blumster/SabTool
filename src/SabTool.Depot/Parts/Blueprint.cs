namespace SabTool.Depot
{
    public partial class ResourceDepot
    {
        public bool LoadBlueprints()
        {
            LoadedResources |= Resource.Blueprints;

            return false;
        }
    }
}
