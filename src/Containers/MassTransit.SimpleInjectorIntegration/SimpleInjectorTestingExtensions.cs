namespace MassTransit.Testing
{
    using System;
    using Registration;
    using SimpleInjector;
    using SimpleInjectorIntegration;


    public static class SimpleInjectorTestingExtensions
    {
        public static Container AddMassTransitInMemoryTestHarness(this Container container,
            Action<ISimpleInjectorBusConfigurator> configure = null)
        {
            container.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.SetBusFactory(new InMemoryTestHarnessRegistrationBusFactory());
            });
            container.RegisterSingleton(() =>
            {
                var busInstance = container.GetInstance<IBusInstance>();

                if (busInstance is InMemoryTestHarnessBusInstance instance)
                    return instance.Harness;

                throw new ConfigurationException("Test Harness configuration is invalid");
            });

            return container;
        }
    }
}
