namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using MassTransit.Registration;
    using Mediator;
    using ScopeProviders;
    using Scoping;
    using Transports;


    public class WindsorContainerRegistrationConfigurator :
        RegistrationConfigurator,
        IWindsorContainerConfigurator
    {
        readonly string _name;
        readonly IWindsorContainer _container;

        public WindsorContainerRegistrationConfigurator(string name, IWindsorContainer container)
            : base(new WindsorContainerRegistrar(name, container))
        {
            _name = name;
            _container = container;

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
                    .UsingFactoryMethod(provider => CreateRegistration(_name, provider.Resolve<IConfigurationServiceProvider>()))
                    .LifestyleSingleton()
            );
        }

        public IWindsorContainer Container => _container;

        public void AddBus(Func<IKernel, IBusControl> busFactory)
        {
            IBusControl BusFactory(IKernel kernel)
            {
                var provider = kernel.Resolve<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return busFactory(kernel);
            }

            _container.Register(
                Component.For<IBusControl>()
                    .Forward<IBus>()
                    .UsingFactoryMethod(BusFactory).LifestyleSingleton(),
                Component.For<ISendEndpointProvider>()
                    .UsingFactoryMethod(GetCurrentSendEndpointProvider)
                    .LifestyleTransient(),
                Component.For<IPublishEndpoint>()
                    .UsingFactoryMethod(GetCurrentPublishEndpoint)
                    .LifestyleTransient(),
                Component.For<IClientFactory>()
                    .UsingFactoryMethod(kernel => ClientFactoryProvider(kernel.Resolve<IBus>()))
                    .LifestyleSingleton()
            );
        }

        public void AddMediator(Action<IKernel, IReceiveEndpointConfigurator> configure = null)
        {
            IMediator MediatorFactory(IKernel kernel)
            {
                var provider = kernel.Resolve<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return Bus.Factory.CreateMediator(cfg =>
                {
                    configure?.Invoke(kernel, cfg);

                    ConfigureMediator(_name, cfg, provider);
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
    }
}
