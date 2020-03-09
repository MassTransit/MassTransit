namespace MassTransit.Conductor.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Caching;


    public class ServiceEndpointClientCache :
        IServiceEndpointClientCache,
        ICacheValueObserver<ServiceClientContext>
    {
        readonly ICache<ServiceClientContext> _cache;
        readonly IServiceInstanceClientCache _instanceClientCache;
        readonly ConcurrentDictionary<Type, IServiceEndpointMessageClientCache> _messageTypes;
        readonly IIndex<Guid, ServiceClientContext> _index;

        public ServiceEndpointClientCache(IServiceInstanceClientCache instanceClientCache)
        {
            _instanceClientCache = instanceClientCache;

            _cache = new GreenCache<ServiceClientContext>(ServiceInstanceClientCacheDefaults.Settings);
            _index = _cache.AddIndex("clientId", x => x.ClientId);

            _messageTypes = new ConcurrentDictionary<Type, IServiceEndpointMessageClientCache>();

            instanceClientCache.Connect(this);
        }

        public IServiceEndpointMessageClientCache GetMessageCache<T>()
            where T : class
        {
            return _messageTypes.GetOrAdd(typeof(T), CreateMessageCache<T>());
        }

        public Task<ServiceClientContext> GetOrAdd(Guid clientId, Uri address)
        {
            return _index.Get(clientId, id => _instanceClientCache.GetOrAdd(clientId, address));
        }

        public void NotifyFaulted(ServiceClientContext context, Exception exception)
        {
            _index.Remove(context.ClientId);
        }

        public Task ForEachAsync(Func<Task<ServiceClientContext>, Task> callback)
        {
            return Task.WhenAll(_cache.GetAll().Select(callback));
        }

        public Task NotifyEndpointReady(IReceiveEndpoint receiveEndpoint, InstanceInfo instanceInfo, ServiceInfo serviceInfo)
        {
            return Task.WhenAll(_messageTypes.Values.Select(type => type.NotifyEndpointReady(receiveEndpoint, instanceInfo, serviceInfo)));
        }

        public ConnectHandle Connect(ICacheValueObserver<ServiceClientContext> observer)
        {
            return _cache.Connect(observer);
        }

        IServiceEndpointMessageClientCache CreateMessageCache<T>()
            where T : class
        {
            return new ServiceEndpointMessageClientCache<T>(this);
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
