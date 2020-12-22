namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using Contexts;
    using Util;


    public class EventHubRider :
        IEventHubRider
    {
        readonly IDictionary<string, IReceiveEndpointControl> _endpoints;
        readonly Recycle<IEventHubProducerSharedContext> _producerSharedContext;

        public EventHubRider(IDictionary<string, IReceiveEndpointControl> endpoints, Func<IEventHubProducerSharedContext> producerSharedContext)
        {
            _endpoints = endpoints;
            _producerSharedContext = new Recycle<IEventHubProducerSharedContext>(producerSharedContext);
        }

        public IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default)
        {
            return new EventHubProducerProvider(_producerSharedContext.Supervisor, consumeContext);
        }

        public void Connect(IHost host)
        {
            host.AddReceiveEndpoint(_endpoints);
        }
    }
}
