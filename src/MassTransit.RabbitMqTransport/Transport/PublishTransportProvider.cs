namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Caching;
    using Integration;
    using Transports;


    public class PublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IRabbitMqHostControl _host;
        readonly IIndex<Uri, CachedSendTransport> _index;
        readonly IModelContextSupervisor _modelContextSupervisor;

        public PublishTransportProvider(IRabbitMqHostControl host, IModelContextSupervisor modelContextSupervisor)
        {
            _host = host;
            _modelContextSupervisor = modelContextSupervisor;

            var cacheSettings = new CacheSettings(SendEndpointCacheDefaults.Capacity, SendEndpointCacheDefaults.MinAge, SendEndpointCacheDefaults.MaxAge);

            var cache = new GreenCache<CachedSendTransport>(cacheSettings);
            _index = cache.AddIndex("key", x => x.Address);
        }

        public async Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            async Task<CachedSendTransport> Create(Uri transportAddress)
            {
                var transport = await _host.CreatePublishTransport<T>(_modelContextSupervisor);

                return new CachedSendTransport(transportAddress, transport);
            }

            return await _index.Get(publishAddress, Create).ConfigureAwait(false);
        }
    }
}
