namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;
    using GreenPipes.Caching;


    public interface IServiceInstanceCache :
        IConnectCacheValueObserver<ServiceInstanceContext>
    {
        Task<ServiceInstanceContext> GetOrAdd(Guid instanceId, InstanceInfo instance = default);

        void Remove(Guid instanceId);
    }
}
