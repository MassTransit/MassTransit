namespace MassTransit.EventHubIntegration
{
    using System;
    using MassTransit.Registration;


    public interface IEvenHubEndpointConnector
    {
        HostReceiveEndpointHandle ConnectEventHubEndpoint(string eventHubName, string consumerGroup,
            Action<IRiderRegistrationContext, IEventHubReceiveEndpointConfigurator> configure);
    }
}
