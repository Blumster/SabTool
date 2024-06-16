namespace SabTool.Data.Graphics;

public sealed class Bone
{
    public static Bone DefaultBone { get; } = new Bone
    {
        Crc = new Crc(0x204E3386u) // "DefaultBone"
    };

    public Crc UnkNamePtr { get; set; }
    public byte UnkByte { get; set; }
    public Crc Crc { get; set; }
    public short Flags { get; set; }
    public short Index { get; set; }
    public byte UnkFlags { get; set; }
    public Vector4 AABBOffset { get; set; }
    public Vector4 AABBSize { get; set; }

    public string DumpString(int indentCount = 0)
    {
        var sb = new StringBuilder();

        sb.Append(' ', indentCount).AppendLine($"{nameof(Bone)}()");
        sb.Append(' ', indentCount).AppendLine("{");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(UnkNamePtr)} = {UnkNamePtr}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(UnkByte)} = {UnkByte}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Crc)} = {Crc}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Flags)} = 0x{Flags:X4}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Index)} = {Index}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(UnkFlags)} = 0x{UnkFlags:X2}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(AABBOffset)} = {AABBOffset}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(AABBSize)} = {AABBSize}");
        sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
