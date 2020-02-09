using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.CLI.Commands
{
    using Base;
    using GameTemplates;

    public class BaseGameTemplatesCategory : BaseCategory
    {
        public override string Key => "game-templates";

        public override string Usage => "<sub command>";

        public override void Setup()
        {
            _commands.Clear();

            AddInstance<GameTemplatesCategory>();
            AddInstance<GameTemplatesDLCCategory>();
        }
    }
}
