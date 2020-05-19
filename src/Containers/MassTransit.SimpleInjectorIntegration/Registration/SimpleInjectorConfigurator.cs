namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using Context;
    using MassTransit.Registration;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using SimpleInjector;
    using Transports;


    public class SimpleInjectorConfigurator :
        RegistrationConfigurator,
        ISimpleInjectorConfigurator
    {
        readonly Lifestyle _hybridLifestyle;

        public SimpleInjectorConfigurator(Container container)
            : base(new SimpleInjectorContainerRegistrar(container))
        {
            Container = container;

            _hybridLifestyle = Lifestyle.CreateHybrid(container.Options.DefaultScopedLifestyle, Lifestyle.Singleton);

            AddMassTransitComponents(Container);

            Container.RegisterSingleton(() => new BusHealth(nameof(IBus)));

            Container.RegisterSingleton<IBusHealth>(() => Container.GetInstance<BusHealth>());

            Container.RegisterSingleton<IBusRegistryInstance, BusRegistryInstance>();

            Container.RegisterSingleton(() => CreateRegistration(container.GetInstance<IConfigurationServiceProvider>()));

            Container.RegisterSingleton(() => ClientFactoryProvider(Container.GetInstance<IConfigurationServiceProvider>(), Container.GetInstance<IBus>()));
        }

        public Container Container { get; }

        public void AddBus(Func<IBusControl> busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            AddBus(_ => busFactory());
        }

        public void AddBus(Func<IRegistrationContext<Container>, IBusControl> busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured();

            IBusControl BusFactory()
            {
                var provider = Container.GetInstance<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                IRegistrationContext<Container> context = GetRegistrationContext();

                return busFactory(context);
            }

            Container.RegisterSingleton(BusFactory);
            Container.RegisterSingleton<IBus>(() => Container.GetInstance<IBusControl>());
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

        IRegistrationContext<Container> GetRegistrationContext()
        {
            return new RegistrationContext<Container>(
                CreateRegistration(Container.GetInstance<IConfigurationServiceProvider>()),
                Container.GetInstance<BusHealth>(),
                Container
            );
        }

        void AddMassTransitComponents(Container container)
        {
            container.Register<ScopedConsumeContextProvider>(Lifestyle.Scoped);

            container.Register(() => container.GetInstance<ScopedConsumeContextProvider>().GetContext() ?? new MissingConsumeContext(),
                Lifestyle.Scoped);

            container.Register(GetSendEndpointProvider, _hybridLifestyle);

            container.Register(GetPublishEndpoint, _hybridLifestyle);

            container.RegisterSingleton<IConsumerScopeProvider>(() => new SimpleInjectorConsumerScopeProvider(container));
            container.RegisterSingleton<IConfigurationServiceProvider>(() => new SimpleInjectorConfigurationServiceProvider(container));
        }
    }
}
