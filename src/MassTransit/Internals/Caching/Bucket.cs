namespace MassTransit.Internals.Caching
{
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;


    public class Bucket<TValue, TCacheValue> :
        IBucket<TValue, TCacheValue>
        where TValue : class
        where TCacheValue : ICacheValue<TValue>
    {
        readonly int _capacity;
        readonly ConcurrentQueue<TCacheValue> _values;
        readonly IValueTracker<TValue, TCacheValue> _valueTracker;
        int _count;

        public Bucket(IValueTracker<TValue, TCacheValue> valueTracker, int capacity)
        {
            _valueTracker = valueTracker;
            _capacity = capacity;

            _values = new ConcurrentQueue<TCacheValue>();
            _count = 0;
        }

        public int Count => _count;

        public int Capacity => _capacity;

        public Task Add(TCacheValue value)
        {
            _values.Enqueue(value);
            var count = Interlocked.Increment(ref _count);

            if (count <= _capacity)
                return Task.CompletedTask;

            Interlocked.Decrement(ref _count);

            if (_values.TryDequeue(out var cacheValue))
                return _valueTracker.ReBucket(this, cacheValue);

            Interlocked.Increment(ref _count);

            return Task.CompletedTask;
        }

        public async Task Clear()
        {
            while (_values.TryDequeue(out var cacheValue))
            {
                Interlocked.Decrement(ref _count);

                await cacheValue.Evict().ConfigureAwait(false);
            }
        }
    }
}
