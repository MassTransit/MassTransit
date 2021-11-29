namespace MassTransit
{
    using System;


    public interface IEventHubEndpointConnector
    {
        HostReceiveEndpointHandle ConnectEventHubEndpoint(string eventHubName, string consumerGroup,
            Action<IRiderRegistrationContext, IEventHubReceiveEndpointConfigurator> configure);
    }
}
