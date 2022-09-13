namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;
using SabTool.CLI.Commands.Cinematics;

public sealed class CinematicsCategory : BaseCategory
{
    public override string Key => "cinematics";

    public override string Shortcut => "c";

    public override string Usage => "<sub command>";

    public override void Setup()
    {
        SetupWithTypes(typeof(Cinematics.CinematicsCategory), typeof(ComplexAnimsCategory), typeof(ConversationsCategory), typeof(DialogCategory), typeof(RandomTextCategory));
    }
}
