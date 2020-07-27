namespace MassTransit.Testing
{
    using System;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using WindsorIntegration;


    public static class WindsorTestingExtensions
    {
        public static IWindsorContainer AddInMemoryTestHarness(this IWindsorContainer container, Action<IWindsorContainerBusConfigurator> configure = null)
        {
            container.Register(Component.For<InMemoryTestHarness>().UsingFactoryMethod(kernel =>
            {
                var testHarness = new InMemoryTestHarness();

                var busRegistrationContext = kernel.Resolve<IBusRegistrationContext>();
                testHarness.OnConfigureInMemoryBus += configurator => configurator.ConfigureEndpoints(busRegistrationContext);

                return testHarness;
            }).LifestyleSingleton());

            container.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.AddBus(context => context.GetRequiredService<InMemoryTestHarness>().BusControl);
            });

            return container;
        }
    }
}
