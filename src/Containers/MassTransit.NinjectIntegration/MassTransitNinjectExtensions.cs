namespace MassTransit.NinjectIntegration
{
    using System;
    using Ninject;
    using SubscriptionConfigurators;

    public static class MassTransitNinjectExtensions
    {
        public static UnsubscribeAction LoadFrom(this SubscriptionBusServiceConfigurator cfg, IKernel kernel)
        {
            throw new NotImplementedException("Ninject doesn't support this feature. Github Issue https://github.com/ninject/ninject/issues/35");

            //cfg.AddSubscribersByType(concretes);

            return () => true;
        }
    }
}