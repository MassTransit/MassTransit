namespace MassTransit.KafkaIntegration.Caching
{
    using System;
    using System.Threading.Tasks;
    using Internals.Caching;
    using Transports;


    public class TopicProducerCache<T> :
        ITopicProducerCache<T>
    {
        readonly ICache<T, ICachedTopicProducer<T>, ITimeToLiveCacheValue<ICachedTopicProducer<T>>> _cache;

        public TopicProducerCache()
        {
            var options = new CacheOptions { Capacity = SendEndpointCacheDefaults.Capacity };
            var policy = new TimeToLiveCachePolicy<ICachedTopicProducer<T>>(SendEndpointCacheDefaults.MaxAge);

            _cache = new MassTransitCache<T, ICachedTopicProducer<T>, ITimeToLiveCacheValue<ICachedTopicProducer<T>>>(policy, options);
        }

        public async Task<ITopicProducer<TKey, TValue>> GetProducer<TKey, TValue>(T key, Func<T, ITopicProducer<TKey, TValue>> factory)
            where TValue : class
        {
            ICachedTopicProducer<T> cached = await _cache.GetOrAdd(key, _ => Task.FromResult(CreateProducer(key, factory))).ConfigureAwait(false);
            if (cached is ITopicProducer<TKey, TValue> producer)
                return producer;

            throw new ConfigurationException($"Producer for key: {key} is not ${typeof(ITopicProducer<TKey, TValue>).Name}");
        }

        static ICachedTopicProducer<T> CreateProducer<TKey, TValue>(T key, Func<T, ITopicProducer<TKey, TValue>> factory)
            where TValue : class
        {
            ITopicProducer<TKey, TValue> producer = factory(key);
            return new CachedTopicProducer<T, TKey, TValue>(key, producer);
        }
    }
}
