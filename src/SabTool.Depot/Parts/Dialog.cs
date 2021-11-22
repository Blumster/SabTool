using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Depot
{
    using Data;
    using Serializers;

    public partial class ResourceDepot
    {
        private const string DialogTextFileName = "GameText.dlg";

        private Dictionary<string, Dialog> Dialogs { get; set; } = new();

        public bool LoadDialogs()
        {
            Console.WriteLine("Loading Dialogs...");

            var dialogPath = GetGamePath(@"Cinematics\Dialog");

            foreach (var folder in Directory.GetDirectories(dialogPath))
            {
                var dialogFilePath = Path.Combine(folder, DialogTextFileName);
                if (!File.Exists(dialogFilePath))
                    continue;

                var language = new DirectoryInfo(folder).Name;

                using var fs = new FileStream(dialogFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                Dialogs.Add(language, DialogSerializer.DeserialzieRaw(fs));
            }

            Console.WriteLine("Dialogs loaded!");

            LoadedResources |= Resource.Dialogs;

            return true;
        }

        public IEnumerable<KeyValuePair<string, Dialog>> GetDialogs()
        {
            foreach (var dialog in Dialogs)
                yield return dialog;
        }
    }
}
