namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using MassTransit.Registration;
    using Mediator;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using Transports;


    public class WindsorContainerRegistrationConfigurator :
        RegistrationConfigurator,
        IWindsorContainerConfigurator
    {
        readonly IWindsorContainer _container;

        public WindsorContainerRegistrationConfigurator(IWindsorContainer container)
            : base(new WindsorContainerRegistrar(container))
        {
            _container = container;

            if (!container.Kernel.HasComponent(typeof(IBusRegistry)))
                container.Register(Component.For<IBusRegistry>().ImplementedBy<BusRegistry>().LifestyleSingleton());

            container.RegisterScopedContextProviderIfNotPresent();

            container.Register(
                Component.For<IConsumerScopeProvider>().ImplementedBy<WindsorConsumerScopeProvider>().LifestyleTransient(),
                Component.For<IConfigurationServiceProvider>()
                    .ImplementedBy<WindsorConfigurationServiceProvider>()
                    .LifestyleSingleton(),
                Component.For<ISagaRepositoryFactory>()
                    .ImplementedBy<WindsorSagaRepositoryFactory>()
                    .LifestyleSingleton(),
                Component.For<IRegistrationConfigurator>()
                    .Instance(this)
                    .LifestyleSingleton(),
                Component.For<MassTransit.IRegistration>()
                    .UsingFactoryMethod(provider => CreateRegistration(provider.Resolve<IConfigurationServiceProvider>()))
                    .LifestyleSingleton()
            );
        }

        public IWindsorContainer Container => _container;

        public void AddBus(Func<IRegistrationContext<IKernel>, IBusControl> busFactory)
        {
            ThrowIfAlreadyConfigured();

            IBusControl BusFactory(IKernel kernel)
            {
                var provider = kernel.Resolve<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                IRegistrationContext<IKernel> context = GetRegistrationContext(kernel);

                return busFactory(context);
            }

            _container.Register(
                Component.For<IBusControl>()
                    .Forward<IBus>()
                    .UsingFactoryMethod(BusFactory)
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
                Component.For<IBusRegistryInstance>()
                    .ImplementedBy<BusRegistryInstance>()
                    .LifestyleSingleton()
            );
        }

        public void AddMediator(Action<IKernel, IReceiveEndpointConfigurator> configure = null)
        {
            ThrowIfAlreadyConfigured();

            IMediator MediatorFactory(IKernel kernel)
            {
                var provider = kernel.Resolve<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return Bus.Factory.CreateMediator(cfg =>
                {
                    configure?.Invoke(kernel, cfg);

                    ConfigureMediator(cfg, provider);
                });
            }

            _container.Register(
                Component.For<IMediator>()
                    .Forward<IClientFactory>()
                    .UsingFactoryMethod(MediatorFactory).LifestyleSingleton()
            );
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

        IRegistrationContext<IKernel> GetRegistrationContext(IKernel context)
        {
            return new RegistrationContext<IKernel>(
                CreateRegistration(context.Resolve<IConfigurationServiceProvider>()),
                context.Resolve<BusHealth>(),
                context
            );
        }
    }
}
