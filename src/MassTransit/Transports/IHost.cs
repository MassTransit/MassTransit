namespace MassTransit.Transports
{
    using System;
    using System.Threading;


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

        IBusTopology Topology { get; }

        HostHandle Start(CancellationToken cancellationToken);

        void AddReceiveEndpoint(string endpointName, ReceiveEndpoint receiveEndpoint);

        IRider GetRider(string name);

        void AddRider(string name, IRiderControl riderControl);

        BusHealthResult CheckHealth(BusState busState, string healthMessage);
    }


    public interface IHost<out TEndpointConfigurator> :
        IHost,
        IReceiveConnector<TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
    }
}
