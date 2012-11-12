// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using NewIdProviders;


    /// <summary>
    /// A NewId is a type that fits into the same space as a Guid/Uuid/uniqueidentifier,
    /// but is guaranteed to be both unique and ordered, assuming it is generated using
    /// a single instance of the generator for each network address used.
    /// </summary>
    public struct NewId :
        IEquatable<NewId>,
        IComparable<NewId>,
        IComparable,
        IFormattable
    {
        const int LowercaseA = 0x61;
        public static readonly NewId Empty = new NewId(0, 0, 0, 0);

        static NewIdGenerator _generator;
        static ITickProvider _tickProvider;
        static IWorkerIdProvider _workerIdProvider;

        readonly Int32 _a;
        readonly Int32 _b;
        readonly Int32 _c;
        readonly Int32 _d;

        /// <summary>
        /// Creates a NewId using the specified byte array.
        /// </summary>
        /// <param name="bytes"></param>
        public NewId(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");
            if (bytes.Length != 16)
                throw new ArgumentException("Exactly 16 bytes expected", "bytes");

            ConvertBytes(bytes, out _a, out _b, out _c, out _d);
        }

        public NewId(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("A string is expected", "value");

            var guid = new Guid(value);

            byte[] bytes = guid.ToByteArray();

            ConvertBytes(bytes, out _a, out _b, out _c, out _d);
        }

        public NewId(int a, int b, int c, int d)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
        }

        public NewId(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
        {
            _a = (f << 24) | (g << 16) | (h << 8) | i;
            _b = (j << 24) | (k << 16) | (d << 8) | e;
            _c = (c << 16) | b;
            _d = a;
        }

        static NewIdGenerator Generator
        {
            get { return _generator ?? (_generator = new NewIdGenerator(TickProvider, WorkerIdProvider)); }
        }

        static IWorkerIdProvider WorkerIdProvider
        {
            get { return _workerIdProvider ?? (_workerIdProvider = new NetworkAddressWorkerIdProvider()); }
        }

        static ITickProvider TickProvider
        {
            get { return _tickProvider ?? (_tickProvider = new StopwatchTickProvider()); }
        }

        public DateTime Timestamp
        {
            get
            {
                long ticks = (long)(((ulong)_a) << 32 | (uint)_b);

                return new DateTime(ticks, DateTimeKind.Utc);
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (!(obj is NewId))
                throw new ArgumentException("Argument must be a NewId");

            return CompareTo((NewId)obj);
        }

        public int CompareTo(NewId other)
        {
            if (_a != other._a)
                return ((uint)_a < (uint)other._a) ? -1 : 1;
            if (_b != other._b)
                return ((uint)_b < (uint)other._b) ? -1 : 1;
            if (_c != other._c)
                return ((uint)_c < (uint)other._c) ? -1 : 1;
            if (_d != other._d)
                return ((uint)_d < (uint)other._d) ? -1 : 1;

            return 0;
        }

        public bool Equals(NewId other)
        {
            return other._a == _a && other._b == _b && other._c == _c && other._d == _d;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "D";

            bool sequential = false;
            if (format.Length == 2 && (format[1] == 'S' || format[1] == 's'))
                sequential = true;
            else if (format.Length != 1)
                throw new FormatException("The format string must be exactly one character or null");

            char formatCh = format[0];

            var result = new char[38];
            int offset = 0;
            int length = 38;

            bool dash = true;

            if (formatCh == 'D' || formatCh == 'd')
                length = 36;
            else if (formatCh == 'N' || formatCh == 'n')
            {
                length = 32;
                dash = false;
            }
            else if (formatCh == 'B' || formatCh == 'b')
            {
                result[offset++] = '{';
                result[37] = '}';
            }
            else if (formatCh == 'P' || formatCh == 'p')
            {
                result[offset++] = '(';
                result[37] = ')';
            }
            else
                throw new FormatException("The format string was not valid");

            if (sequential)
            {
                offset = TwoBytesToChars(result, offset, _a >> 24, _a >> 16, LowercaseA);
                offset = TwoBytesToChars(result, offset, _a >> 8, _a, LowercaseA);
                if (dash)
                    result[offset++] = '-';
                offset = TwoBytesToChars(result, offset, _b >> 24, _b >> 16, LowercaseA);
                if (dash)
                    result[offset++] = '-';
                offset = TwoBytesToChars(result, offset, _b >> 8, _b, LowercaseA);
                if (dash)
                    result[offset++] = '-';
                offset = TwoBytesToChars(result, offset, _c >> 24, _c >> 16, LowercaseA);
                if (dash)
                    result[offset++] = '-';
                offset = TwoBytesToChars(result, offset, _c >> 8, _c, LowercaseA);
                offset = TwoBytesToChars(result, offset, _d >> 24, _d >> 16, LowercaseA);
                TwoBytesToChars(result, offset, _d >> 8, _d, LowercaseA);
            }
            else
            {
                offset = TwoBytesToChars(result, offset, _d >> 24, _d >> 16, LowercaseA);
                offset = TwoBytesToChars(result, offset, _d >> 8, _d, LowercaseA);
                if (dash)
                    result[offset++] = '-';
                offset = TwoBytesToChars(result, offset, _c >> 8, _c, LowercaseA);
                if (dash)
                    result[offset++] = '-';
                offset = TwoBytesToChars(result, offset, _c >> 24, _c >> 16, LowercaseA);
                if (dash)
                    result[offset++] = '-';
                offset = TwoBytesToChars(result, offset, _b >> 8, _b, LowercaseA);
                if (dash)
                    result[offset++] = '-';
                offset = TwoBytesToChars(result, offset, _a >> 24, _a >> 16, LowercaseA);
                offset = TwoBytesToChars(result, offset, _a >> 8, _a, LowercaseA);
                TwoBytesToChars(result, offset, _b >> 24, _b >> 16, LowercaseA);
            }

            return new string(result, 0, length);
        }

        public Guid ToGuid()
        {
            int a = _d;
            var b = (short)_c;
            var c = (short)(_c >> 16);
            var d = (byte)(_b >> 8);
            var e = (byte)_b;
            var f = (byte)(_a >> 24);
            var g = (byte)(_a >> 16);
            var h = (byte)(_a >> 8);
            var i = (byte)_a;
            var j = (byte)(_b >> 24);
            var k = (byte)(_b >> 16);

            return new Guid(a, b, c, d, e, f, g, h, i, j, k);
        }

        public byte[] ToByteArray()
        {
            var bytes = new byte[16];

            bytes[0] = (byte)(_a);
            bytes[1] = (byte)(_a >> 8);
            bytes[2] = (byte)(_a >> 16);
            bytes[3] = (byte)(_a >> 24);
            bytes[4] = (byte)(_b >> 16);
            bytes[5] = (byte)(_b >> 24);
            bytes[6] = (byte)(_b);
            bytes[7] = (byte)(_b >> 8);
            bytes[8] = (byte)(_c >> 24);
            bytes[9] = (byte)(_c >> 16);
            bytes[10] = (byte)(_c >> 8);
            bytes[11] = (byte)(_c);
            bytes[12] = (byte)(_d >> 24);
            bytes[13] = (byte)(_d >> 16);
            bytes[14] = (byte)(_d >> 8);
            bytes[15] = (byte)(_d);

            return bytes;
        }

        public override string ToString()
        {
            return ToString("D", null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (obj.GetType() != typeof(NewId))
                return false;
            return Equals((NewId)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _a;
                result = (result * 397) ^ _b;
                result = (result * 397) ^ _c;
                result = (result * 397) ^ _d;
                return result;
            }
        }

        public static bool operator ==(NewId left, NewId right)
        {
            return left._a == right._a && left._b == right._b && left._c == right._c && left._d == right._d;
        }

        public static bool operator !=(NewId left, NewId right)
        {
            return !(left == right);
        }

        public static void SetGenerator(NewIdGenerator generator)
        {
            _generator = generator;
        }

        public static void SetWorkerIdProvider(IWorkerIdProvider provider)
        {
            _workerIdProvider = provider;
        }

        public static void SetTickProvider(ITickProvider provider)
        {
            _tickProvider = provider;
        }

        public static NewId Next()
        {
            return Generator.Next();
        }

        public static Guid NextGuid()
        {
            return Generator.Next().ToGuid();
        }

        static void ConvertBytes(byte[] bytes, out Int32 a, out Int32 b, out Int32 c, out Int32 d)
        {
            a = bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0];
            b = bytes[5] << 24 | bytes[4] << 16 | bytes[7] << 8 | bytes[6];
            c = bytes[8] << 24 | bytes[9] << 16 | bytes[10] << 8 | bytes[11];
            d = bytes[12] << 24 | bytes[13] << 16 | bytes[14] << 8 | bytes[15];
        }

        static char HexToChar(int value, int alpha)
        {
            value = value & 0xf;
            return (char)((value > 9) ? value - 10 + alpha : value + 0x30);
        }

        static int TwoBytesToChars(char[] chars, int offset, int first, int second, int alpha)
        {
            offset = ByteToChars(chars, offset, first, alpha);
            return ByteToChars(chars, offset, second, alpha);
        }

        static int ByteToChars(char[] chars, int offset, int value, int alpha)
        {
            chars[offset++] = HexToChar(value >> 4, alpha);
            chars[offset++] = HexToChar(value, alpha);

            return offset;
        }
    }
}