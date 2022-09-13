namespace SabTool.Data.Lua;

using SabTool.Utils;

public sealed class LuaPackage
{
    public List<Entry> Entries { get; } = new();

    public sealed class Entry
    {
        public Crc PathCRC { get; set; }
        public Crc NameCRC { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
        public int Size2 { get; set; }
        public bool IsModule { get; set; }
        public byte[] Data { get; set; }
    }
}
