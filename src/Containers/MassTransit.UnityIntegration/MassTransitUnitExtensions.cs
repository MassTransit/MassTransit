namespace MassTransit.UnityIntegration
{
    using System.Linq;
    using Configuration;
    using Microsoft.Practices.Unity;
    using Magnum.Extensions;

    public static class MassTransitUnitExtensions
    {
        public static UnsubscribeAction LoadConsumersFromContainer(this BusConfiguration cfg, UnityContainer container)
        {
            var concretes = from r in container.Registrations
                            where r.MappedToType.Implements<IConsumer>()
                            select r.RegisteredType;

            //cfg.AddSubscribersByType(concretes);
            return () => true;
        }
    }
}