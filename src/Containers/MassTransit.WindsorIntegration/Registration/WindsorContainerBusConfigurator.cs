namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using MassTransit.Registration;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using Transports;


    public class WindsorContainerBusConfigurator :
        RegistrationConfigurator,
        IWindsorContainerBusConfigurator
    {
        public WindsorContainerBusConfigurator(IWindsorContainer container)
            : base(new WindsorContainerRegistrar(container))
        {
            IBusRegistrationContext CreateRegistrationContext(IKernel context)
            {
                var provider = context.Resolve<IConfigurationServiceProvider>();
                var busHealth = context.Resolve<BusHealth>();
                return new BusRegistrationContext(provider, busHealth, EndpointRegistrations, ConsumerRegistrations, SagaRegistrations,
                    ExecuteActivityRegistrations, ActivityRegistrations);
            }

            Container = container;

            if (!container.Kernel.HasComponent(typeof(IBusDepot)))
                container.Register(Component.For<IBusDepot>().ImplementedBy<BusDepot>().LifestyleSingleton());

            container.RegisterScopedContextProviderIfNotPresent();

            if (!container.Kernel.HasComponent(typeof(IConsumerScopeProvider)))
            {
                container.Register(Component.For<IConsumerScopeProvider>()
                    .ImplementedBy<WindsorConsumerScopeProvider>()
                    .LifestyleTransient());
            }

            if (!container.Kernel.HasComponent(typeof(IConfigurationServiceProvider)))
            {
                container.Register(Component.For<IConfigurationServiceProvider>()
                    .ImplementedBy<WindsorConfigurationServiceProvider>()
                    .LifestyleSingleton());
            }

            container.Register(
                Component.For<IBusRegistrationContext>()
                    .UsingFactoryMethod(provider => CreateRegistrationContext(provider))
                    .LifestyleSingleton(),
                Component.For<ISendEndpointProvider>()
                    .UsingFactoryMethod(GetCurrentSendEndpointProvider)
                    .LifestyleTransient(),
                Component.For<IPublishEndpoint>()
                    .UsingFactoryMethod(GetCurrentPublishEndpoint)
                    .LifestyleTransient(),
                Component.For<IClientFactory>()
                    .UsingFactoryMethod(kernel => ClientFactoryProvider(kernel.Resolve<IConfigurationServiceProvider>(), kernel.Resolve<IBus>()))
                    .LifestyleSingleton(),
                Component.For<BusHealth>()
                    .Forward<IBusHealth>()
                    .UsingFactoryMethod(() => new BusHealth(nameof(IBus)))
                    .LifestyleSingleton(),
                Component.For<IBusInstance>()
                    .ImplementedBy<DefaultBusInstance>()
                    .LifestyleSingleton()
            );
        }

        public IWindsorContainer Container { get; }

        public void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(AddBus));

            Container.Register(
                Component.For<IBusControl>()
                    .Forward<IBus>()
                    .UsingFactoryMethod(kernel => BusFactory(kernel, busFactory))
                    .LifestyleSingleton());
        }

        public void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory
        {
            throw new NotImplementedException();
        }

        public void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            throw new NotImplementedException();
        }

        IBusControl BusFactory(IKernel kernel, Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            var provider = kernel.Resolve<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            var context = kernel.Resolve<IBusRegistrationContext>();

            return busFactory(context);
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IKernel context)
        {
            return (ISendEndpointProvider)context.GetConsumeContext()
                ?? new ScopedSendEndpointProvider<IKernel>(context.Resolve<IBus>(), context);
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IKernel context)
        {
            return (IPublishEndpoint)context.GetConsumeContext()
                ?? new PublishEndpoint(new ScopedPublishEndpointProvider<IKernel>(context.Resolve<IBus>(), context));
        }
    }
}
