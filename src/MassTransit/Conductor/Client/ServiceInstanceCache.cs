namespace MassTransit.Conductor.Client
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts.Conductor;
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

        public Task<ServiceInstanceContext> GetOrAdd(Guid instanceId, Uri instanceServiceEndpoint, InstanceInfo instance)
        {
            return _index.Get(instanceId, id => CreateInstance(instanceId, instanceServiceEndpoint, instance));
        }

        public void Remove(Guid instanceId)
        {
            _index.Remove(instanceId);
        }

        public ConnectHandle Connect(ICacheValueObserver<ServiceInstanceContext> observer)
        {
            return _cache.Connect(observer);
        }

        static Task<ServiceInstanceContext> CreateInstance(Guid instanceId, Uri instanceServiceEndpoint, InstanceInfo instance)
        {
            var context = new InstanceContext(instanceId, instanceServiceEndpoint) {Started = instance?.Started};

            return Task.FromResult<ServiceInstanceContext>(context);
        }


        class InstanceContext :
            BasePipeContext,
            ServiceInstanceContext,
            INotifyValueUsed
        {
            public InstanceContext(Guid instanceId, Uri endpoint)
            {
                InstanceId = instanceId;
                Endpoint = endpoint;
            }

            public event Action Used;

            public Guid InstanceId { get; }

            public Uri Endpoint { get; set; }

            public DateTime? Started { get; set; }

            public IReadOnlyDictionary<string, string> InstanceAttributes => throw new NotImplementedException();

            public void NotifySent<T>(SendContext<T> context)
                where T : class
            {
                NotifyUsed();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void NotifyUsed()
            {
                Used?.Invoke();
            }
        }
    }
}
