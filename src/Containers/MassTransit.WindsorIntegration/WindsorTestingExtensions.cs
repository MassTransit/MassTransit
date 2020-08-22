namespace MassTransit.Testing
{
    using System;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Registration;
    using WindsorIntegration;


    public static class WindsorTestingExtensions
    {
        public static IWindsorContainer AddMassTransitInMemoryTestHarness(this IWindsorContainer container,
            Action<IWindsorContainerBusConfigurator> configure = null)
        {
            container.Register(Component.For<InMemoryTestHarness>().UsingFactoryMethod(kernel =>
            {
                var busInstance = kernel.Resolve<IBusInstance>();

                if (busInstance is InMemoryTestHarnessBusInstance instance)
                    return instance.Harness;

                throw new ConfigurationException("Test Harness configuration is invalid");
            }).LifestyleSingleton());

            container.AddMassTransit(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.SetBusFactory(new InMemoryTestHarnessRegistrationBusFactory());
            });

            return container;
        }
    }
}
