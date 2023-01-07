namespace MassTransit.NewIdFormatters
{
    using System.Runtime.CompilerServices;
    using System;
#if NET6_0_OR_GREATER
    using System.Runtime.Intrinsics.X86;
    using System.Runtime.Intrinsics;
#endif


    public class ZBase32Formatter : INewIdFormatter
    {
        // taken from analysis done at http://philzimmermann.com/docs/human-oriented-base-32-encoding.txt
        const string LowerCaseChars = "ybndrfg8ejkmcpqxot1uwisza345h769";
        const string UpperCaseChars = "YBNDRFG8EJKMCPQXOT1UWISZA345H769";

        readonly string _chars;
        readonly bool _isUpper;

        public ZBase32Formatter(bool upperCase = false)
        {
            _chars = upperCase ? UpperCaseChars : LowerCaseChars;
            _isUpper = upperCase;
        }

        public static readonly INewIdFormatter LowerCase = new ZBase32Formatter();

        public unsafe string Format(in byte[] bytes)
        {
#if NET6_0_OR_GREATER
            if (Avx2.IsSupported)
            {
                return string.Create(26, (bytes, _isUpper), (span, state) =>
                {
                    var (bytes, isUpperCase) = state;

                    EncodeKnownCase(bytes, span, isUpperCase);
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
        private static void EncodeKnownCase(ReadOnlySpan<byte> source, Span<char> destination, bool isUpperCase)
        {
        #region lut
            var upperCaseLow = Vector256.Create((byte)'Y', (byte)'B', (byte)'N', (byte)'D', (byte)'R', (byte)'F', (byte)'G', (byte)'8', (byte)'E', (byte)'J', (byte)'K', (byte)'M', (byte)'C', (byte)'P', (byte)'Q', (byte)'X', (byte)'Y', (byte)'B', (byte)'N', (byte)'D', (byte)'R', (byte)'F', (byte)'G', (byte)'8', (byte)'E', (byte)'J', (byte)'K', (byte)'M', (byte)'C', (byte)'P', (byte)'Q', (byte)'X');

            var upperCaseHigh = Vector256.Create((byte)'O', (byte)'T', (byte)'1', (byte)'U', (byte)'W', (byte)'I', (byte)'S', (byte)'Z', (byte)'A', (byte)'3', (byte)'4', (byte)'5', (byte)'H', (byte)'7', (byte)'6', (byte)'9', (byte)'O', (byte)'T', (byte)'1', (byte)'U', (byte)'W', (byte)'I', (byte)'S', (byte)'Z', (byte)'A', (byte)'3', (byte)'4', (byte)'5', (byte)'H', (byte)'7', (byte)'6', (byte)'9');

            var lowerCaseLow = Vector256.Create((byte)'y', (byte)'b', (byte)'n', (byte)'d', (byte)'r', (byte)'f', (byte)'g', (byte)'8', (byte)'e', (byte)'j', (byte)'k', (byte)'m', (byte)'c', (byte)'p', (byte)'q', (byte)'x', (byte)'y', (byte)'b', (byte)'n', (byte)'d', (byte)'r', (byte)'f', (byte)'g', (byte)'8', (byte)'e', (byte)'j', (byte)'k', (byte)'m', (byte)'c', (byte)'p', (byte)'q', (byte)'x');

            var lowerCaseHigh = Vector256.Create((byte)'o', (byte)'t', (byte)'1', (byte)'u', (byte)'w', (byte)'i', (byte)'s', (byte)'z', (byte)'a', (byte)'3', (byte)'4', (byte)'5', (byte)'h', (byte)'7', (byte)'6', (byte)'9', (byte)'o', (byte)'t', (byte)'1', (byte)'u', (byte)'w', (byte)'i', (byte)'s', (byte)'z', (byte)'a', (byte)'3', (byte)'4', (byte)'5', (byte)'h', (byte)'7', (byte)'6', (byte)'9');
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
