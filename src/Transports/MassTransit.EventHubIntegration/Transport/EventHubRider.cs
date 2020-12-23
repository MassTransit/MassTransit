namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;


    public class EventHubRider :
        IEventHubRider
    {
        readonly IDictionary<string, IReceiveEndpointControl> _endpoints;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly IEvenHubProducerProviderFactory _producerProviderFactory;

        public EventHubRider(IEventHubHostConfiguration hostConfiguration, IDictionary<string, IReceiveEndpointControl> endpoints,
            IEvenHubProducerProviderFactory producerProviderFactory)
        {
            _hostConfiguration = hostConfiguration;
            _endpoints = endpoints;
            _producerProviderFactory = producerProviderFactory;
        }

        public IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default)
        {
            return _producerProviderFactory.GetProducerProvider(consumeContext);
        }

        public void Connect(IHost host)
        {
            host.AddReceiveEndpoint(_endpoints);
            //Host connect supervisor
        }
    }
}
