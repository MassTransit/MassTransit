namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Topology;
    using Transports;


    public interface IHost :
        ISupervisor,
        IReceiveConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IReceiveObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IReceiveEndpointObserverConnector,
        IProbeSite
    {
        Uri Address { get; }

        IHostTopology Topology { get; }

        Task<HostHandle> Start(CancellationToken cancellationToken);

        void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint);
    }
}
