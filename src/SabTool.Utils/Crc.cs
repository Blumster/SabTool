using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Utils
{
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

        public string GetStringOrHexString()
        {
            var stringValue = GetString();

            return string.IsNullOrEmpty(stringValue) ? $"0x{Value:X8}" : stringValue;
        }

        public static bool operator==(Crc left, Crc right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, null) && ReferenceEquals(right, null);

            return left.Value == right.Value;
        }

        public static bool operator!=(Crc left, Crc right)
        {
            return !(left == right);
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

        public static implicit operator Crc(uint val)
        {
            return new Crc(val);
        }
    }
}
