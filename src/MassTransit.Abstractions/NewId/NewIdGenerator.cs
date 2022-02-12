namespace MassTransit
{
    using System;
    using System.Threading;


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

            _c = (workerId[0] << 24) | (workerId[1] << 16) | (workerId[2] << 8) | workerId[3];

            if (processIdProvider != null)
            {
                var processId = processIdProvider.GetProcessId();
                _d = (processId[0] << 24) | (processId[1] << 16);
            }
            else
                _d = (workerId[4] << 24) | (workerId[5] << 16);

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

            var d = (byte)(b >> 8);
            var e = (byte)b;
            var f = (byte)(a >> 24);
            var g = (byte)(a >> 16);
            var h = (byte)(a >> 8);
            var i = (byte)a;
            var j = (byte)(b >> 24);
            var k = (byte)(b >> 16);

            // swapping high and low byte, because SQL-server is doing the wrong ordering otherwise
            var sequenceSwapped = ((sequence << 8) | ((sequence >> 8) & 0x00FF)) & 0xFFFF;

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
            var b = (short)(_b >> 16);
            var c = (short)_b;

            if (lockTaken)
                _spinLock.Exit();

            var d = (byte)(_gc >> 8);
            var e = (byte)_gc;
            var f = (byte)(_gb >> 8);
            var g = (byte)_gb;

            // swapping high and low byte, because SQL-server is doing the wrong ordering otherwise
            var sequenceSwapped = ((sequence << 8) | ((sequence >> 8) & 0x00FF)) & 0xFFFF;

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

        void UpdateTimestamp(long tick)
        {
            _b = (int)(tick & 0xFFFFFFFF);
            _a = (int)(tick >> 32);

            _sequence = 0;
            _lastTick = tick;
        }
    }
}
