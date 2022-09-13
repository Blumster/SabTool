namespace SabTool.Data.Sounds;

using SabTool.Utils;

public sealed class SoundStream
{
    public Crc Id { get; set; }
    public byte[] Data { get; set; }
}
