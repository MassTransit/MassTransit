namespace MassTransit.EventHubIntegration
{
    using System;


    public interface IEvenHubEndpointConnector
    {
        HostReceiveEndpointHandle ConnectEventHubEndpoint(string eventHubName, string consumerGroup,
            Action<IEventHubReceiveEndpointConfigurator> configure);
    }
}
