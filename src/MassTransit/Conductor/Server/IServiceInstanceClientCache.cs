namespace MassTransit.Conductor.Server
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes.Caching;


    /// <summary>
    /// Tracks clients for a service instance
    /// </summary>
    public interface IServiceInstanceClientCache :
        IConnectCacheValueObserver<ServiceClientContext>
    {
        Task<ServiceClientContext> GetOrAdd(Guid clientId, Uri address);
    }
}
