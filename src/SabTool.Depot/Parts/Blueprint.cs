namespace SabTool.Depot
{
    public partial class ResourceDepot
    {
        public bool LoadBlueprints(bool reload)
        {
            LoadedResources |= Resource.Blueprints;

            return false;
        }
    }
}
