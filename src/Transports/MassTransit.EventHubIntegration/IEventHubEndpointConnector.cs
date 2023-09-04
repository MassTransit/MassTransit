namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface IEventHubEndpointConnector
    {
        HostReceiveEndpointHandle ConnectEventHubEndpoint(string eventHubName, string consumerGroup,
            Action<IRiderRegistrationContext, IEventHubReceiveEndpointConfigurator> configure);

        Task<bool> DisconnectEventHubEndpoint(string eventHubName, string consumerGroup);
    }
}
