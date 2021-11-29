namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;


    public class CachedEventHubProducerProvider :
        IEventHubProducerProvider
    {
        readonly IEventHubProducerCache<Uri> _cache;
        readonly IEventHubProducerProvider _provider;

        public CachedEventHubProducerProvider(IEventHubProducerProvider provider)
        {
            _provider = provider;
            _cache = new EventHubProducerCache<Uri>();
        }

        public Task<IEventHubProducer> GetProducer(Uri address)
        {
            return _cache.GetProducer(address, _provider.GetProducer);
        }
    }
}
