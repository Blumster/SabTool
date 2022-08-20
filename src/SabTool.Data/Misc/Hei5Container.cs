using System.Collections.Generic;

namespace SabTool.Data.Misc;

public class Hei5Container
{
    public List<Hei1> Hei1s { get; } = new();
    public int MaxGridX { get; set; }
    public int MaxGridZ { get; set; }
    public float GridSize { get; set; }
    public float TopLeftX { get; set; }
    public float TopLeftY { get; set; }
    public float TopLeftZ { get; set; }
}

public class Hei1 // Height map?? (find the y coordinate for a given x and z coordinate)
{
    public int Unk32 { get; set; }
    public int Unk36 { get; set; }
    public float Unk40 { get; set; }
    public float Unk44 { get; set; }
    public float Unk44Calc { get; set; }
    public byte[] Unk48 { get; set; }
    public float StartPosX { get; set; }
    public float StartPosY { get; set; }
    public float StartPosZ { get; set; }
    public float EndPosX { get; set; }
    public float EndPosY { get; set; }
    public float EndPosZ { get; set; }
}
