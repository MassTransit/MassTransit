namespace MassTransit.Testing
{
    using System;
    using StructureMap;
    using StructureMapIntegration;


    public static class StructureMapTestingExtensions
    {
        public static ConfigurationExpression AddMassTransitInMemoryTestHarness(this ConfigurationExpression configuration,
            Action<IConfigurationExpressionBusConfigurator> configure = null)
        {
            configuration.For<InMemoryTestHarness>().Use(provider => CreateInMemoryTestHarness(provider)).Singleton();

            configuration.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.AddBus(context => context.GetRequiredService<InMemoryTestHarness>().BusControl);
            });

            return configuration;
        }

        static InMemoryTestHarness CreateInMemoryTestHarness(IContext provider)
        {
            var testHarness = new InMemoryTestHarness();

            var busRegistrationContext = provider.GetInstance<IBusRegistrationContext>();
            testHarness.OnConfigureInMemoryBus += configurator => configurator.ConfigureEndpoints(busRegistrationContext);

            return testHarness;
        }
    }
}
