namespace SabTool.Depot
{
    public partial class ResourceDepot
    {
        private bool LoadBlueprints()
        {
            LoadedResources |= Resource.Blueprints;

            return false;
        }
    }
}
