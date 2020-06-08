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
                Component.For<MassTransit.IRegistration>()
                    .UsingFactoryMethod(provider => CreateRegistration(provider.Resolve<IConfigurationServiceProvider>()))
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

        public void AddBus(Func<IRegistrationContext, IBusControl> busFactory)
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

        IBusControl BusFactory(IKernel kernel, Func<IRegistrationContext, IBusControl> busFactory)
        {
            var provider = kernel.Resolve<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            var context = GetRegistrationContext(kernel);

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

        IRegistrationContext GetRegistrationContext(IKernel context)
        {
            return new RegistrationContext(
                CreateRegistration(context.Resolve<IConfigurationServiceProvider>()),
                context.Resolve<BusHealth>()
            );
        }
    }
}
