namespace MassTransit.NewIdFormatters
{
    using System;
#if NET6_0_OR_GREATER
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;
    using System.Runtime.Intrinsics;
    using System.Runtime.Intrinsics.X86;
#endif


// We need to target netstandard2.0, so keep using ref parameter.
// CS9191: The 'ref' modifier for argument 2 corresponding to 'in' parameter is equivalent to 'in'. Consider using 'in' instead.
#pragma warning disable CS9191

    public class DashedHexFormatter :
        INewIdFormatter
    {
        readonly uint _alpha;
        readonly int _length;
        readonly char _prefix;
        readonly char _suffix;
        const uint LowerCaseUInt = 0x2020U;

        public DashedHexFormatter(char prefix = '\0', char suffix = '\0', bool upperCase = false)
        {
            if (prefix == '\0' || suffix == '\0')
                _length = 36;
            else
            {
                _prefix = prefix;
                _suffix = suffix;
                _length = 38;
            }

            _alpha = upperCase ? 0 : LowerCaseUInt;
        }

        public unsafe string Format(in byte[] bytes)
        {
#if NET6_0_OR_GREATER
            if (Avx2.IsSupported && BitConverter.IsLittleEndian)
            {
                var isUpperCase = _alpha != LowerCaseUInt;
                return string.Create(_length, (bytes, isUpperCase, _prefix, _suffix), (span, state) =>
                {
                    EncodeVector256(span, state);
                });
            }
#endif
            var result = stackalloc char[_length];

            var i = 0;
            var offset = 0;
            if (_prefix != '\0')
                result[offset++] = _prefix;
            for (; i < 4; i++)
            {
                var value = bytes[i];
                HexToChar(value, result, offset, _alpha);
                offset+=2;
            }

            result[offset++] = '-';
            for (; i < 6; i++)
            {
                var value = bytes[i];
                HexToChar(value, result, offset, _alpha);
                offset+=2;
            }

            result[offset++] = '-';
            for (; i < 8; i++)
            {
                var value = bytes[i];
                HexToChar(value, result, offset, _alpha);
                offset+=2;
            }

            result[offset++] = '-';
            for (; i < 10; i++)
            {
                var value = bytes[i];
                HexToChar(value, result, offset, _alpha);
                offset+=2;
            }

            result[offset++] = '-';
            for (; i < 16; i++)
            {
                var value = bytes[i];
                HexToChar(value, result, offset, _alpha);
                offset+=2;
            }

            if (_suffix != '\0')
                result[offset] = _suffix;

            return new string(result, 0, _length);
        }

#if NET6_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EncodeVector256(Span<char> span, (byte[] bytes, bool, char _prefix, char _suffix) state)
        {
            var (bytes, isUpper, prefix, suffix) = state;
            var swizzle = Vector256.Create((byte)
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                0x80, 0x08, 0x09, 0x0a, 0x0b, 0x80, 0x0c, 0x0d,
                0x80, 0x80, 0x80, 0x00, 0x01, 0x02, 0x03, 0x80,
                0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b);

            var dash = Vector256.Create((byte)
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x2d, 0x00, 0x00, 0x00, 0x00, 0x2d, 0x00, 0x00,
                0x00, 0x00, 0x2d, 0x00, 0x00, 0x00, 0x00, 0x2d,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);

            var inputVec = MemoryMarshal.Read<Vector128<byte>>(bytes);
            var hexVec = IntrinsicsHelper.EncodeBytesHex(inputVec, isUpper);

            var a1 = Avx2.Shuffle(hexVec, swizzle);
            var a2 = Avx2.Or(a1, dash);

            if (span.Length == 38)
            {
                span[0] = prefix;
                span[^1] = suffix;
            }

            var charSpan = span.Length == 38 ? span[1..^1] : span;
            var spanBytes = MemoryMarshal.Cast<char, byte>(charSpan);
            IntrinsicsHelper.Vector256ToCharUtf16(a2, spanBytes);

            var shuffleSpare = Avx2.Shuffle(hexVec, Vector256.Create((byte)14, 0xFF, 15, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 12, 0xFF, 13, 0xFF, 14, 0xFF, 15, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF));
            var byte1415 = shuffleSpare.AsUInt32().GetElement(0);
            MemoryMarshal.Write(spanBytes[32..], ref byte1415);

            var final = shuffleSpare.AsUInt64().GetElement(2);
            MemoryMarshal.Write(spanBytes[64..], ref final);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        static unsafe void HexToChar(byte value, char* buffer, int startingIndex, uint casing)
        {
            uint difference = (((uint)value & 0xF0U) << 4) + ((uint)value & 0x0FU) - 0x8989U;
            uint packedResult = ((((uint)(-(int)difference) & 0x7070U) >> 4) + difference + 0xB9B9U) | (uint)casing;

            buffer[startingIndex + 1] = (char)(packedResult & 0xFF);
            buffer[startingIndex] = (char)(packedResult >> 8);
        }
    }
}
