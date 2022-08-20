using System.Collections.Generic;

namespace SabTool.Data.Misc;

public class Hei5Container
{
    public List<Hei1> Hei1s { get; } = new();
    public int Unk40 { get; set; }
    public int Unk44 { get; set; }
    public float Unk48 { get; set; }
    public float Unk4C { get; set; }
    public float Unk50 { get; set; }
    public float Unk54 { get; set; }
}

public class Hei1
{
    public int Unk32 { get; set; }
    public int Unk36 { get; set; }
    public float Unk40 { get; set; }
    public float Unk44 { get; set; }
    public byte[] Unk48 { get; set; }
    public int Unk0 { get; set; }
    public int Unk4 { get; set; }
    public float Unk8 { get; set; }
    public float Unk16 { get; set; }
    public float Unk20 { get; set; }
    public float Unk24 { get; set; }
}
