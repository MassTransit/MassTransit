namespace MassTransit.KafkaIntegration.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using MassTransit.Registration;
    using Pipeline.Observables;
    using Transport;


    public class KafkaBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly RiderObservable _observers;
        readonly IEnumerable<IKafkaProducerSpecification> _producers;
        readonly IEnumerable<IKafkaConsumerSpecification> _consumers;

        public KafkaBusInstanceSpecification(IEnumerable<IKafkaConsumerSpecification> consumers, IEnumerable<IKafkaProducerSpecification> producers,
            RiderObservable observers)
        {
            _consumers = consumers;
            _producers = producers;
            _observers = observers;
        }

        public void Configure(IBusInstance busInstance)
        {
            IKafkaReceiveEndpoint[] endpoints = _consumers
                .Select(x => x.CreateReceiveEndpoint(busInstance))
                .ToArray();

            IKafkaProducerFactory[] producers = _producers
                .Select(x => x.CreateProducerFactory(busInstance))
                .ToArray();

            busInstance.ConnectKafka(_observers, endpoints, producers);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            foreach (KeyValuePair<string, IKafkaConsumerSpecification[]> kv in _consumers.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToArray()))
            {
                if (kv.Value.Length > 1)
                    yield return this.Failure($"Topic: {kv.Key} was added more than once.");

                foreach (var result in kv.Value.SelectMany(x => x.Validate()))
                    yield return result;
            }

            foreach (var result in _producers.SelectMany(x => x.Validate()))
                yield return result;
        }
    }
}
