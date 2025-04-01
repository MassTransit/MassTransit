namespace MassTransit
{
    using System;
    using System.Threading;
#if NET6_0_OR_GREATER
    using System.Runtime.CompilerServices;
    using System.Runtime.Intrinsics;
    using System.Runtime.Intrinsics.X86;
#endif


    public class NewIdGenerator :
        INewIdGenerator
    {
        readonly int _c;
        readonly int _d;
        readonly short _gb;
        readonly short _gc;
        readonly ITickProvider _tickProvider;
        int _a;
        int _b;
        long _lastTick;
        int _sequence;

        SpinLock _spinLock;

        public NewIdGenerator(ITickProvider tickProvider, IWorkerIdProvider workerIdProvider, IProcessIdProvider? processIdProvider = null, int workerIndex = 0)
        {
            _tickProvider = tickProvider;

            _spinLock = new SpinLock(false);

            var workerId = workerIdProvider.GetWorkerId(workerIndex);

            _c = (workerId[2] << 24) | (workerId[3] << 16) | (workerId[4] << 8) | workerId[5];

            if (processIdProvider != null)
            {
                var processId = processIdProvider.GetProcessId();
                _d = (processId[0] << 24) | (processId[1] << 16);
            }
            else
                _d = (workerId[0] << 24) | (workerId[1] << 16);

            _gb = (short)_c;
            _gc = (short)(_c >> 16);
        }

        public NewId Next()
        {
            var ticks = _tickProvider.Ticks;

            var lockTaken = false;
            _spinLock.Enter(ref lockTaken);

            if (ticks > _lastTick)
                UpdateTimestamp(ticks);
            else if (_sequence == 65535) // we are about to rollover, so we need to increment ticks
                UpdateTimestamp(_lastTick + 1);

            var sequence = _sequence++;

            var a = _a;
            var b = _b;

            if (lockTaken)
                _spinLock.Exit();

            return new NewId(a, b, _c, _d | sequence);
        }

        public Guid NextGuid()
        {
            var ticks = _tickProvider.Ticks;

            var lockTaken = false;
            _spinLock.Enter(ref lockTaken);

            if (ticks > _lastTick)
                UpdateTimestamp(ticks);
            else if (_sequence == 65535) // we are about to rollover, so we need to increment ticks
                UpdateTimestamp(_lastTick + 1);

            var sequence = _sequence++;

            var a = _a;
            var b = _b;

            if (lockTaken)
                _spinLock.Exit();

            // swapping high and low byte, because SQL-server is doing the wrong ordering otherwise
            var sequenceSwapped = ((sequence << 8) | ((sequence >> 8) & 0x00FF)) & 0xFFFF;

#if NET6_0_OR_GREATER
            if (Ssse3.IsSupported && BitConverter.IsLittleEndian)
            {
                var vec = Vector128.Create((int)a, b, _c, _d | sequenceSwapped);
                var result = Ssse3.Shuffle(vec.AsByte(), Vector128.Create((byte)12, 13, 14, 15, 8, 9, 10, 11, 5, 4, 3, 2, 1, 0, 7, 6));
                return Unsafe.As<Vector128<byte>, Guid>(ref result);
            }
#endif

            var d = (byte)(b >> 8);
            var e = (byte)b;
            var f = (byte)(a >> 24);
            var g = (byte)(a >> 16);
            var h = (byte)(a >> 8);
            var i = (byte)a;
            var j = (byte)(b >> 24);
            var k = (byte)(b >> 16);

            return new Guid(_d | sequenceSwapped, _gb, _gc, d, e, f, g, h, i, j, k);
        }

        public Guid NextSequentialGuid()
        {
            var ticks = _tickProvider.Ticks;

            var lockTaken = false;
            _spinLock.Enter(ref lockTaken);

            if (ticks > _lastTick)
                UpdateTimestamp(ticks);
            else if (_sequence == 65535) // we are about to rollover, so we need to increment ticks
                UpdateTimestamp(_lastTick + 1);

            var sequence = _sequence++;

            var a = _a;
#if NET6_0_OR_GREATER
            var v = _b;
#endif
            var b = (short)(_b >> 16);
            var c = (short)_b;

            if (lockTaken)
                _spinLock.Exit();

            // swapping high and low byte, because SQL-server is doing the wrong ordering otherwise
            var sequenceSwapped = ((sequence << 8) | ((sequence >> 8) & 0x00FF)) & 0xFFFF;

#if NET6_0_OR_GREATER
            if (Ssse3.IsSupported && BitConverter.IsLittleEndian)
            {
                var vec = Vector128.Create((int)a, v, _c, _d | sequenceSwapped);
                var result = Ssse3.Shuffle(vec.AsByte(), Vector128.Create((byte)0, 1, 2, 3, 6, 7, 4, 5, 11, 10, 9, 8, 15, 14, 13, 12));
                return Unsafe.As<Vector128<byte>, Guid>(ref result);
            }
#endif

            var d = (byte)(_gc >> 8);
            var e = (byte)_gc;
            var f = (byte)(_gb >> 8);
            var g = (byte)_gb;

            var h = (byte)((_d | sequenceSwapped) >> 24);
            var i = (byte)((_d | sequenceSwapped) >> 16);
            var j = (byte)((_d | sequenceSwapped) >> 8);
            var k = (byte)(_d | sequenceSwapped);

            return new Guid(a, b, c, d, e, f, g, h, i, j, k);
        }

        public ArraySegment<NewId> Next(NewId[] ids, int index, int count)
        {
            if (index + count > ids.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            var ticks = _tickProvider.Ticks;

            var lockTaken = false;
            _spinLock.Enter(ref lockTaken);

            if (ticks > _lastTick)
                UpdateTimestamp(ticks);

            var limit = index + count;
            for (var i = index; i < limit; i++)
            {
                if (_sequence == 65535) // we are about to rollover, so we need to increment ticks
                    UpdateTimestamp(_lastTick + 1);

                ids[i] = new NewId(_a, _b, _c, _d | _sequence++);
            }

            if (lockTaken)
                _spinLock.Exit();

            return new ArraySegment<NewId>(ids, index, count);
        }

        public ArraySegment<Guid> NextSequentialGuid(Guid[] ids, int index, int count)
        {
            if (index + count > ids.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            var ticks = _tickProvider.Ticks;

#if NET6_0_OR_GREATER
            var v = _b;
#endif
            var a = _a;
            var b = (short)(_b >> 16);
            var c = (short)_b;

            var lockTaken = false;
            _spinLock.Enter(ref lockTaken);

            if (ticks > _lastTick)
                UpdateTimestamp(ticks);

            var d = (byte)(_gc >> 8);
            var e = (byte)_gc;
            var f = (byte)(_gb >> 8);
            var g = (byte)_gb;

            var limit = index + count;
            for (var offset = index; offset < limit; offset++)
            {
                if (_sequence == 65535) // we are about to rollover, so we need to increment ticks
                {
                    UpdateTimestamp(_lastTick + 1);

#if NET6_0_OR_GREATER
                    v = _b;
#endif
                    a = _a;
                    b = (short)(_b >> 16);
                    c = (short)_b;
                }

                var sequence = _sequence++;

                // swapping high and low byte, because SQL-server is doing the wrong ordering otherwise
                var sequenceSwapped = ((sequence << 8) | ((sequence >> 8) & 0x00FF)) & 0xFFFF;

#if NET6_0_OR_GREATER
                if (Ssse3.IsSupported && BitConverter.IsLittleEndian)
                {
                    var vec = Vector128.Create((int)a, v, _c, _d | sequenceSwapped);
                    var result = Ssse3.Shuffle(vec.AsByte(), Vector128.Create((byte)0, 1, 2, 3, 6, 7, 4, 5, 11, 10, 9, 8, 15, 14, 13, 12));
                    ids[offset] = Unsafe.As<Vector128<byte>, Guid>(ref result);
                    continue;
                }
#endif

                var h = (byte)((_d | sequenceSwapped) >> 24);
                var i = (byte)((_d | sequenceSwapped) >> 16);
                var j = (byte)((_d | sequenceSwapped) >> 8);
                var k = (byte)(_d | sequenceSwapped);

                ids[offset] = new Guid(a, b, c, d, e, f, g, h, i, j, k);
            }

            if (lockTaken)
                _spinLock.Exit();

            return new ArraySegment<Guid>(ids, index, count);
        }

        void UpdateTimestamp(long tick)
        {
            _b = (int)(tick & 0xFFFFFFFF);
            _a = (int)(tick >> 32);

            _sequence = 0;
            _lastTick = tick;
        }
    }
}
