using System;
using System.Threading.Tasks;
using MassTransit.Internals.Caching;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration
{
    public class EventStoreDbProducerCache<TKey> :
        IEventStoreDbProducerCache<TKey>
    {
        readonly ICache<TKey, CachedEventStoreDbProducer<TKey>, ITimeToLiveCacheValue<CachedEventStoreDbProducer<TKey>>> _cache;

        public EventStoreDbProducerCache()
        {
            var options = new CacheOptions {Capacity = SendEndpointCacheDefaults.Capacity};
            var policy = new TimeToLiveCachePolicy<CachedEventStoreDbProducer<TKey>>(SendEndpointCacheDefaults.MaxAge);

            _cache = new MassTransitCache<TKey, CachedEventStoreDbProducer<TKey>, ITimeToLiveCacheValue<CachedEventStoreDbProducer<TKey>>>(policy, options);
        }

        public async Task<IEventStoreDbProducer> GetProducer(TKey key, Func<TKey, Task<IEventStoreDbProducer>> factory)
        {
            return await _cache.GetOrAdd(key, x => GetProducerFromFactory(x, factory)).ConfigureAwait(false);
        }

        static async Task<CachedEventStoreDbProducer<TKey>> GetProducerFromFactory(TKey address, Func<TKey, Task<IEventStoreDbProducer>> factory)
        {
            var sendEndpoint = await factory(address).ConfigureAwait(false);

            return new CachedEventStoreDbProducer<TKey>(address, sendEndpoint);
        }
    }
}
