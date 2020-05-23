namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using Registration;


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
    }
}
