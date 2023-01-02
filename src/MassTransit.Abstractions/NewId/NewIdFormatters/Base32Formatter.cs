namespace MassTransit.NewIdFormatters
{
    using System;
    using System.Runtime.CompilerServices;


    public class Base32Formatter :
        INewIdFormatter
    {
        const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz234567";
        const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        readonly string _chars;

        public Base32Formatter(bool upperCase = false)
        {
            _chars = upperCase ? UpperCaseChars : LowerCaseChars;
        }

        public Base32Formatter(in string chars)
        {
            if (chars.Length != 32)
                throw new ArgumentException("The character string must be exactly 32 characters", nameof(chars));

            _chars = chars;
        }

        public unsafe string Format(in byte[] bytes)
        {
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe void ConvertLongToBase32(in char* buffer, int offset, long value, int count, string chars)
        {
            for (var i = count - 1; i >= 0; i--)
            {
                var index = (int)(value % 32);
                buffer[offset + i] = chars[index];
                value /= 32;
            }
        }
    }
}
