namespace SabTool.Data.Cinematics;

using SabTool.Data.Cinematics.CinematicElements;
using SabTool.Utils;

public class Cinematic
{
    public struct UnkSub
    {
        public Crc A;
        public int B; 
        public byte C;
    }

    public float Field18 { get; set; }
    public Crc Name { get; set; }
    public int Field4C { get; set; }
    public float Duration { get; set; }
    public float Field54 { get; set; }
    public float Field58 { get; set; }
    public float Field5C { get; set; }
    public float Field60 { get; set; }
    public UnkSub[] UnkSubs { get; set; }
    public int Field194 { get; set; }
    public float Field198 { get; set; }
    public float Field19C { get; set; }
    public float Field1A0 { get; set; }
    public float Field1A4 { get; set; }
    public float Field1A8 { get; set; }
    public byte Field1AC { get; set; }
    public List<CinemaElement> Elements { get; set; } = new();
    public int Field1E8 { get; set; }
    public byte UnkSubCount { get; set; }
    public byte Field1F4 { get; set; }
    public byte Field1F5 { get; set; }
    public byte Field1F6 { get; set; }
    public byte Field1F7 { get; set; }

    public Cinematic(Crc name)
    {
        Name = name;
    }
}
