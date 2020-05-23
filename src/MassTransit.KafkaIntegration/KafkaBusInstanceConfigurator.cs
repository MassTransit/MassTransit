namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using Registration;


    public class KafkaBusInstanceConfigurator :
        IBusInstanceConfigurator
    {
        readonly IEnumerable<IKafkaSubscriptionDefinition> _definitions;
        readonly IRegistration _registration;

        public KafkaBusInstanceConfigurator(IEnumerable<IKafkaSubscriptionDefinition> definitions, IRegistration registration)
        {
            _definitions = definitions;
            _registration = registration;
        }

        public void Configure(IBusInstance busInstance)
        {
            IKafkaSubscription[] subscriptions = _definitions
                .Select(x => x.Build(busInstance, _registration))
                .ToArray();
            busInstance.ConnectKafka(subscriptions);
        }
    }
}
