
using SabTool.CLI.Base;
using SabTool.CLI.Commands.Graphics;

namespace SabTool.CLI.Commands;
public sealed class GraphicsCategory : BaseCategory
{
    public override string Key => "graphics";

    public override string Shortcut => "gr";

    public override string Usage => "<sub command>";

    public override void Setup()
    {
        SetupWithTypes(typeof(MaterialCategory), typeof(ShaderCategory), typeof(ShaderMappingCategory));
    }
}
