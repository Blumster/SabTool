namespace SabTool.Data.Sounds;

public sealed class SoundPack
{
    public record Entry(uint Id, int Size, int Offset);

    public string Tag { get; set; }
    public int Align { get; set; }
    public int Unk1 { get; set; }
    public int NumBanks { get; set; }
    public int BankEntriesOffset { get; set; }
    public int NumStreams { get; set; }
    public int StreamEntriesOffset { get; set; }

    public string FilePath { get; set; }
    public List<SoundBank> SoundBanks { get; set; } = new();
    public List<SoundStream> SoundStreams { get; set; } = new();
}
