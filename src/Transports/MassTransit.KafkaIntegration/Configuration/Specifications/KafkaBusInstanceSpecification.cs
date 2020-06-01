namespace MassTransit.KafkaIntegration.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using MassTransit.Registration;
    using Riders;
    using Transport;


    public class KafkaBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly RiderObservable _observers;
        readonly IEnumerable<IKafkaTopicSpecification> _topics;

        public KafkaBusInstanceSpecification(IEnumerable<IKafkaTopicSpecification> topics, RiderObservable observers)
        {
            _topics = topics;
            _observers = observers;
        }

        public void Configure(IBusInstance busInstance)
        {
            IKafkaReceiveEndpoint[] endpoints = _topics
                .Select(x => x.CreateEndpoint(busInstance))
                .ToArray();
            busInstance.ConnectKafka(_observers, endpoints);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_topics == null || !_topics.Any())
                yield return this.Failure("Topics", "should not be empty");

            foreach (KeyValuePair<string, IKafkaTopicSpecification[]> kv in _topics.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToArray()))
            {
                if (kv.Value.Length > 1)
                    yield return this.Failure($"Topic: {kv.Key} was added more than once.");

                foreach (var result in kv.Value.SelectMany(x => x.Validate()))
                    yield return result;
            }
        }
    }
}
