namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Processors;
    using Registration;
    using Riders;


    public class EventHubBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly IEnumerable<IEventHubDefinition> _definitions;
        readonly RiderObservable _observer;

        public EventHubBusInstanceSpecification(IEnumerable<IEventHubDefinition> definitions, RiderObservable observer)
        {
            _definitions = definitions;
            _observer = observer;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_definitions == null || !_definitions.Any())
                yield return this.Failure("Topics", "should not be empty");

            foreach (KeyValuePair<string, IEventHubDefinition[]> kv in _definitions.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToArray()))
            {
                if (kv.Value.Length > 1)
                    yield return this.Failure($"EventHub: {kv.Key} was added more than once.");

                foreach (var result in kv.Value.SelectMany(x => x.Validate()))
                    yield return result;
            }
        }

        public void Configure(IBusInstance busInstance)
        {
            IEventHubReceiveEndpoint[] endpoints = _definitions
                .Select(x => x.Create(busInstance))
                .ToArray();
            busInstance.ConnectEventHub(_observer, endpoints);
        }
    }
}
