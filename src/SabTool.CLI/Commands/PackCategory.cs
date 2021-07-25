namespace SabTool.CLI.Commands
{
    using Base;
    using Pack;

    public class PackCategory : BaseCategory
    {
        public override string Key => "pack";

        public override string Shortcut => "p";

        public override string Usage => "<sub command>";

        public override void Setup()
        {
            SetupWithTypes(typeof(DynpackCategory), typeof(PalettepackCategory), typeof(PackDLCDynpackCategory));
        }
    }
}
