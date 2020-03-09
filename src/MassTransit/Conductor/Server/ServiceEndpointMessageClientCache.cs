namespace MassTransit.Conductor.Server
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Contracts;
    using GreenPipes.Caching;
    using Metadata;


    public class ServiceEndpointMessageClientCache<TMessage> :
        IServiceEndpointMessageClientCache,
        ICacheValueObserver<ServiceClientContext>
        where TMessage : class
    {
        readonly ICache<ServiceClientContext> _cache;
        readonly IServiceEndpointClientCache _endpointClientCache;
        readonly IIndex<Guid, ServiceClientContext> _index;

        public ServiceEndpointMessageClientCache(IServiceEndpointClientCache endpointClientCache)
        {
            _endpointClientCache = endpointClientCache;
            _cache = new GreenCache<ServiceClientContext>(ServiceInstanceClientCacheDefaults.Settings);
            _index = _cache.AddIndex("clientId", x => x.ClientId);

            MessageInfo = MessageInfoCache.GetMessageInfo<TMessage>();

            endpointClientCache.Connect(this);
        }

        public MessageInfo MessageInfo { get; }

        public Task<ServiceClientContext> Link(Guid clientId, Uri address)
        {
            return _index.Get(clientId, id => CreateClient(clientId, address));
        }

        public void Unlink(Guid clientId)
        {
            _index.Remove(clientId);
        }

        public async Task NotifyEndpointReady(IReceiveEndpoint receiveEndpoint, InstanceInfo instanceInfo, ServiceInfo serviceInfo)
        {
            try
            {
                var endpoint = await receiveEndpoint.GetPublishSendEndpoint<Up<TMessage>>().ConfigureAwait(false);

                await endpoint.Send<Up<TMessage>>(new
                {
                    Service = serviceInfo,
                    Instance = instanceInfo,
                    Message = MessageInfo
                }).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Failed to publish endpoint up for {MessageType}", TypeMetadataCache<TMessage>.ShortName);
            }
        }

        Task<ServiceClientContext> CreateClient(Guid clientId, Uri address)
        {
            return _endpointClientCache.GetOrAdd(clientId, address);
        }

        public void ValueAdded(INode<ServiceClientContext> node, ServiceClientContext value)
        {
        }

        public void ValueRemoved(INode<ServiceClientContext> node, ServiceClientContext value)
        {
            _index.Remove(value.ClientId);
        }

        public void CacheCleared()
        {
            _cache.Clear();
        }
    }
}
