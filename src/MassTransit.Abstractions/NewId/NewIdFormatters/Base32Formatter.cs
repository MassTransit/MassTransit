namespace MassTransit.NewIdFormatters
{
    using System;
#if NET6_0_OR_GREATER
    using System.Runtime.InteropServices;
    using System.Runtime.Intrinsics;
    using System.Runtime.Intrinsics.X86;
    using System.Runtime.CompilerServices;
#endif


    public class Base32Formatter :
        INewIdFormatter
    {
        const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz234567";
        const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        readonly string _chars;
        readonly bool _isUpperCase;
#if NET6_0_OR_GREATER
        readonly bool _isCustom;
        readonly Vector256<byte> _lower;
        readonly Vector256<byte> _upper;
#endif
        public Base32Formatter(bool upperCase = false)
        {
            _chars = upperCase ? UpperCaseChars : LowerCaseChars;
            _isUpperCase = upperCase;
        }

        public Base32Formatter(in string chars)
        {
            if (chars.Length != 32)
                throw new ArgumentException("The character string must be exactly 32 characters", nameof(chars));

            _chars = chars;

#if NET6_0_OR_GREATER
            if (Avx2.IsSupported && BitConverter.IsLittleEndian)
            {
                _isCustom = true;
                var bytes = MemoryMarshal.Cast<char, byte>(chars);
                var lower = MemoryMarshal.Read<Vector256<byte>>(bytes);
                var upper = MemoryMarshal.Read<Vector256<byte>>(bytes[32..]);

                _lower = IntrinsicsHelper.GetByteLutFromChar(lower);
                _upper = IntrinsicsHelper.GetByteLutFromChar(upper);
            }
#endif
        }

        public unsafe string Format(in byte[] bytes)
        {
#if NET6_0_OR_GREATER
            if (Avx2.IsSupported)
            {
                if (_isCustom)
                {
                    return string.Create(26, (bytes, _lower, _upper), (span, state) =>
                    {
                        var (bytes, lower, upper) = state;
                        IntrinsicsHelper.EncodeBase32(bytes, span, lower, upper);
                    });
                }
                return string.Create(26, (bytes, _isUpperCase), (span, state) =>
                {
                    var (bytes, isUpperCase) = state;
                    EncodeKnown(bytes, span, isUpperCase);
                });
            }
#endif
            var result = stackalloc char[26];

            var offset = 0;
            for (var i = 0; i < 3; i++)
            {
                var indexed = i * 5;
                long number = (bytes[indexed] << 12) | (bytes[indexed + 1] << 4) | (bytes[indexed + 2] >> 4);
                ConvertLongToBase32(result, offset, number, 4, _chars);

                offset += 4;

                number = ((bytes[indexed + 2] & 0xf) << 16) | (bytes[indexed + 3] << 8) | bytes[indexed + 4];
                ConvertLongToBase32(result, offset, number, 4, _chars);

                offset += 4;
            }

            ConvertLongToBase32(result, offset, bytes[15], 2, _chars);

            return new string(result, 0, 26);
        }

        static unsafe void ConvertLongToBase32(char* buffer, int offset, long value, int count, string chars)
        {
            for (var i = count - 1; i >= 0; i--)
            {
                //30, 26, 25, 7, 24, 31, 4, 10, 23, 1, 2, 9, 17, 11, 4, 23, 7, 9, 16, 16, 15, 8, 16, 19, 2, 4,
                var index = (int)(value % 32);
                buffer[offset + i] = chars[index];
                value /= 32;
            }
        }

#if NET6_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EncodeKnown(ReadOnlySpan<byte> source, Span<char> destination, bool isUpperCase)
        {
            #region lut
            var lowerCaseLow = Vector256.Create((byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g', (byte)'h', (byte)'i', (byte)'j', (byte)'k', (byte)'l', (byte)'m', (byte)'n', (byte)'o', (byte)'p', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g', (byte)'h', (byte)'i', (byte)'j', (byte)'k', (byte)'l', (byte)'m', (byte)'n', (byte)'o', (byte)'p');

            var lowerCaseHigh = Vector256.Create((byte)'q', (byte)'r', (byte)'s', (byte)'t', (byte)'u', (byte)'v', (byte)'w', (byte)'x', (byte)'y', (byte)'z', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'q', (byte)'r', (byte)'s', (byte)'t', (byte)'u', (byte)'v', (byte)'w', (byte)'x', (byte)'y', (byte)'z', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7');

            var upperCaseLow = Vector256.Create((byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G', (byte)'H', (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N', (byte)'O', (byte)'P', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G', (byte)'H', (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N', (byte)'O', (byte)'P');

            var upperCaseHigh = Vector256.Create((byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'W', (byte)'X', (byte)'Y', (byte)'Z', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'W', (byte)'X', (byte)'Y', (byte)'Z', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7');
            #endregion

            if (isUpperCase)
            {
                IntrinsicsHelper.EncodeBase32(source, destination, upperCaseLow, upperCaseHigh);
            }
            else
            {
                IntrinsicsHelper.EncodeBase32(source, destination, lowerCaseLow, lowerCaseHigh);
            }
        }
#endif
    }
}
