namespace MassTransit.SignalR.Configuration.Definitions
{
    using Consumers;
    using Microsoft.AspNetCore.SignalR;


    public class AllConsumerDefinition<THub> :
        ConsumerDefinition<AllConsumer<THub>>
        where THub : Hub
    {
        public AllConsumerDefinition(HubConsumerDefinition<THub> endpointDefinition)
        {
            EndpointDefinition = endpointDefinition;
        }
    }
}
