namespace MassTransit.ActiveMqTransport.Contexts
{
    using System.Threading.Tasks;
    using Apache.NMS;
    using GreenPipes.Agents;
    using GreenPipes.Caching;
    using Transports;
    using Util;


    public class MessageProducerCache :
        Agent
    {
        public delegate Task<IMessageProducer> MessageProducerFactory(IDestination destination);


        readonly GreenCache<CachedMessageProducer> _cache;

        readonly IIndex<IDestination, CachedMessageProducer> _index;

        public MessageProducerCache()
        {
            var cacheSettings = new CacheSettings(SendEndpointCacheDefaults.Capacity, SendEndpointCacheDefaults.MinAge, SendEndpointCacheDefaults.MaxAge);
            _cache = new GreenCache<CachedMessageProducer>(cacheSettings);
            _cache.Connect(new CloseAndDisposeOnRemoveObserver());

            _index = _cache.AddIndex("destination", x => x.Destination);
        }

        public async Task<IMessageProducer> GetMessageProducer(IDestination key, MessageProducerFactory factory)
        {
            var messageProducer = await _index.Get(key, x => GetMessageProducerFromFactory(x, factory)).ConfigureAwait(false);

            return messageProducer;
        }

        async Task<CachedMessageProducer> GetMessageProducerFromFactory(IDestination destination, MessageProducerFactory factory)
        {
            var messageProducer = await factory(destination).ConfigureAwait(false);

            return new CachedMessageProducer(destination, messageProducer);
        }

        protected override Task StopAgent(StopContext context)
        {
            foreach (Task<CachedMessageProducer> producer in _cache.GetAll())
                producer.Dispose();

            _cache.Clear();

            return TaskUtil.Completed;
        }


        class CloseAndDisposeOnRemoveObserver :
            ICacheValueObserver<CachedMessageProducer>
        {
            public void ValueAdded(INode<CachedMessageProducer> node, CachedMessageProducer value)
            {
            }

            public void ValueRemoved(INode<CachedMessageProducer> node, CachedMessageProducer value)
            {
                value.Close();
                value.Dispose();
            }

            public void CacheCleared()
            {
            }
        }
    }
}
