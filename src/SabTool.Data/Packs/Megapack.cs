using System;
using System.Collections.Generic;

namespace SabTool.Data.Packs;

using SabTool.Utils;

public class Megapack
{
    public Dictionary<Crc, FileEntry> FileEntries { get; set; } = new();
    public uint FileCount { get; set; }
    public Tuple<Crc, Crc>[] BlockPathToNameCrcs { get; set; }
}

public enum FileEntryType
{
    Mesh = 0,
    Texture = 1,
    Physics = 2,
    PathGraph = 3,
    AIFence = 4,
    Unk5 = 5,
    SoundBank = 6,
    FlashMovie = 7,
    WSD = 8
}

public enum FileReadMethod : uint
{
    Mesh = 0xFE5E3A56u,
    Texture = 0xA40D777Du,
    Physics = 0x4445EA18u,
    PathGraph = 0xE1087B27u,
    AIFence = 0xD3098461u,
    SoundBank = 0xDD62BA1Au,
    FlashMovie = 0xB5D2FE96u,
    WSD = 0x9AB5A351u,
    Unk5 = 0x00000001u,
    Typ2 = 0x00000002u, // SBLA (not yet finished)
    Typ3 = 0x00000003u  // SBLA (finished)
}

public class FileEntry
{
    public Crc Path { get; set; }
    public Crc Name { get; set; }
    public int Size { get; set; }
    public long Offset { get; set; }

    public string ToPrettyString(int namePad = 50)
    {
        return $"FileEntry(Size: 0x{Size:X8}, offset: 0x{Offset:X8}, Name: {Name.ToString().PadRight(namePad)}, Path: {Path})";
    }

    public override string ToString()
    {
        return ToPrettyString(0);
    }
}
