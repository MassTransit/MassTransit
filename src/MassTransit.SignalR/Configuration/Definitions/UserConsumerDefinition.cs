namespace MassTransit.SignalR.Configuration.Definitions
{
    using Consumers;
    using Microsoft.AspNetCore.SignalR;


    public class UserConsumerDefinition<THub> :
        ConsumerDefinition<UserConsumer<THub>>
        where THub : Hub
    {
        public UserConsumerDefinition(HubConsumerDefinition<THub> endpointDefinition)
        {
            EndpointDefinition = endpointDefinition;
        }
    }
}
