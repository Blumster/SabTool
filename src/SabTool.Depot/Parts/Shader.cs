namespace SabTool.Depot;

using SabTool.Data.Graphics.Shaders;
using SabTool.Serializers.Graphics.Shaders;

public partial class ResourceDepot
{
    private const string ShadersFileName = "France.shaders";
    private ShaderContainer ShaderContainer { get; set; }

    private bool LoadShaders()
    {
        try
        {
            var shadersFile = GetLooseFile(ShadersFileName) ?? throw new Exception($"{ShadersFileName} is missing from {LooseFilesFileName}!");

            Console.WriteLine($"Loading Shaders from {ShadersFileName}...");

            ShaderContainer = ShaderContainerSerializer.DeserializeRaw(shadersFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while loading shaders: {ex}");
            return false;
        }

        Console.WriteLine("Shaders loaded!");
        return true;
    }

    public IEnumerable<Shader> GetPixelShaders() => ShaderContainer.PixelShaders;
    public IEnumerable<Shader> GetVertexShaders() => ShaderContainer.VertexShaders;
}
