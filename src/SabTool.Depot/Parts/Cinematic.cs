using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Depot
{
    using Data.Cinematics;
    using Serializers.Cinematics;

    public partial class ResourceDepot
    {
        private const string DialogTextFileName = "GameText.dlg";

        private Dictionary<string, Dialog> Dialogs { get; set; } = new();
        private List<ComplexAnimStructure> ComplexAnims { get; set; }

        public bool LoadCinematics()
        {
            Console.WriteLine("Loading Cinematics...");

            LoadDialogs();
            LoadComplexAnims();

            Console.WriteLine("Cinematics loaded!");

            LoadedResources |= Resource.Cinematics;

            return true;
        }

        private void LoadDialogs()
        {
            Console.WriteLine("  Loading Dialogs...");

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

            Console.WriteLine("  Dialogs loaded!");
        }

        private void LoadComplexAnims()
        {
            Console.WriteLine("  Loading ComplexAnims...");

            var complexAnimsFilePath = GetGamePath(@"Cinematics\ComplexAnimations\ComplexAnims.cxa");

            using var fs = new FileStream(complexAnimsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            ComplexAnims = ComplexAnimsSerializer.DeserializeRaw(fs);

            Console.WriteLine("  ComplexAnims loaded!");
        }

        public IEnumerable<KeyValuePair<string, Dialog>> GetDialogs()
        {
            foreach (var dialog in Dialogs)
                yield return dialog;
        }

        public IEnumerable<ComplexAnimStructure> GetComplexAnimStructures()
        {
            return ComplexAnims;
        }
    }
}
