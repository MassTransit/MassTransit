namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Caching;
    using Integration;
    using Transports;


    public class SendTransportProvider :
        ISendTransportProvider
    {
        readonly IRabbitMqHostControl _host;
        readonly IModelContextSupervisor _modelContextSupervisor;
        readonly IIndex<Uri, CachedSendTransport> _index;

        public SendTransportProvider(IRabbitMqHostControl host, IModelContextSupervisor modelContextSupervisor)
        {
            _host = host;
            _modelContextSupervisor = modelContextSupervisor;

            var cacheSettings = new CacheSettings(SendEndpointCacheDefaults.Capacity, SendEndpointCacheDefaults.MinAge, SendEndpointCacheDefaults.MaxAge);

            var cache = new GreenCache<CachedSendTransport>(cacheSettings);
            _index = cache.AddIndex("key", x => x.Address);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new RabbitMqEndpointAddress(_host.Address, address);
        }

        async Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            var endpointAddress = new RabbitMqEndpointAddress(_host.Address, address);

            async Task<CachedSendTransport> Create(Uri transportAddress)
            {
                var transport = await _host.CreateSendTransport(endpointAddress, _modelContextSupervisor);

                return new CachedSendTransport(transportAddress, transport);
            }

            return await _index.Get(address, Create).ConfigureAwait(false);
        }
    }
}
