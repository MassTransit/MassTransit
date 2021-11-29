namespace MassTransit.SignalR.Configuration.Definitions
{
    using Consumers;
    using Microsoft.AspNetCore.SignalR;


    public class ConnectionConsumerDefinition<THub> :
        ConsumerDefinition<ConnectionConsumer<THub>>
        where THub : Hub
    {
        public ConnectionConsumerDefinition(HubConsumerDefinition<THub> endpointDefinition)
        {
            EndpointDefinition = endpointDefinition;
        }
    }
}
