namespace SabTool.CLI.Commands;

using Base;
using Cinematics;

public class CinematicsCategory : BaseCategory
{
    public override string Key => "cinematics";

    public override string Shortcut => "c";

    public override string Usage => "<sub command>";

    public override void Setup()
    {
        SetupWithTypes(typeof(Cinematics.CinematicsCategory), typeof(ComplexAnimsCategory), typeof(ConversationsCategory), typeof(DialogCategory), typeof(RandomTextCategory));
    }
}
