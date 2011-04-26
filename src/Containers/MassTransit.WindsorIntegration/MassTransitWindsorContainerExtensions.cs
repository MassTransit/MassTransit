namespace MassTransit.WindsorIntegration
{
    using System.Linq;
    using Castle.Windsor;
    using BusConfigurators;

    public static class MassTransitWindsorContainerExtensions
    {
        public static UnsubscribeAction LoadConsumersFromContainer(this ServiceBusConfigurator cfg, IWindsorContainer container)
        {
            var concretes = from h in container.Kernel.GetHandlers(typeof (IConsumer))
                           select h.ComponentModel.Implementation;

            concretes.Each(cfg.RegisterSubscription);

            return () => true;
        }
    }
}