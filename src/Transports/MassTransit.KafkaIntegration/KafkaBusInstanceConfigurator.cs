namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using Attachments;
    using GreenPipes;
    using Registration;
    using Subscriptions;


    public class KafkaBusInstanceConfigurator :
        IBusInstanceConfigurator
    {
        readonly BusAttachmentObservable _observers;
        readonly IEnumerable<IKafkaTopic> _topics;

        public KafkaBusInstanceConfigurator(IEnumerable<IKafkaTopic> topics, BusAttachmentObservable observers)
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
                return new[] {this.Failure("Definitions", "should not be empty")};

            return _topics.SelectMany(x => x.Validate());
        }
    }
}
