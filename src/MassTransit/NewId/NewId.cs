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
    using NewIdFormatters;
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
        public static readonly NewId Empty = new NewId(0, 0, 0, 0);
        static INewIdFormatter _braceFormatter = new DashedHexFormatter('{', '}');
        static INewIdFormatter _dashedHexFormatter = new DashedHexFormatter();

        static NewIdGenerator _generator;

        static INewIdFormatter _hexFormatter = new HexFormatter();
        static INewIdFormatter _parenFormatter = new DashedHexFormatter('(', ')');
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

            FromByteArray(bytes, out _a, out _b, out _c, out _d);
        }

        public NewId(string value)
        {
            if(string.IsNullOrEmpty(value))
                throw new ArgumentException("must not be null or empty", "value");

            var guid = new Guid(value);

            byte[] bytes = guid.ToByteArray();

            FromByteArray(bytes, out _a, out _b, out _c, out _d);
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
            get { return _workerIdProvider ?? (_workerIdProvider = new BestPossibleWorkerIdProvider()); }
        }

        static ITickProvider TickProvider
        {
            get { return _tickProvider ?? (_tickProvider = new StopwatchTickProvider()); }
        }

        public DateTime Timestamp
        {
            get
            {
                var ticks = (long)(((ulong)_a) << 32 | (uint)_b);

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
            byte[] bytes = sequential ? GetSequentialFormatteryArray() : GetFormatteryArray();

            if (formatCh == 'B' || formatCh == 'b')
                return _braceFormatter.Format(bytes);
            if (formatCh == 'P' || formatCh == 'p')
                return _parenFormatter.Format(bytes);
            if (formatCh == 'D' || formatCh == 'd')
                return _dashedHexFormatter.Format(bytes);
            if (formatCh == 'N' || formatCh == 'n')
                return _hexFormatter.Format(bytes);

            throw new FormatException("The format string was not valid");
        }

        public string ToString(INewIdFormatter formatter, bool sequential = false)
        {
            byte[] bytes = sequential ? GetSequentialFormatteryArray() : GetFormatteryArray();

            return formatter.Format(bytes);
        }

        byte[] GetFormatteryArray()
        {
            var bytes = new byte[16];
            bytes[0] = (byte)(_d >> 24);
            bytes[1] = (byte)(_d >> 16);
            bytes[2] = (byte)(_d >> 8);
            bytes[3] = (byte)_d;
            bytes[4] = (byte)(_c >> 8);
            bytes[5] = (byte)_c;
            bytes[6] = (byte)(_c >> 24);
            bytes[7] = (byte)(_c >> 16);
            bytes[8] = (byte)(_b >> 8);
            bytes[9] = (byte)_b;
            bytes[10] = (byte)(_a >> 24);
            bytes[11] = (byte)(_a >> 16);
            bytes[12] = (byte)(_a >> 8);
            bytes[13] = (byte)_a;
            bytes[14] = (byte)(_b >> 24);
            bytes[15] = (byte)(_b >> 16);

            return bytes;
        }

        byte[] GetSequentialFormatteryArray()
        {
            var bytes = new byte[16];
            bytes[0] = (byte)(_a >> 24);
            bytes[1] = (byte)(_a >> 16);
            bytes[2] = (byte)(_a >> 8);
            bytes[3] = (byte)_a;
            bytes[4] = (byte)(_b >> 24);
            bytes[5] = (byte)(_b >> 16);
            bytes[6] = (byte)(_b >> 8);
            bytes[7] = (byte)_b;
            bytes[8] = (byte)(_c >> 24);
            bytes[9] = (byte)(_c >> 16);
            bytes[10] = (byte)(_c >> 8);
            bytes[11] = (byte)_c;
            bytes[12] = (byte)(_d >> 24);
            bytes[13] = (byte)(_d >> 16);
            bytes[14] = (byte)(_d >> 8);
            bytes[15] = (byte)_d;

            return bytes;
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

        public Guid ToSequentialGuid()
        {
            int a = _a;
            var b = (short)(_b >> 16);
            var c = (short)_b;
            var d = (byte)(_c >> 24);
            var e = (byte)(_c >> 16);
            var f = (byte)(_c >> 8);
            var g = (byte)(_c);
            var h = (byte)(_d >> 24);
            var i = (byte)(_d >> 16);
            var j = (byte)(_d >> 8);
            var k = (byte)(_d);

            return new Guid(a, b, c, d, e, f, g, h, i, j, k);
        }

        public byte[] ToByteArray()
        {
            var bytes = new byte[16];

            bytes[0] = (byte)(_d);
            bytes[1] = (byte)(_d >> 8);
            bytes[2] = (byte)(_d >> 16);
            bytes[3] = (byte)(_d >> 24);
            bytes[4] = (byte)(_c);
            bytes[5] = (byte)(_c >> 8);
            bytes[6] = (byte)(_c >> 16);
            bytes[7] = (byte)(_c >> 24);
            bytes[8] = (byte)(_b >> 8);
            bytes[9] = (byte)(_b);
            bytes[10] = (byte)(_a >> 24);
            bytes[11] = (byte)(_a >> 16);
            bytes[12] = (byte)(_a >> 8);
            bytes[13] = (byte)(_a);
            bytes[14] = (byte)(_b >> 24);
            bytes[15] = (byte)(_b >> 16);

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

        static void FromByteArray(byte[] bytes, out Int32 a, out Int32 b, out Int32 c, out Int32 d)
        {
            a = bytes[10] << 24 | bytes[11] << 16 | bytes[12] << 8 | bytes[13];
            b = bytes[14] << 24 | bytes[15] << 16 | bytes[8] << 8 | bytes[9];
            c = bytes[7] << 24 | bytes[6] << 16 | bytes[5] << 8 | bytes[4];
            d = bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0];
        }
    }
}