namespace SabTool.Data.Sounds;

public class WWiseIDTable
{
    public const int Version = 5;

    public List<Entry1> Entry1s { get; } = new();
    public List<Entry2> Entry2s { get; } = new();
    public List<Entry3> Entry3s { get; } = new();
    public List<Entry3> Entry4s { get; } = new();
    public List<Entry3> Entry5s { get; } = new();
    public List<Entry3> Entry6s { get; } = new();
    public List<Entry3> Entry7s { get; } = new();
    public List<Entry3> Entry8s { get; } = new();
    public List<Entry3> Entry9s { get; } = new();
    public List<Entry3> Entry10s { get; } = new();
    public List<string> Params {  get; } = new();

    public class Entry1
    {
        public string Name { get; set; }
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public string[] Names { get; set; }
    }

    public class Entry2
    {
        public Crc Name { get; set; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
    }

    public class Entry3
    {
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
    }
}

