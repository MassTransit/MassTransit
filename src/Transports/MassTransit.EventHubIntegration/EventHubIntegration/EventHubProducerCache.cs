namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Internals.Caching;
    using Transports;


    public class EventHubProducerCache<TKey> :
        IEventHubProducerCache<TKey>
    {
        readonly ICache<TKey, CachedEventHubProducer<TKey>, ITimeToLiveCacheValue<CachedEventHubProducer<TKey>>> _cache;

        public EventHubProducerCache()
        {
            var options = new CacheOptions {Capacity = SendEndpointCacheDefaults.Capacity};
            var policy = new TimeToLiveCachePolicy<CachedEventHubProducer<TKey>>(SendEndpointCacheDefaults.MaxAge);

            _cache = new MassTransitCache<TKey, CachedEventHubProducer<TKey>, ITimeToLiveCacheValue<CachedEventHubProducer<TKey>>>(policy, options);
        }

        public async Task<IEventHubProducer> GetProducer(TKey key, Func<TKey, Task<IEventHubProducer>> factory)
        {
            return await _cache.GetOrAdd(key, x => GetProducerFromFactory(x, factory)).ConfigureAwait(false);
        }

        static async Task<CachedEventHubProducer<TKey>> GetProducerFromFactory(TKey address, Func<TKey, Task<IEventHubProducer>> factory)
        {
            var sendEndpoint = await factory(address).ConfigureAwait(false);

            return new CachedEventHubProducer<TKey>(address, sendEndpoint);
        }
    }
}
