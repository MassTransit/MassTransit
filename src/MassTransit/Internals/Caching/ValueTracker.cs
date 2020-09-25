namespace MassTransit.Internals.Caching
{
    using System;
    using System.Threading.Tasks;


    public class ValueTracker<TValue, TCacheValue> :
        IValueTracker<TValue, TCacheValue>
        where TValue : class
        where TCacheValue : ICacheValue<TValue>
    {
        readonly Bucket<TValue, TCacheValue> _new;
        readonly ICachePolicy<TValue, TCacheValue> _policy;
        readonly Bucket<TValue, TCacheValue> _unused;
        readonly Bucket<TValue, TCacheValue> _used;

        public ValueTracker(ICachePolicy<TValue, TCacheValue> policy, int capacity)
        {
            if (capacity < 8)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be >= 8");

            _policy = policy;

            _new = new Bucket<TValue, TCacheValue>(this, capacity / 4);
            _used = new Bucket<TValue, TCacheValue>(this, capacity / 2);
            _unused = new Bucket<TValue, TCacheValue>(this, capacity / 4);
        }

        public int Capacity => _new.Capacity + _used.Capacity + _unused.Capacity;

        public Task Add(TCacheValue value)
        {
            return _new.Add(value);
        }

        public Task ReBucket(IBucket<TValue, TCacheValue> source, TCacheValue value)
        {
            var usage = _policy.CheckValue(value);

            if (usage == 0)
            {
                return source == _unused
                    ? value.Evict()
                    : _unused.Add(value);
            }

            if (usage > 0)
            {
                return source == _used
                    ? value.Evict()
                    : _used.Add(value);
            }

            return value.Evict();
        }
    }
}
