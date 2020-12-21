namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using Contexts;
    using Riders;
    using Util;


    public class EventHubRider :
        BaseRider,
        IEventHubRider
    {
        readonly IDictionary<string, IReceiveEndpointControl> _endpoints;
        readonly Recycle<IEventHubProducerSharedContext> _producerSharedContext;

        public EventHubRider(IDictionary<string, IReceiveEndpointControl> endpoints, Func<IEventHubProducerSharedContext> producerSharedContext)
            : base("azure.event-hub")
        {
            _endpoints = endpoints;
            _producerSharedContext = new Recycle<IEventHubProducerSharedContext>(producerSharedContext);
        }

        public IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default)
        {
            return new EventHubProducerProvider(_producerSharedContext.Supervisor, consumeContext);
        }

        protected override void AddReceiveEndpoint(IHost host)
        {
            foreach (KeyValuePair<string, IReceiveEndpointControl> endpoint in _endpoints)
                host.AddReceiveEndpoint(endpoint.Key, endpoint.Value);
        }
    }
}
