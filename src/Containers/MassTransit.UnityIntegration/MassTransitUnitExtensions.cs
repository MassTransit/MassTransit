using MassTransit.BusConfigurators;

namespace MassTransit.UnityIntegration
{
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Magnum.Extensions;

    public static class MassTransitUnitExtensions
    {
        public static UnsubscribeAction LoadConsumersFromContainer(this ServiceBusConfigurator cfg, UnityContainer container)
        {
            var concretes = from r in container.Registrations
                            where r.MappedToType.Implements<IConsumer>()
                            select r.RegisteredType;

            concretes.Each(cfg.RegisterSubscription);

            return () => true;
        }
    }
}