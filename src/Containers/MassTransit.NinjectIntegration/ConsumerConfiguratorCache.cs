namespace MassTransit.NinjectIntegration
{
    using System;
    using Ninject;


    public static class ConsumerConfiguratorCache
    {
        public static void Configure(Type consumerType, IReceiveEndpointConfigurator configurator, IKernel kernel)
        {
            Cached.Registry.Add(consumerType).Configure(configurator, kernel);
        }


        static class Cached
        {
            internal static readonly IConsumerRegistry Registry = new ConsumerRegistry();
        }
    }
}
