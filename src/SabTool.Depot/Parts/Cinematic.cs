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
        private List<ConversationStructure>? Conversations { get; set; }
        private List<ConversationStructure>? DLCConversations { get; set; }
        private List<ComplexAnimStructure>? ComplexAnims { get; set; }
        private List<Cinematic>? Cinematics { get; set; }
        private List<Cinematic>? DLCCinematics { get; set; }
        private List<RandomText>? RandomTexts { get; set; }

        public bool LoadCinematics()
        {
            Console.WriteLine("Loading Cinematics...");

            LoadDialogs();
            LoadConversations();
            LoadDLCConversations();
            LoadComplexAnims();
            LoadCinematicsFile();
            LoadDLCCinematicsFile();
            LoadRandomTexts();

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

        private void LoadConversations()
        {
            Console.WriteLine("  Loading Conversations...");

            using var conversationsStream = GetLooseFile(@"Cinematics\Conversations\Conversations.cnvpack") ?? throw new Exception("Conversations cnvpack is missing from the loosefiles!");

            Conversations = ConversationsSerializer.DeserializeRaw(conversationsStream);

            Console.WriteLine("  Conversations loaded!");
        }

        private void LoadDLCConversations()
        {
            Console.WriteLine("  Loading DLC Conversations...");

            var conversationsFilePath = GetGamePath(@"DLC\01\Cinematics\Conversations\Conversations.cnvpack");

            using var fs = new FileStream(conversationsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            DLCConversations = ConversationsSerializer.DeserializeRaw(fs);

            Console.WriteLine("  DLC Conversations loaded!");
        }

        private void LoadComplexAnims()
        {
            Console.WriteLine("  Loading ComplexAnims...");

            var complexAnimsFilePath = GetGamePath(@"Cinematics\ComplexAnimations\ComplexAnims.cxa");

            using var fs = new FileStream(complexAnimsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            ComplexAnims = ComplexAnimsSerializer.DeserializeRaw(fs);

            Console.WriteLine("  ComplexAnims loaded!");
        }

        private void LoadCinematicsFile()
        {
            Console.WriteLine("  Loading Cinematics...");

            using var cinematicsStream = GetLooseFile(@"Cinematics\Cinematics.cinpack") ?? throw new Exception("Cinematics cinpack is missing from the loose files!");

            Cinematics = CinematicsSerializer.DeserializeRaw(cinematicsStream);

            Console.WriteLine("  Cinematics loaded!");
        }

        private void LoadDLCCinematicsFile()
        {
            Console.WriteLine("  Loading DLC Cinematics...");

            var complexAnimsFilePath = GetGamePath(@"DLC\01\Cinematics\Cinematics.cinpack");

            using var fs = new FileStream(complexAnimsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            DLCCinematics = CinematicsSerializer.DeserializeRaw(fs);

            Console.WriteLine("  DLC Cinematics loaded!");
        }

        private void LoadRandomTexts()
        {
            Console.WriteLine("  Loading Random Texts...");

            var randomTextsFilePath = GetGamePath(@"Cinematics\Dialog\Random\RandomText.rnd");

            using var fs = new FileStream(randomTextsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            RandomTexts = RandomTextSerializer.DeserializeRaw(fs);

            Console.WriteLine("  Random Texts loaded!");
        }

        public IEnumerable<KeyValuePair<string, Dialog>> GetDialogs()
        {
            foreach (var dialog in Dialogs)
                yield return dialog;
        }

        public IEnumerable<ConversationStructure>? GetConversations()
        {
            return Conversations;
        }

        public IEnumerable<ConversationStructure>? GetDLCConversations()
        {
            return DLCConversations;
        }

        public IEnumerable<ComplexAnimStructure>? GetComplexAnimStructures()
        {
            return ComplexAnims;
        }

        public IEnumerable<Cinematic>? GetCinematics()
        {
            return Cinematics;
        }

        public IEnumerable<Cinematic>? GetDLCCinematics()
        {
            return DLCCinematics;
        }

        public IEnumerable<RandomText>? GetRandomTexts()
        {
            return RandomTexts;
        }
    }
}
