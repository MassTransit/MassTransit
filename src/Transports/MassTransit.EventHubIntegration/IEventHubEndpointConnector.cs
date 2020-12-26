namespace MassTransit.EventHubIntegration
{
    using System;
    using MassTransit.Registration;


    public interface IEventHubEndpointConnector
    {
        HostReceiveEndpointHandle ConnectEventHubEndpoint(string eventHubName, string consumerGroup,
            Action<IRiderRegistrationContext, IEventHubReceiveEndpointConfigurator> configure);
    }
}
