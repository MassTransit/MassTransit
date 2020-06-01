namespace MassTransit.EventHubIntegration.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using MassTransit.Registration;
    using Riders;
    using Transport;


    public class EventHubBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly IEnumerable<IEventHubSpecification> _specifications;
        readonly RiderObservable _observer;

        public EventHubBusInstanceSpecification(IEnumerable<IEventHubSpecification> specifications, RiderObservable observer)
        {
            _specifications = specifications;
            _observer = observer;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_specifications == null || !_specifications.Any())
                yield return this.Failure("Topics", "should not be empty");

            foreach (KeyValuePair<string, IEventHubSpecification[]> kv in _specifications.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToArray()))
            {
                if (kv.Value.Length > 1)
                    yield return this.Failure($"EventHub: {kv.Key} was added more than once.");

                foreach (var result in kv.Value.SelectMany(x => x.Validate()))
                    yield return result;
            }
        }

        public void Configure(IBusInstance busInstance)
        {
            IEventHubReceiveEndpoint[] endpoints = _specifications
                .Select(x => x.Create(busInstance))
                .ToArray();
            busInstance.ConnectEventHub(_observer, endpoints);
        }
    }
}
