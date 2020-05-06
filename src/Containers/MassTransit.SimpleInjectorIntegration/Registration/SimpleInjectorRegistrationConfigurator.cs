namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using Context;
    using MassTransit.Registration;
    using Mediator;
    using ScopeProviders;
    using Scoping;
    using SimpleInjector;
    using Transports;


    public class SimpleInjectorRegistrationConfigurator :
        RegistrationConfigurator,
        ISimpleInjectorConfigurator
    {
        readonly string _name;
        readonly Lifestyle _hybridLifestyle;

        public SimpleInjectorRegistrationConfigurator(string name, Container container)
            : base(new SimpleInjectorContainerRegistrar(name, container))
        {
            _name = name;
            Container = container;

            _hybridLifestyle = Lifestyle.CreateHybrid(container.Options.DefaultScopedLifestyle, Lifestyle.Singleton);

            AddMassTransitComponents(Container);

            Container.RegisterInstance<IRegistrationConfigurator>(this);

            Container.RegisterSingleton(() => CreateRegistration(_name, container.GetInstance<IConfigurationServiceProvider>()));
        }

        public Container Container { get; }

        public void AddBus(Func<IBusControl> busFactory)
        {
            AddBus(_ => busFactory());
        }

        public void AddBus(Func<Container, IBusControl> busFactory)
        {
            IBusControl BusFactory()
            {
                var provider = Container.GetInstance<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return busFactory(Container);
            }

            Container.RegisterSingleton(BusFactory);

            Container.RegisterSingleton<IBus>(() => Container.GetInstance<IBusControl>());

            Container.Register(GetSendEndpointProvider, _hybridLifestyle);

            Container.Register(GetPublishEndpoint, _hybridLifestyle);

            Container.RegisterSingleton(() => ClientFactoryProvider(Container.GetInstance<IBus>()));
        }

        public void AddMediator(Action<Container, IReceiveEndpointConfigurator> configure = null)
        {
            IMediator MediatorFactory()
            {
                var provider = Container.GetInstance<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return Bus.Factory.CreateMediator(cfg =>
                {
                    configure?.Invoke(Container, cfg);

                    ConfigureMediator(_name, cfg, provider);
                });
            }

            Container.RegisterSingleton(MediatorFactory);

            Container.RegisterSingleton<IClientFactory>(() => Container.GetInstance<IMediator>());
        }

        ISendEndpointProvider GetSendEndpointProvider()
        {
            return (ISendEndpointProvider)Container.GetConsumeContext()
                ?? new ScopedSendEndpointProvider<Container>(Container.GetInstance<IBus>(), Container);
        }

        IPublishEndpoint GetPublishEndpoint()
        {
            return (IPublishEndpoint)Container.GetConsumeContext()
                ?? new PublishEndpoint(new ScopedPublishEndpointProvider<Container>(Container.GetInstance<IBus>(), Container));
        }

        static void AddMassTransitComponents(Container container)
        {
            container.Register<ScopedConsumeContextProvider>(Lifestyle.Scoped);

            container.Register(() => container.GetInstance<ScopedConsumeContextProvider>().GetContext("default") ?? new MissingConsumeContext(),
                Lifestyle.Scoped);

            container.RegisterSingleton<IConsumerScopeProvider>(() => new SimpleInjectorConsumerScopeProvider(container));
            container.RegisterSingleton<ISagaRepositoryFactory>(() => new SimpleInjectorSagaRepositoryFactory(container));
            container.RegisterSingleton<IConfigurationServiceProvider>(() => new SimpleInjectorConfigurationServiceProvider(container));
        }
    }
}
