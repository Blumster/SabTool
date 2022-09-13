namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;
using SabTool.CLI.Commands.Sounds;

public sealed class SoundCategory : BaseCategory
{
    public override string Key => "sound";

    public override string Shortcut => "s";

    public override string Usage => "<sub command>";

    public override void Setup()
    {
        SetupWithTypes(typeof(SoundPackCategory));
    }
}
