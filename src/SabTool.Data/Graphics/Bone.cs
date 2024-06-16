namespace SabTool.Data.Graphics;

public sealed class Bone
{
    public static Bone DefaultBone { get; } = new Bone
    {
        CrcName = new Crc(0x204E3386u) // "DefaultBone"
    };

    public Crc DebugName { get; set; }
    public bool LockTranslation { get; set; }
    public Crc CrcName { get; set; }
    public short Flags { get; set; }
    public short Index { get; set; }
    public byte ExportFlags { get; set; }
    public Vector4 AABBOffset { get; set; }
    public Vector4 AABBSize { get; set; }

    public string DumpString(int indentCount = 0)
    {
        var sb = new StringBuilder();

        sb.Append(' ', indentCount).AppendLine($"{nameof(Bone)}()");
        sb.Append(' ', indentCount).AppendLine("{");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(DebugName)} = {DebugName}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(LockTranslation)} = {LockTranslation}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(CrcName)} = {CrcName}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Flags)} = 0x{Flags:X4}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(Index)} = {Index}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(ExportFlags)} = 0x{ExportFlags:X2}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(AABBOffset)} = {AABBOffset}");
        sb.Append(' ', indentCount + 2).AppendLine($"{nameof(AABBSize)} = {AABBSize}");
        sb.Append(' ', indentCount).AppendLine("}");

        return sb.ToString();
    }
}
