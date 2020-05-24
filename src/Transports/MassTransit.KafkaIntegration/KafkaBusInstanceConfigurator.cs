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
        readonly IEnumerable<IKafkaSubscriptionDefinition> _definitions;

        public KafkaBusInstanceConfigurator(IEnumerable<IKafkaSubscriptionDefinition> definitions)
        {
            _definitions = definitions;
        }

        public void Configure(IBusInstance busInstance)
        {
            IKafkaSubscription[] subscriptions = _definitions
                .Select(x => x.Build(busInstance))
                .ToArray();
            busInstance.ConnectKafka(subscriptions);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_definitions == null || !_definitions.Any())
                return new[] {this.Failure("Definitions", "should not be empty")};

            return _definitions.SelectMany(x => x.Validate());
        }
    }
}
