namespace MassTransit.EventHubIntegration.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using MassTransit.Registration;


    public class EventHubBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly IEventHubProducerSpecification _producerSpecification;
        readonly IEnumerable<IEventHubReceiveEndpointSpecification> _specifications;

        public EventHubBusInstanceSpecification(IEnumerable<IEventHubReceiveEndpointSpecification> specifications,
            IEventHubProducerSpecification producerSpecification)
        {
            _specifications = specifications;
            _producerSpecification = producerSpecification;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            foreach (KeyValuePair<string, IEventHubReceiveEndpointSpecification[]> kv in _specifications.GroupBy(x => x.EndpointName)
                .ToDictionary(x => x.Key, x => x.ToArray()))
            {
                if (kv.Value.Length > 1)
                    yield return this.Failure($"EventHub: {kv.Key} was added more than once.");

                foreach (var result in kv.Value.SelectMany(x => x.Validate()))
                    yield return result;
            }

            foreach (var result in _producerSpecification.Validate())
                yield return result;
        }

        public void Configure(IBusInstance busInstance)
        {
            IDictionary<string, IReceiveEndpointControl> endpoints = _specifications
                .ToDictionary(x => x.EndpointName, x => x.CreateReceiveEndpoint(busInstance));
            busInstance.ConnectEventHub(() => _producerSpecification.CreateContext(busInstance), endpoints);
        }
    }
}
