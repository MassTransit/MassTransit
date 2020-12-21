namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using Contexts;
    using Riders;


    public class EventHubRider :
        BaseRider,
        IEventHubRider
    {
        readonly IDictionary<string, IReceiveEndpointControl> _endpoints;
        readonly IEventHubProducerSharedContext _producerSharedContext;

        public EventHubRider(IDictionary<string, IReceiveEndpointControl> endpoints, IEventHubProducerSharedContext producerSharedContext)
            : base("azure.event-hub")
        {
            _endpoints = endpoints;
            _producerSharedContext = producerSharedContext;
        }

        public IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default)
        {
            return new EventHubProducerProvider(_producerSharedContext, consumeContext);
        }

        protected override void AddReceiveEndpoint(IHost host)
        {
            foreach (KeyValuePair<string, IReceiveEndpointControl> endpoint in _endpoints)
                host.AddReceiveEndpoint(endpoint.Key, endpoint.Value);
        }
    }
}
