namespace SabTool.Depot
{
    public partial class ResourceDepot
    {
        public bool LoadMaps()
        {
            Console.WriteLine("Loading Maps...");

            LoadedResources |= Resource.Maps;

            Console.WriteLine("Maps loaded!");

            return true;
        }
    }
}
