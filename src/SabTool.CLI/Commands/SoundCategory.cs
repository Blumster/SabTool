namespace SabTool.CLI.Commands
{
    using Base;
    using Sounds;

    public class SoundCategory : BaseCategory
    {
        public override string Key => "sound";

        public override string Shortcut => "s";

        public override string Usage => "<sub command>";

        public override void Setup()
        {
            SetupWithTypes(typeof(SoundPackCategory));
        }
    }
}
