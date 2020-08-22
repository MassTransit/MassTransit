namespace MassTransit.AutofacIntegration
{
    using System;
    using Autofac;
    using MassTransit.Registration;
    using Testing;


    public static class AutofacTestingExtensions
    {
        /// <summary>
        /// Add the In-Memory test harness to the container, and configure it using the callback specified.
        /// </summary>
        public static ContainerBuilder AddMassTransitInMemoryTestHarness(this ContainerBuilder builder,
            Action<IContainerBuilderBusConfigurator> configure = null)
        {
            builder.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.SetBusFactory(new InMemoryTestHarnessRegistrationBusFactory());
            });
            builder.Register(provider =>
                {
                    var busInstance = provider.Resolve<IBusInstance>();

                    if (busInstance is InMemoryTestHarnessBusInstance instance)
                        return instance.Harness;

                    throw new ConfigurationException("Test Harness configuration is invalid");
                })
                .SingleInstance()
                .As<InMemoryTestHarness>()
                .As<BusTestHarness>();

            return builder;
        }
    }
}
