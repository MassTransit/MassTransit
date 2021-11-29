namespace MassTransit.SignalR.Configuration.Definitions
{
    using Consumers;
    using Microsoft.AspNetCore.SignalR;


    public class GroupManagementConsumerDefinition<THub> :
        ConsumerDefinition<GroupManagementConsumer<THub>>
        where THub : Hub
    {
        public GroupManagementConsumerDefinition(HubConsumerDefinition<THub> endpointDefinition)
        {
            EndpointDefinition = endpointDefinition;
        }
    }
}
