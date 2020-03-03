namespace MassTransit.Conductor.Server
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;


    public interface IServiceEndpointMessageClientCache
    {
        MessageInfo MessageInfo { get; }

        Task<ServiceClientContext> Link(Guid clientId, Uri address);

        void Unlink(Guid clientId);

        Task NotifyEndpointReady(IReceiveEndpoint receiveEndpoint, InstanceInfo instanceInfo, ServiceInfo serviceInfo);
    }
}
