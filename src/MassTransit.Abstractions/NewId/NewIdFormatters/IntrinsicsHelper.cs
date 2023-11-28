#if NET6_0_OR_GREATER
namespace MassTransit.NewIdFormatters
{
    using System.Diagnostics;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Intrinsics;
    using System.Runtime.Intrinsics.X86;


// We need to target netstandard2.0, so keep using ref parameter.
// CS9191: The 'ref' modifier for argument 2 corresponding to 'in' parameter is equivalent to 'in'. Consider using 'in' instead.
#pragma warning disable CS9191

    internal static class IntrinsicsHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Vector128ToCharUtf16(Vector128<byte> value, Span<byte> destination)
        {
            var widened = Avx2.ConvertToVector256Int16(value).AsByte();
            MemoryMarshal.Write(destination, ref widened);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Vector256ToCharUtf16(Vector256<byte> vec, Span<byte> destination)
        {
            Vector256<byte> zero = Vector256<byte>.Zero;
            Vector256<byte> c0 = Avx2.UnpackLow(vec, zero);
            Vector256<byte> c1 = Avx2.UnpackHigh(vec, zero);

            Vector256<byte> t0 = Avx2.Permute2x128(c0, c1, 0b_10_00_00);
            Vector256<byte> t1 = Avx2.Permute2x128(c0, c1, 0b_11_00_01);

            MemoryMarshal.Write(destination, ref t0);
            MemoryMarshal.Write(destination[Vector256<byte>.Count..], ref t1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<byte> GetByteLutFromChar(Vector256<byte> value)
        {
            var shuffled = Avx2.Shuffle(value, Vector256.Create((byte)0, 2, 4, 6, 8, 10, 12, 14, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0, 2, 4, 6, 8, 10, 12, 14));
            return Avx2.Permute4x64(shuffled.AsDouble(), 0b_11_00_11_00).AsByte();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<byte> EncodeBytesHex(Vector128<byte> vec, bool isUpper)
        {
            var lowerCharSet = Vector256.Create((byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f');

            var upperCharSet = Vector256.Create((byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F');

            var x = Avx2.ConvertToVector256Int16(vec);
            var highNibble = Avx2.ShiftLeftLogical(x, 8);
            var lowNibble = Avx2.ShiftRightLogical(x, 4);
            var values = Avx2.And(Avx2.Or(highNibble, lowNibble).AsByte(), Vector256.Create((byte)0x0F));
            return Avx2.Shuffle(isUpper ? upperCharSet : lowerCharSet, values);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EncodeBase32(ReadOnlySpan<byte> span, Span<char> output, Vector256<byte> lowLut, Vector256<byte> upperLut)
        {
            Debug.Assert(span.Length >= 16);
            Debug.Assert(output.Length >= 26);

            Span<byte> buffer = stackalloc byte[64];
            span.CopyTo(buffer[6..]);

            var inputVector = MemoryMarshal.Read<Vector256<byte>>(buffer);
            var splitVector = Split130Bits5x26(inputVector);
            var encodedVector = EncodeValuesBase32(splitVector, lowLut, upperLut);

            Vector256ToCharUtf16(encodedVector, buffer);

            var byteSpan = MemoryMarshal.Cast<char, byte>(output);
            buffer[..52].CopyTo(byteSpan);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<byte> Split130Bits5x26(Vector256<byte> input)
        {
            var splitShuffle = Vector256.Create((byte)
                0x07, 0x06, 0x08, 0x07, 0x09, 0x08, 0x0A, 0x09,
                0x0C, 0x0B, 0x0D, 0x0C, 0x0E, 0x0D, 0x0F, 0x0E,
                0x01, 0x00, 0x02, 0x01, 0x03, 0x02, 0x04, 0x03,
                0x05, 0x06,
                0x07, 0x06, 0x08, 0x07, 0x09, 0x08);

            var splitM1 = Vector256.Create((ulong)0x0800_0200_0080_0020, 0x0800_0200_0080_0020, 0x0800_0200_0080_0020, 0x0800).AsUInt16();
            var splitM2 = Vector256.Create((ulong)0x0100_0040_0010_0004, 0x0100_0040_0010_0004, 0x0100_0040_0010_0004, 0x0100).AsInt16();

            var maskM1 = Vector256.Create((ushort)0x00_1F);
            var maskM2 = Vector256.Create((ushort)0x1F_00);

            var x1 = Avx2.Shuffle(input, splitShuffle).AsUInt16();
            var x2 = Avx2.MultiplyHigh(x1, splitM1);
            var x3 = Avx2.MultiplyLow(x1.AsInt16(), splitM2).AsUInt16();
            var x4 = Avx2.And(x2, maskM1);
            var x5 = Avx2.And(x3, maskM2);

            var x6 = Avx2.Or(x4, x5).AsByte();
            return x6;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<byte> EncodeValuesBase32(Vector256<byte> x, Vector256<byte> lower, Vector256<byte> upper)
        {
            var mask16 = Vector256.Create((sbyte)0x10);
            var x1 = Avx2.Shuffle(lower, x);
            var x2 = Avx2.Shuffle(upper, x);
            var x3 = Avx2.CompareGreaterThan(mask16, x.AsSByte()).AsByte();

            return Avx2.BlendVariable(x2, x1, x3);
        }
    }
}
#endif
