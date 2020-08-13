namespace MassTransit.AutofacIntegration
{
    using System;
    using Autofac;
    using Testing;


    public static class AutofacTestingExtensions
    {
        /// <summary>
        /// Add the In-Memory test harness to the container, and configure it using the callback specified.
        /// </summary>
        public static ContainerBuilder AddMassTransitInMemoryTestHarness(this ContainerBuilder builder,
            Action<IContainerBuilderBusConfigurator> configure = null)
        {
            builder.Register(provider =>
            {
                var testHarness = new InMemoryTestHarness();

                var busRegistrationContext = provider.Resolve<IBusRegistrationContext>();
                testHarness.OnConfigureInMemoryBus += configurator => configurator.ConfigureEndpoints(busRegistrationContext);

                return testHarness;
            }).SingleInstance();

            builder.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.AddBus(context => context.GetRequiredService<InMemoryTestHarness>().BusControl);
            });

            return builder;
        }
    }
}
