namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Caching;


    public class EventHubProducerCache<TKey> :
        IEventHubProducerCache<TKey>
    {
        readonly IIndex<TKey, CachedEventHubProducer<TKey>> _index;

        public EventHubProducerCache()
        {
            var cacheSettings = new CacheSettings(1000, TimeSpan.FromSeconds(10), TimeSpan.FromHours(24));
            var cache = new GreenCache<CachedEventHubProducer<TKey>>(cacheSettings);
            _index = cache.AddIndex("key", x => x.Key);
        }

        public async Task<IEventHubProducer> GetProducer(TKey key, Func<TKey, Task<IEventHubProducer>> factory)
        {
            CachedEventHubProducer<TKey> producer = await _index.Get(key, x => GetProducerFromFactory(x, factory)).ConfigureAwait(false);

            return producer;
        }

        static async Task<CachedEventHubProducer<TKey>> GetProducerFromFactory(TKey address, Func<TKey, Task<IEventHubProducer>> factory)
        {
            var sendEndpoint = await factory(address).ConfigureAwait(false);

            return new CachedEventHubProducer<TKey>(address, sendEndpoint);
        }
    }
}
