namespace MassTransit.NinjectIntegration
{
    using System;
    using Ninject;


    public static class SagaConfiguratorCache
    {
        public static void Configure(Type sagaType, IReceiveEndpointConfigurator configurator, IKernel kernel)
        {
            Cached.Registry.Add(sagaType).Configure(configurator, kernel);
        }


        static class Cached
        {
            internal static readonly ISagaRegistry Registry = new SagaRegistry();
        }
    }
}
