namespace MassTransit
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using NewIdFormatters;
    using NewIdProviders;


    /// <summary>
    /// A NewId is a type that fits into the same space as a Guid/Uuid/unique identifier,
    /// but is guaranteed to be both unique and ordered, assuming it is generated using
    /// a single instance of the generator for each network address used.
    /// </summary>
    public readonly struct NewId :
        IEquatable<NewId>,
        IComparable<NewId>,
        IComparable,
        IFormattable
    {
        public static readonly NewId Empty = new NewId(0, 0, 0, 0);

        static readonly INewIdFormatter BraceFormatter = new DashedHexFormatter('{', '}');
        static readonly INewIdFormatter DashedHexFormatter = new DashedHexFormatter();
        static readonly INewIdFormatter HexFormatter = new HexFormatter();
        static readonly INewIdFormatter ParenFormatter = new DashedHexFormatter('(', ')');

        static INewIdGenerator? _generator;
        static ITickProvider? _tickProvider;
        static IWorkerIdProvider? _workerIdProvider;

        readonly int _a;
        readonly int _b;
        readonly int _c;
        readonly int _d;

        /// <summary>
        /// Creates a NewId using the specified byte array.
        /// </summary>
        /// <param name="bytes"></param>
        public NewId(in byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length != 16)
                throw new ArgumentException("Exactly 16 bytes expected", nameof(bytes));

            FromByteArray(bytes, out _a, out _b, out _c, out _d);
        }

        public NewId(in string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("must not be null or empty", nameof(value));

            var guid = new Guid(value);

            var bytes = guid.ToByteArray();

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
            _c = (c << 16) | (ushort)b;
            _d = (int)(a & 0xFFFF0000) | ((a >> 8) & 0x00FF) | ((a << 8) & 0xFF00);
        }

        static IWorkerIdProvider WorkerIdProvider => _workerIdProvider ??= new BestPossibleWorkerIdProvider();

        static IProcessIdProvider? ProcessIdProvider { get; set; } = new CurrentProcessIdProvider();

        static ITickProvider TickProvider => _tickProvider ??= new DateTimeTickProvider();

        public DateTime Timestamp
        {
            get
            {
                var ticks = (long)(((ulong)_a << 32) | (uint)_b);

                return new DateTime(ticks, DateTimeKind.Utc);
            }
        }

        public int CompareTo(object? obj)
        {
            if (obj == null)
                return 1;
            if (obj is NewId id)
                return CompareTo(id);

            throw new ArgumentException("Argument must be a NewId");
        }

        public int CompareTo(NewId other)
        {
            if (_a != other._a)
                return (uint)_a < (uint)other._a ? -1 : 1;
            if (_b != other._b)
                return (uint)_b < (uint)other._b ? -1 : 1;
            if (_c != other._c)
                return (uint)_c < (uint)other._c ? -1 : 1;
            if (_d != other._d)
                return (uint)_d < (uint)other._d ? -1 : 1;

            return 0;
        }

        public bool Equals(NewId other)
        {
            return other._a == _a && other._b == _b && other._c == _c && other._d == _d;
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (format == null || string.IsNullOrEmpty(format))
                format = "D";

            var sequential = false;
            if (format.Length == 2 && (format[1] == 'S' || format[1] == 's'))
                sequential = true;
            else if (format.Length != 1)
                throw new FormatException("The format string must be exactly one character or null");

            var formatCh = format[0];
            var bytes = sequential ? GetSequentialFormatterArray() : GetFormatterArray();

            if (formatCh == 'B' || formatCh == 'b')
                return BraceFormatter.Format(bytes);
            if (formatCh == 'P' || formatCh == 'p')
                return ParenFormatter.Format(bytes);
            if (formatCh == 'D' || formatCh == 'd')
                return DashedHexFormatter.Format(bytes);
            if (formatCh == 'N' || formatCh == 'n')
                return HexFormatter.Format(bytes);

            throw new FormatException("The format string was not valid");
        }

        static readonly ThreadLocal<byte[]> _formatterArray = new ThreadLocal<byte[]>(() => new byte[16]);

        public string ToString(INewIdFormatter formatter, bool sequential = false)
        {
            var bytes = sequential ? GetSequentialFormatterArray() : GetFormatterArray();

            return formatter.Format(bytes);
        }

        byte[] GetFormatterArray()
        {
            var bytes = _formatterArray.Value;

            bytes[15] = (byte)(_b >> 16);
            bytes[14] = (byte)(_b >> 24);
            bytes[13] = (byte)_a;
            bytes[12] = (byte)(_a >> 8);
            bytes[11] = (byte)(_a >> 16);
            bytes[10] = (byte)(_a >> 24);
            bytes[9] = (byte)_b;
            bytes[8] = (byte)(_b >> 8);
            bytes[7] = (byte)(_c >> 16);
            bytes[6] = (byte)(_c >> 24);
            bytes[5] = (byte)_c;
            bytes[4] = (byte)(_c >> 8);
            bytes[3] = (byte)(_d >> 8);
            bytes[2] = (byte)_d;
            bytes[1] = (byte)(_d >> 16);
            bytes[0] = (byte)(_d >> 24);

            return bytes;
        }

        byte[] GetSequentialFormatterArray()
        {
            var bytes = _formatterArray.Value;

            bytes[15] = (byte)_d;
            bytes[14] = (byte)(_d >> 8);
            bytes[13] = (byte)(_d >> 16);
            bytes[12] = (byte)(_d >> 24);
            bytes[11] = (byte)_c;
            bytes[10] = (byte)(_c >> 8);
            bytes[9] = (byte)(_c >> 16);
            bytes[8] = (byte)(_c >> 24);
            bytes[7] = (byte)_b;
            bytes[6] = (byte)(_b >> 8);
            bytes[5] = (byte)(_b >> 16);
            bytes[4] = (byte)(_b >> 24);
            bytes[3] = (byte)_a;
            bytes[2] = (byte)(_a >> 8);
            bytes[1] = (byte)(_a >> 16);
            bytes[0] = (byte)(_a >> 24);

            return bytes;
        }

        public Guid ToGuid()
        {
            var a = (int)(_d & 0xFFFF0000) | ((_d >> 8) & 0x00FF) | ((_d << 8) & 0xFF00);
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
            var a = _a;
            var b = (short)(_b >> 16);
            var c = (short)_b;
            var d = (byte)(_c >> 24);
            var e = (byte)(_c >> 16);
            var f = (byte)(_c >> 8);
            var g = (byte)_c;
            var h = (byte)(_d >> 24);
            var i = (byte)(_d >> 16);
            var j = (byte)(_d >> 8);
            var k = (byte)_d;

            return new Guid(a, b, c, d, e, f, g, h, i, j, k);
        }

        public static NewId FromGuid(in Guid guid)
        {
            var bytes = guid.ToByteArray();
            FromByteArray(bytes, out var a, out var b, out var c, out var d);

            return new NewId(a, b, c, d);
        }

        public static NewId FromSequentialGuid(in Guid guid)
        {
            var bytes = guid.ToByteArray();
            FromSequentialByteArray(bytes, out var a, out var b, out var c, out var d);

            return new NewId(a, b, c, d);
        }

        public byte[] ToByteArray()
        {
            var bytes = new byte[16];

            bytes[15] = (byte)(_b >> 16);
            bytes[14] = (byte)(_b >> 24);
            bytes[13] = (byte)_a;
            bytes[12] = (byte)(_a >> 8);
            bytes[11] = (byte)(_a >> 16);
            bytes[10] = (byte)(_a >> 24);
            bytes[9] = (byte)_b;
            bytes[8] = (byte)(_b >> 8);
            bytes[7] = (byte)(_c >> 24);
            bytes[6] = (byte)(_c >> 16);
            bytes[5] = (byte)(_c >> 8);
            bytes[4] = (byte)_c;
            bytes[3] = (byte)(_d >> 24);
            bytes[2] = (byte)(_d >> 16);
            bytes[1] = (byte)_d;
            bytes[0] = (byte)(_d >> 8);

            return bytes;
        }

        public override string ToString()
        {
            return ToString("D", null);
        }

        public string ToString(string? format)
        {
            return ToString(format, null);
        }

        public override bool Equals(object? obj)
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
                var result = _a;
                result = (result * 397) ^ _b;
                result = (result * 397) ^ _c;
                result = (result * 397) ^ _d;
                return result;
            }
        }

        public static bool operator ==(in NewId left, in NewId right)
        {
            return left._a == right._a && left._b == right._b && left._c == right._c && left._d == right._d;
        }

        public static bool operator !=(in NewId left, in NewId right)
        {
            return !(left == right);
        }

        public static bool operator <(in NewId left, in NewId right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(in NewId left, in NewId right)
        {
            return left.CompareTo(right) > 0;
        }

        public static void SetGenerator(INewIdGenerator generator)
        {
            _generator = generator;
        }

        public static void SetWorkerIdProvider(IWorkerIdProvider provider)
        {
            _workerIdProvider = provider;
        }

        public static void SetProcessIdProvider(IProcessIdProvider? provider)
        {
            ProcessIdProvider = provider;
        }

        public static void SetTickProvider(ITickProvider provider)
        {
            _tickProvider = provider;
        }

        static SpinLock _spinLock = new SpinLock(false);

        static INewIdGenerator _getGenerator()
        {
            if (_generator != null)
                return _generator;

            var lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);

                _generator ??= new NewIdGenerator(TickProvider, WorkerIdProvider, ProcessIdProvider);
            }
            finally
            {
                if (lockTaken)
                    _spinLock.Exit();
            }

            return _generator;
        }

        /// <summary>
        /// Generate a NewId
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NewId Next()
        {
            return _getGenerator().Next();
        }

        /// <summary>
        /// Generate an array of NewIds
        /// </summary>
        /// <param name="count">The number of NewIds to generate</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NewId[] Next(int count)
        {
            var ids = new NewId[count];

            _getGenerator().Next(ids, 0, count);

            return ids;
        }

        /// <summary>
        /// Generate an array of NewIds
        /// </summary>
        /// <param name="index">The starting offset for the newly generated ids</param>
        /// <param name="count">The number of NewIds to generate</param>
        /// <param name="ids">An existing array</param>
        /// <returns></returns>
        public static ArraySegment<NewId> Next(NewId[] ids, int index, int count)
        {
            return _getGenerator().Next(ids, index, count);
        }

        /// <summary>
        /// Generate a NewId, and return it as a Guid
        /// </summary>
        /// <returns></returns>
        public static Guid NextGuid()
        {
            return _getGenerator().NextGuid();
        }

        /// <summary>
        /// Generate a NewId, and return it as a Guid in sequential format
        /// </summary>
        /// <returns></returns>
        public static Guid NextSequentialGuid()
        {
            return _getGenerator().NextSequentialGuid();
        }

        static void FromByteArray(in byte[] bytes, out int a, out int b, out int c, out int d)
        {
            a = (bytes[10] << 24) | (bytes[11] << 16) | (bytes[12] << 8) | bytes[13];
            b = (bytes[14] << 24) | (bytes[15] << 16) | (bytes[8] << 8) | bytes[9];
            c = (bytes[7] << 24) | (bytes[6] << 16) | (bytes[5] << 8) | bytes[4];
            d = (bytes[3] << 24) | (bytes[2] << 16) | (bytes[0] << 8) | bytes[1];
        }

        static void FromSequentialByteArray(in byte[] bytes, out int a, out int b, out int c, out int d)
        {
            a = (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];
            b = (bytes[5] << 24) | (bytes[4] << 16) | (bytes[7] << 8) | bytes[6];
            c = (bytes[8] << 24) | (bytes[9] << 16) | (bytes[10] << 8) | bytes[11];
            d = (bytes[12] << 24) | (bytes[13] << 16) | (bytes[15] << 8) | bytes[14];
        }
    }
}
