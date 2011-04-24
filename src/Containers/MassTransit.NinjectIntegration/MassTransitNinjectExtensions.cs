namespace MassTransit.NinjectIntegration
{
    using System;
    using Configuration;
    using Ninject;

    public static class MassTransitNinjectExtensions
    {
        public static UnsubscribeAction LoadConsumersFromContainer(this BusConfiguration cfg, IKernel kernel)
        {
            throw new NotImplementedException("Ninject doesn't support this feature. Github Issue https://github.com/ninject/ninject/issues/35");

            //cfg.AddSubscribersByType(concretes);

            return () => true;
        }
    }
}