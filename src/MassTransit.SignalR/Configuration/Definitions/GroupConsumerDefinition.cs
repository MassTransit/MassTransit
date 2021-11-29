namespace MassTransit.SignalR.Configuration.Definitions
{
    using Consumers;
    using Microsoft.AspNetCore.SignalR;


    public class GroupConsumerDefinition<THub> :
        ConsumerDefinition<GroupConsumer<THub>>
        where THub : Hub
    {
        public GroupConsumerDefinition(HubConsumerDefinition<THub> endpointDefinition)
        {
            EndpointDefinition = endpointDefinition;
        }
    }
}
