namespace MassTransit.Conductor.Client
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Caching;


    public class ServiceInstanceCache :
        IServiceInstanceCache
    {
        readonly ICache<ServiceInstanceContext> _cache;
        readonly IIndex<Guid, ServiceInstanceContext> _index;

        public ServiceInstanceCache()
        {
            _cache = new GreenCache<ServiceInstanceContext>(ServiceClientCacheDefaults.Settings);
            _index = _cache.AddIndex("clientId", x => x.InstanceId);
        }

        public Task<ServiceInstanceContext> GetOrAdd(Guid instanceId, InstanceInfo instance)
        {
            return _index.Get(instanceId, id => CreateInstance(instanceId, instance));
        }

        public void Remove(Guid instanceId)
        {
            _index.Remove(instanceId);
        }

        public ConnectHandle Connect(ICacheValueObserver<ServiceInstanceContext> observer)
        {
            return _cache.Connect(observer);
        }

        static Task<ServiceInstanceContext> CreateInstance(Guid instanceId, InstanceInfo instance)
        {
            var context = new InstanceContext(instanceId) {Started = instance?.Started};

            return Task.FromResult<ServiceInstanceContext>(context);
        }


        class InstanceContext :
            BasePipeContext,
            ServiceInstanceContext,
            INotifyValueUsed
        {
            public InstanceContext(Guid instanceId)
            {
                InstanceId = instanceId;
            }

            public Guid InstanceId { get; }

            public DateTime? Started { get; set; }

            public void NotifySent<T>(SendContext<T> context)
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
