namespace MassTransit.Testing
{
    using System;
    using Registration;
    using StructureMap;
    using StructureMapIntegration;


    public static class StructureMapTestingExtensions
    {
        public static ConfigurationExpression AddMassTransitInMemoryTestHarness(this ConfigurationExpression configuration,
            Action<IConfigurationExpressionBusConfigurator> configure = null)
        {
            configuration.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.SetBusFactory(new InMemoryTestHarnessRegistrationBusFactory());
            });

            configuration.For<InMemoryTestHarness>().Use(provider => CreateInMemoryTestHarness(provider)).Singleton();

            return configuration;
        }

        static InMemoryTestHarness CreateInMemoryTestHarness(IContext provider)
        {
            var busInstance = provider.GetInstance<IBusInstance>();

            if (busInstance is InMemoryTestHarnessBusInstance instance)
                return instance.Harness;

            throw new ConfigurationException("Test Harness configuration is invalid");
        }
    }
}
