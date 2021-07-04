namespace SabTool.Data.Misc
{
    using Utils;

    public record Crc(uint Value)
    {
        public override string ToString()
        {
            return $"0x{Value:X8} ({Hash.HashToString(Value)})";
        }
    }
}
