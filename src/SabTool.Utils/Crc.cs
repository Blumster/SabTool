namespace SabTool.Utils;

public sealed class Crc
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

    public string GetHexString()
    {
        return $"0x{Value:X8}";
    }

    public string GetStringOrHexString()
    {
        string stringValue = GetString();

        return string.IsNullOrEmpty(stringValue) ? $"0x{Value:X8}" : stringValue;
    }

    public static bool operator ==(Crc left, Crc right)
    {
        return left is null || right is null ? left is null && right is null : left.Value == right.Value;
    }

    public static bool operator !=(Crc left, Crc right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"0x{Value:X8} ({Hash.HashToString(Value)})";
    }

    public override bool Equals(object obj)
    {
        return obj is Crc other && other.Value == Value;
    }

    public override int GetHashCode()
    {
        return (int)Value;
    }

    public static implicit operator Crc(uint val)
    {
        return new Crc(val);
    }
}
