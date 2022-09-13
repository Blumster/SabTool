﻿namespace SabTool.Data.Cinematics.CinematicElements;

using SabTool.Utils;

public sealed class CinemaSubtitle : CinemaElement
{
    public float EndTime { get; set; }
    public int UnkInt1 { get; set; }
    public int UnkInt2 { get; set; }
    public Crc UnkCrc { get; set; }
}
