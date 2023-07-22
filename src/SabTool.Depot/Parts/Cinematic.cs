
using SabTool.Data.Cinematics;
using SabTool.Serializers.Cinematics;

namespace SabTool.Depot;
public sealed partial class ResourceDepot
{
    private const string DialogTextFileName = "GameText.dlg";

    private Dictionary<string, Dialog> Dialogs { get; set; } = new();
    private List<ConversationStructure>? Conversations { get; set; }
    private List<ConversationStructure>? DLCConversations { get; set; }
    private List<ComplexAnimStructure>? ComplexAnims { get; set; }
    private List<Cinematic>? Cinematics { get; set; }
    private List<Cinematic>? DLCCinematics { get; set; }
    private List<RandomText>? RandomTexts { get; set; }

    private bool LoadCinematics()
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

        string dialogPath = GetGamePath(@"Cinematics\Dialog");

        foreach (string folder in Directory.GetDirectories(dialogPath))
        {
            string dialogFilePath = Path.Combine(folder, DialogTextFileName);
            if (!File.Exists(dialogFilePath))
                continue;

            string language = new DirectoryInfo(folder).Name;

            using FileStream fs = new(dialogFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            Dialogs.Add(language, DialogSerializer.DeserialzieRaw(fs));
        }

        Console.WriteLine("  Dialogs loaded!");
    }

    private void LoadConversations()
    {
        Console.WriteLine("  Loading Conversations...");

        using MemoryStream conversationsStream = GetLooseFile(@"Cinematics\Conversations\Conversations.cnvpack") ?? throw new Exception("Conversations cnvpack is missing from the loosefiles!");

        Conversations = ConversationsSerializer.DeserializeRaw(conversationsStream);

        Console.WriteLine("  Conversations loaded!");
    }

    private void LoadDLCConversations()
    {
        Console.WriteLine("  Loading DLC Conversations...");

        string conversationsFilePath = GetGamePath(@"DLC\01\Cinematics\Conversations\Conversations.cnvpack");

        using FileStream fs = new(conversationsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        DLCConversations = ConversationsSerializer.DeserializeRaw(fs);

        Console.WriteLine("  DLC Conversations loaded!");
    }

    private void LoadComplexAnims()
    {
        Console.WriteLine("  Loading ComplexAnims...");

        string complexAnimsFilePath = GetGamePath(@"Cinematics\ComplexAnimations\ComplexAnims.cxa");

        using FileStream fs = new(complexAnimsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        ComplexAnims = ComplexAnimsSerializer.DeserializeRaw(fs);

        Console.WriteLine("  ComplexAnims loaded!");
    }

    private void LoadCinematicsFile()
    {
        Console.WriteLine("  Loading Cinematics...");

        using MemoryStream cinematicsStream = GetLooseFile(@"Cinematics\Cinematics.cinpack") ?? throw new Exception("Cinematics cinpack is missing from the loose files!");

        Cinematics = CinematicsSerializer.DeserializeRaw(cinematicsStream);

        Console.WriteLine("  Cinematics loaded!");
    }

    private void LoadDLCCinematicsFile()
    {
        Console.WriteLine("  Loading DLC Cinematics...");

        string complexAnimsFilePath = GetGamePath(@"DLC\01\Cinematics\Cinematics.cinpack");

        using FileStream fs = new(complexAnimsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        DLCCinematics = CinematicsSerializer.DeserializeRaw(fs);

        Console.WriteLine("  DLC Cinematics loaded!");
    }

    private void LoadRandomTexts()
    {
        Console.WriteLine("  Loading Random Texts...");

        string randomTextsFilePath = GetGamePath(@"Cinematics\Dialog\Random\RandomText.rnd");

        using FileStream fs = new(randomTextsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        RandomTexts = RandomTextSerializer.DeserializeRaw(fs);

        Console.WriteLine("  Random Texts loaded!");
    }

    public IEnumerable<KeyValuePair<string, Dialog>> GetDialogs()
    {
        if (!IsResourceLoaded(Resource.Cinematics))
            _ = Load(Resource.Cinematics);

        foreach (KeyValuePair<string, Dialog> dialog in Dialogs)
            yield return dialog;
    }

    public IEnumerable<ConversationStructure> GetConversations()
    {
        if (!IsResourceLoaded(Resource.Cinematics))
            _ = Load(Resource.Cinematics);

        return Conversations!;
    }

    public IEnumerable<ConversationStructure> GetDLCConversations()
    {
        if (!IsResourceLoaded(Resource.Cinematics))
            _ = Load(Resource.Cinematics);

        return DLCConversations!;
    }

    public IEnumerable<ComplexAnimStructure> GetComplexAnimStructures()
    {
        if (!IsResourceLoaded(Resource.Cinematics))
            _ = Load(Resource.Cinematics);

        return ComplexAnims!;
    }

    public IEnumerable<Cinematic> GetCinematics()
    {
        if (!IsResourceLoaded(Resource.Cinematics))
            _ = Load(Resource.Cinematics);

        return Cinematics!;
    }

    public IEnumerable<Cinematic> GetDLCCinematics()
    {
        if (!IsResourceLoaded(Resource.Cinematics))
            _ = Load(Resource.Cinematics);

        return DLCCinematics!;
    }

    public IEnumerable<RandomText> GetRandomTexts()
    {
        if (!IsResourceLoaded(Resource.Cinematics))
            _ = Load(Resource.Cinematics);

        return RandomTexts!;
    }
}
