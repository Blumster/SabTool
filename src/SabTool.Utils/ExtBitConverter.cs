using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SabTool.Utils
{
    public static class ExtBitConverter
    {
        /// <summary>
        /// Returns the specified half-precision floating point value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        public static unsafe byte[] GetBytes(Half value)
        {
            byte[] bytes = new byte[sizeof(Half)];
            Unsafe.As<byte, Half>(ref bytes[0]) = value;
            return bytes;
        }

        /// <summary>
        /// Converts a half-precision floating-point value into a span of bytes.
        /// </summary>
        /// <param name="destination">When this method returns, the bytes representing the converted half-precision floating-point value.</param>
        /// <param name="value">The half-precision floating-point value to convert.</param>
        /// <returns><see langword="true"/> if the conversion was successful; <see langword="false"/> otherwise.</returns>
        public static unsafe bool TryWriteBytes(Span<byte> destination, Half value)
        {
            if (destination.Length < sizeof(Half))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            return true;
        }

        /// <summary>
        /// Returns a half-precision floating point number converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
        /// <returns>A half-precision floating point number signed integer formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="startIndex"/> equals the length of <paramref name="value"/> minus 1.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 1.</exception>
        public static Half ToHalf(byte[] value, int startIndex) => Int16BitsToHalf(BitConverter.ToInt16(value, startIndex));

        /// <summary>
        /// Converts a read-only byte span into a half-precision floating-point value.
        /// </summary>
        /// <param name="value">A read-only span containing the bytes to convert.</param>
        /// <returns>A half-precision floating-point value representing the converted bytes.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The length of <paramref name="value"/> is less than 2.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Half ToHalf(ReadOnlySpan<byte> value)
        {
            if (value.Length < sizeof(Half))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<Half>(ref MemoryMarshal.GetReference(value));
        }

        /// <summary>
        /// Converts the specified half-precision floating point number to a 16-bit signed integer.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>A 16-bit signed integer whose bits are identical to <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe short HalfToInt16Bits(Half value)
        {
            return *((short*)&value);
        }

        /// <summary>
        /// Converts the specified 16-bit signed integer to a half-precision floating point number.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>A half-precision floating point number whose bits are identical to <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Half Int16BitsToHalf(short value)
        {
            return *(Half*)&value;
        }
    }
}
