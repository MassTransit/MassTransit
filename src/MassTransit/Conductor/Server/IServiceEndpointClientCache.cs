namespace MassTransit.Conductor.Server
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;
    using GreenPipes.Caching;


    public interface IServiceEndpointClientCache :
        IConnectCacheValueObserver<ServiceClientContext>
    {
        IServiceEndpointMessageClientCache GetMessageCache<T>()
            where T : class;

        Task<ServiceClientContext> GetOrAdd(Guid clientId, Uri address);

        void NotifyFaulted(ServiceClientContext context, Exception exception);

        Task ForEachAsync(Func<Task<ServiceClientContext>, Task> callback);

        Task NotifyEndpointReady(IReceiveEndpoint receiveEndpoint, InstanceInfo instanceInfo, ServiceInfo serviceInfo);
    }
}
