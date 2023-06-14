namespace MassTransit.ActiveMqTransport
{
    using System.Threading.Tasks;
    using Apache.NMS;
    using Internals.Caching;
    using MassTransit.Middleware;
    using Transports;


    public class MessageProducerCache :
        Agent
    {
        public delegate Task<IMessageProducer> MessageProducerFactory(IDestination destination);


        readonly ICache<IDestination, CachedMessageProducer, ITimeToLiveCacheValue<CachedMessageProducer>> _cache;

        public MessageProducerCache()
        {
            var options = new CacheOptions { Capacity = SendEndpointCacheDefaults.Capacity };
            var policy = new TimeToLiveCachePolicy<CachedMessageProducer>(SendEndpointCacheDefaults.MaxAge);

            _cache = new MassTransitCache<IDestination, CachedMessageProducer, ITimeToLiveCacheValue<CachedMessageProducer>>(policy, options);
        }

        public async Task<IMessageProducer> GetMessageProducer(IDestination key, MessageProducerFactory factory)
        {
            var messageProducer = await _cache.GetOrAdd(key, x => GetMessageProducerFromFactory(x, factory)).ConfigureAwait(false);

            return messageProducer;
        }

        static async Task<CachedMessageProducer> GetMessageProducerFromFactory(IDestination destination, MessageProducerFactory factory)
        {
            var messageProducer = await factory(destination).ConfigureAwait(false);

            return new CachedMessageProducer(destination, messageProducer);
        }

        protected override Task StopAgent(StopContext context)
        {
            return _cache.Clear();
        }
    }
}
