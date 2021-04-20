using System;
using System.Threading.Tasks;

namespace MassTransit.EventStoreDbIntegration
{
    public class CachedEventStoreDbProducerProvider :
        IEventStoreDbProducerProvider
    {
        readonly IEventStoreDbProducerCache<Uri> _cache;
        readonly IEventStoreDbProducerProvider _provider;

        public CachedEventStoreDbProducerProvider(IEventStoreDbProducerProvider provider)
        {
            _provider = provider;
            _cache = new EventStoreDbProducerCache<Uri>();
        }

        public Task<IEventStoreDbProducer> GetProducer(Uri address)
        {
            return _cache.GetProducer(address, _provider.GetProducer);
        }
    }
}
