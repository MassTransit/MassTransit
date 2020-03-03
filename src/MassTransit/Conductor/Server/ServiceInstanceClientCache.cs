namespace MassTransit.Conductor.Server
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Caching;


    public class ServiceInstanceClientCache :
        IServiceInstanceClientCache
    {
        readonly ICache<ServiceClientContext> _cache;
        readonly IIndex<Guid, ServiceClientContext> _index;

        public ServiceInstanceClientCache()
        {
            _cache = new GreenCache<ServiceClientContext>(ServiceInstanceClientCacheDefaults.Settings);
            _index = _cache.AddIndex("clientId", x => x.ClientId);
        }

        public ConnectHandle Connect(ICacheValueObserver<ServiceClientContext> observer)
        {
            return _cache.Connect(observer);
        }

        public Task<ServiceClientContext> GetOrAdd(Guid clientId, Uri address)
        {
            return _index.Get(clientId, id => CreateClient(clientId, address));
        }

        static Task<ServiceClientContext> CreateClient(Guid clientId, Uri address)
        {
            return Task.FromResult<ServiceClientContext>(new ClientContext(clientId, address));
        }


        class ClientContext :
            BasePipeContext,
            ServiceClientContext,
            INotifyValueUsed
        {
            public ClientContext(Guid clientId, Uri address)
            {
                ClientId = clientId;
                Address = address;
            }

            public Guid ClientId { get; }
            public Uri Address { get; }

            public void NotifyConsumed<T>(ConsumeContext<T> context)
                where T : class
            {
                NotifyUsed();
            }

            public event Action Used;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void NotifyUsed()
            {
                Used?.Invoke();
            }
        }
    }
}
