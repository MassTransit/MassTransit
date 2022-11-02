namespace MassTransit.Internals.Caching
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;


    public class MassTransitCache<TKey, TValue, TCacheValue> :
        ICache<TKey, TValue, TCacheValue>
        where TValue : class
        where TCacheValue : ICacheValue<TValue>
    {
        readonly ICachePolicy<TValue, TCacheValue> _policy;
        readonly IValueTracker<TValue, TCacheValue> _tracker;
        readonly ConcurrentDictionary<TKey, TCacheValue> _values;
        CacheMetrics _metrics;

        public MassTransitCache(ICachePolicy<TValue, TCacheValue> policy, CacheOptions options = default)
        {
            _policy = policy;

            options ??= new CacheOptions();

            _tracker = new ValueTracker<TValue, TCacheValue>(policy, options.Capacity);
            _values = new ConcurrentDictionary<TKey, TCacheValue>(options.ConcurrencyLevel, options.Capacity);

            _metrics = new CacheMetrics();
        }

        public int Count => _values.Skip(0).Count();

        public double HitRatio => _metrics.HitRatio;

        public Task<IEnumerable<TValue>> Values => GetValues();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<TValue> Get(TKey key)
        {
            if (TryGetValue(key, out var cacheValue))
                return cacheValue.Value;

            _metrics.Miss();
            throw new KeyNotFoundException($"Key not found: {key}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<TValue> GetOrAdd(TKey key, MissingValueFactory<TKey, TValue> missingValueFactory)
        {
            if (TryGetValue(key, out var cacheValue))
            {
                return cacheValue.HasValue
                    ? cacheValue.Value
                    : cacheValue.GetValue(() => new PendingValue<TKey, TValue>(key, missingValueFactory));
            }

            void RemoveValue()
            {
                _values.TryRemove(key, out _);
            }

            cacheValue = _policy.CreateValue(RemoveValue);

            if (!_values.TryAdd(key, cacheValue))
                return GetOrAdd(key, missingValueFactory);

            _metrics.Miss();

            async Task<TValue> Added()
            {
                await _tracker.Add(cacheValue).ConfigureAwait(false);

                return await cacheValue.GetValue(() => new PendingValue<TKey, TValue>(key, missingValueFactory)).ConfigureAwait(false);
            }

            return Added();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<bool> Remove(TKey key)
        {
            if (!TryGetValue(key, out var cacheValue))
                return false;

            await cacheValue.Evict().ConfigureAwait(false);
            return true;
        }

        public Task Clear()
        {
            return _tracker.Clear();
        }

        async Task<IEnumerable<TValue>> GetValues()
        {
            return await Task.WhenAll(_values.Values.Select(x => x.Value));
        }

        bool TryGetValue(TKey key, out TCacheValue value)
        {
            if (_values.TryGetValue(key, out value) && !value.IsFaultedOrCanceled)
            {
                _metrics.Hit();
                return true;
            }

            return false;
        }
    }
}
