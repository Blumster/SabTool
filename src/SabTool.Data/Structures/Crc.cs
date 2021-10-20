namespace SabTool.Data.Structures
{
    using Utils;

    public class Crc
    {
        public uint Value { get; }

        public Crc(uint value)
        {
            Value = value;
        }

        public string GetString()
        {
            return Hash.HashToString(Value);
        }

        public override string ToString()
        {
            return $"0x{Value:X8} ({Hash.HashToString(Value)})";
        }

        public override bool Equals(object obj)
        {
            if (obj is Crc other)
                return other.Value == Value;

            return false;
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }
    }
}
