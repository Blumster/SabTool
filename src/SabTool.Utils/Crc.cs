namespace SabTool.Utils;

public sealed class Crc
{
    public uint Value { get; }

    public Crc(uint value) => Value = value;

    public string GetString() => Hash.HashToString(Value);
    public string GetHexString() => $"0x{Value:X8}";

    public string GetStringOrHexString()
    {
        var stringValue = GetString();

        return string.IsNullOrEmpty(stringValue) ? $"0x{Value:X8}" : stringValue;
    }

    public static bool operator==(Crc left, Crc right)
    {
        if (left is null || right is null)
            return left is null && right is null;

        return left.Value == right.Value;
    }

    public static bool operator!=(Crc left, Crc right) => !(left == right);

    public override string ToString() => $"0x{Value:X8} ({Hash.HashToString(Value)})";
    public override bool Equals(object obj) => obj is Crc other && other.Value == Value;
    public override int GetHashCode() => (int)Value;

    public static implicit operator Crc(uint val) => new(val);
}
