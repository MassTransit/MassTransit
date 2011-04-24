namespace MassTransit.NinjectIntegration
{
    using System;
    using System.Linq;
    using Configuration;
    using Ninject;

    public static class MassTransitNinjectExtensions
    {
        public static UnsubscribeAction LoadConsumersFromContainer(this BusConfiguration cfg, IKernel kernel)
        {
            throw new NotImplementedException("Ninject doesn't yet support this feature.");

            var concrete = from b in kernel.GetBindings(typeof (IConsumer))
                           select b.Target;

            //cfg.AddSubscribersByType(concretes);

            return () => true;
        }
    }
}