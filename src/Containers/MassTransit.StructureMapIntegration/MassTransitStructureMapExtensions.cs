namespace MassTransit.StructureMapIntegration
{
    using System.Linq;
    using Configuration;
    using Magnum.Extensions;
    using StructureMap;

    public static class MassTransitStructureMapExtensions
    {
        public static UnsubscribeAction LoadConsumersFromContainer(this BusConfiguration cfg, IContainer container)
        {
            var concreteTypes = from i in container.Model.InstancesOf<IConsumer>()
                                select i.ConcreteType;

            //cfg.AddSubscribersByType(concretes);
            return () => true;
        }
    }
}