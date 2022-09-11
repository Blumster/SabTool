using System.Collections.Generic;
using System.Numerics;

namespace SabTool.Data.Misc;

using SabTool.Utils;

public class RailwaySpline
{
    public string Name { get; set; }
    public Crc NameCrc { get; set; }
    public float Distance { get; set; }
    public List<RailwaySplineNode> Nodes { get; set; } = new();
}

public class RailwaySplineNode
{
    public string Name { get; set; }
    public Crc NameCrc { get; set; }
    public float StartPos { get; set; }
    public float Distance { get; set; }
    public Vector3 Pos { get; set; }
    public Vector3 Influence { get; set; }
    public Quaternion Tangent { get; set; }
    // IsStart/IsEnd/IsStation only have 0 and 1 as values in France.railway
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }
    public bool IsStation { get; set; }
    public float MaxSpeed { get; set; }
    public float StationWaitTime { get; set; }
    public Crc TrainListBlueprintCrc { get; set; }
    public List<RailwaySplineNodeAttachment> Attachments { get; set; } = new();
}

public class RailwaySplineNodeAttachment
{
    public Crc SplineNameCrc { get; set; }
    public Crc NodeNameCrc { get; set; }
}
