namespace MassTransit.WindsorIntegration
{
    using System.Linq;
    using Castle.Windsor;
    using Configuration;

    public static class MassTransitWindsorContainerExtensions
    {
        public static UnsubscribeAction LoadConsumersFromContainer(this BusConfiguration cfg, IWindsorContainer container)
        {
            var concretes = from h in container.Kernel.GetHandlers(typeof (IConsumer))
                           select h.ComponentModel.Implementation;

            //cfg.AddSubscribersByType(concretes);

            return () => true;
        }
    }
}