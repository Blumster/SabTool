﻿namespace SabTool.Data.Cinematics.CinematicElements;

using SabTool.Utils;

public sealed class CinemaObjectMotionBlur : CinemaElement
{
    public float EndTime { get; set; }
    public Crc UnkCrc { get; set; }
    public byte Flags { get; set; }
}
