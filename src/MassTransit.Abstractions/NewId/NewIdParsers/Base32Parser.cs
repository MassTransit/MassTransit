namespace MassTransit.NewIdParsers
{
    using System;
    using System.Threading;


    public class Base32Parser :
        INewIdParser
    {
        const string ConvertChars = "abcdefghijklmnopqrstuvwxyz234567ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        const string HexChars = "0123456789ABCDEF";
        const string InvalidInputString = "The input string contains invalid characters";

        static readonly ThreadLocal<char[]> _buffer = new ThreadLocal<char[]>(() => new char[32]);
        readonly string _chars;

        public Base32Parser()
            :
            this(ConvertChars)
        {
        }

        public Base32Parser(in string chars)
        {
            if (chars.Length % 32 != 0)
                throw new ArgumentException("The characters must be a multiple of 32", nameof(chars));

            _chars = chars;
        }

        public NewId Parse(in string text)
        {
            if (text.Length != 26)
                throw new ArgumentException("The input string must be 26 characters", nameof(text));

            var buffer = _buffer.Value;

            var bufferOffset = 0;
            var offset = 0;
            long number;
            for (var i = 0; i < 6; ++i)
            {
                number = 0;
                for (var j = 0; j < 4; j++)
                {
                    var index = _chars.IndexOf(text[offset + j]);
                    if (index < 0)
                        throw new ArgumentException(InvalidInputString);

                    number = number * 32 + index % 32;
                }

                ConvertLongToBase16(buffer, bufferOffset, number, 5);

                offset += 4;
                bufferOffset += 5;
            }

            number = 0;
            for (var j = 0; j < 2; j++)
            {
                var index = _chars.IndexOf(text[offset + j]);
                if (index < 0)
                    throw new ArgumentException(InvalidInputString);

                number = number * 32 + index % 32;
            }

            ConvertLongToBase16(buffer, bufferOffset, number, 2);

            return new NewId(new string(buffer, 0, 32));
        }

        static void ConvertLongToBase16(in char[] buffer, int offset, long value, int count)
        {
            for (var i = count - 1; i >= 0; i--)
            {
                var index = (int)(value % 16);
                buffer[offset + i] = HexChars[index];
                value /= 16;
            }
        }
    }
}
