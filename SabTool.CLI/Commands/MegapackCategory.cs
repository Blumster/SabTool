namespace SabTool.CLI.Commands
{
    using Base;
    using Megapack;

    public class MegapackCategory : BaseCategory
    {
        public override string Key => "megapack";

        public override string Usage => "<sub command>";

        public override void Setup()
        {
            _commands.Clear();

            AddInstance<MegapackGlobalCategory>();
            AddInstance<MegapackFranceCategory>();
            AddInstance<MegapackDLCGlobalCategory>();
            AddInstance<MegapackDLCFranceCategory>();
        }

        /*
         * var looseFile = new LooseFile();

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    looseFile.Read(fs);

                foreach (var file in looseFile.Files)
                {
                }
         * */
    }
}
