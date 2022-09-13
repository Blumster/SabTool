﻿namespace SabTool.Data.Cinematics.ComplexAnimationElements;

public sealed class TargetElement : ComplexAnimElement
{
    public bool UnkBool { get; set; }

    public TargetElement()
        : base(ElementType.Target)
    {
    }
}
