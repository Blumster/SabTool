namespace SabTool.Data.Sounds;

using SabTool.Utils;

public sealed class SoundBank
{
    public Crc Id { get; set; }
    public byte[] Data { get; set; }
}
