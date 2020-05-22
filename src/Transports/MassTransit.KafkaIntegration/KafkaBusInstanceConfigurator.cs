namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Registration;
    using Subscriptions;


    public class KafkaBusInstanceConfigurator :
        IBusInstanceConfigurator
    {
        readonly IEnumerable<IKafkaTopic> _topics;

        public KafkaBusInstanceConfigurator(IEnumerable<IKafkaTopic> topics)
        {
            _topics = topics;
        }

        public void Configure(IBusInstance busInstance)
        {
            IKafkaConsumer[] consumers = _topics
                .Select(x => x.CreateConsumer(busInstance))
                .ToArray();
            busInstance.ConnectKafka(consumers);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_topics == null || !_topics.Any())
                return new[] {this.Failure("Definitions", "should not be empty")};

            return _topics.SelectMany(x => x.Validate());
        }
    }
}
