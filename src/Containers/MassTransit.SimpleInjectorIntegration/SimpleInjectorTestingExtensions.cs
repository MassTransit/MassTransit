namespace MassTransit.Testing
{
    using System;
    using SimpleInjector;
    using SimpleInjectorIntegration;


    public static class SimpleInjectorTestingExtensions
    {
        public static Container AddInMemoryTestHarness(this Container container,
            Action<ISimpleInjectorBusConfigurator> configure = null)
        {
            container.RegisterSingleton(() =>
            {
                var testHarness = new InMemoryTestHarness();

                var busRegistrationContext = container.GetInstance<IBusRegistrationContext>();
                testHarness.OnConfigureInMemoryBus += configurator => configurator.ConfigureEndpoints(busRegistrationContext);

                return testHarness;
            });

            container.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.AddBus(context => context.GetRequiredService<InMemoryTestHarness>().BusControl);
            });

            return container;
        }
    }
}
