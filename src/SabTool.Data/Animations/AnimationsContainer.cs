namespace SabTool.Data.Animations;

public sealed class AnimationsContainer
{
    public AnimationPack AnimationPack { get; set; }
    public AnimationHavokData HavokData { get; set; }
    public uint BlendBoneException { get; set; }
    public AnimationIntervalPack IntervalPack { get; set; }
    public AnimationSequencePack SequencePack { get; set; }
    public AnimationTransitionContainer TransitionContainer { get; set; }
    public EdgeData EdgeData { get; set; }
    public float[][][] DistData { get; set; }
    public AnimationBankPack BankPack { get; set; }
    public List<AnimationAddon1> Addon1 { get; } = new();
    public List<Crc> Alphas { get; } = new();

    public AnimationsContainer()
    {
        DistData = new float[3][][];

        for (var i = 0; i < 3; ++i)
        {
            DistData[i] = new float[4][];

            for (var j = 0; j < 4; ++j)
            {
                DistData[i][j] = new float[2];
            }
        }
    }
}

public sealed class EdgeData
{
    public Crc[] TransSequences { get; set; }
    public Crc[] ClamberSequences { get; set; }
}

public sealed class AnimationBankPack
{
    public List<AnimationBank> Banks { get; } = new();
}

public sealed class AnimationBank
{
    public Crc NameCrc { get; set; }
    public string Name { get; set; }
    public Crc ParentBank { get; set; }
    public List<AnimationBankOverride> Overrides { get; } = new();
}

public sealed class AnimationBankOverride
{
    public Crc From { get; set; }
    public Crc To { get; set; }
    public List<uint> Unks { get; } = new();
}

public sealed class AnimationAddon1
{
    public Crc Unk1 { get; set; }
    public Crc Unk2 { get; set; }
    public uint Unk3 { get; set; }
}
