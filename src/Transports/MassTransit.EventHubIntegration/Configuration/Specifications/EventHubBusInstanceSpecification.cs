namespace MassTransit.EventHubIntegration.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using MassTransit.Registration;
    using Pipeline.Observables;


    public class EventHubBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly RiderObservable _observer;
        readonly IEventHubProducerSpecification _producerSpecification;
        readonly IEnumerable<IEventHubReceiveEndpointSpecification> _specifications;

        public EventHubBusInstanceSpecification(IEnumerable<IEventHubReceiveEndpointSpecification> specifications,
            IEventHubProducerSpecification producerSpecification, RiderObservable observer)
        {
            _specifications = specifications;
            _producerSpecification = producerSpecification;
            _observer = observer;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            foreach (KeyValuePair<string, IEventHubReceiveEndpointSpecification[]> kv in _specifications.GroupBy(x => x.Name)
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
            IEventHubReceiveEndpoint[] endpoints = _specifications
                .Select(x => x.Create(busInstance))
                .ToArray();
            var sharedContext = _producerSpecification.CreateContext(busInstance);
            busInstance.ConnectEventHub(_observer, sharedContext, endpoints);
        }
    }
}
