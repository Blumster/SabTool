namespace SabTool.Data.Packs;

using SabTool.Utils;

public class GlobalMap
{
    public uint NumTotalBlocks { get; set; }
    public uint NumStreamBlocks { get; set; }
    public StreamBlock[][] StreamBlockArray { get; set; } = new StreamBlock[2][];
    public Dictionary<Crc, StreamBlock> DynamicBlocks { get; } = new();
    public Dictionary<Crc, StreamBlock> StaticBlocks { get; } = new();

    public StreamBlock? GetDynamicBlock(Crc crc)
    {
        if (DynamicBlocks.TryGetValue(crc, out StreamBlock res))
            return res;

        return null;
    }

    public StreamBlock? GetStaticBlock(Crc crc)
    {
        for (var i = 0; i < StreamBlockArray.Length; ++i)
        {
            for (var j = 0; j < StreamBlockArray[i].Length; ++j)
            {
                if (StreamBlockArray[i][j].Id == crc)
                {
                    return StreamBlockArray[i][j];
                }
            }
        }

        return null;
    }
}
