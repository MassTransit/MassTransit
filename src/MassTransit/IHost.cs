namespace MassTransit
{
    using System;
    using System.Threading;
    using GreenPipes;
    using Pipeline;
    using Riders;
    using Topology;
    using Transports;


    public interface IHost :
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

        HostHandle Start(CancellationToken cancellationToken);

        void AddReceiveEndpoint(string endpointName, ReceiveEndpoint receiveEndpoint);

        IRider GetRider(string name);

        void AddRider(string name, IRiderControl riderControl);

        HealthResult CheckHealth(BusState busState, string healthMessage);
    }


    public interface IHost<out TEndpointConfigurator> :
        IHost,
        IReceiveConnector<TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
    }
}
