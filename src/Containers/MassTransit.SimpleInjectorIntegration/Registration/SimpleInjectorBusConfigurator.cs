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


    public class SimpleInjectorBusConfigurator :
        RegistrationConfigurator,
        ISimpleInjectorBusConfigurator
    {
        readonly Lifestyle _hybridLifestyle;

        public SimpleInjectorBusConfigurator(Container container)
            : base(new SimpleInjectorContainerRegistrar(container))
        {
            Container = container;

            _hybridLifestyle = Lifestyle.CreateHybrid(container.Options.DefaultScopedLifestyle, Lifestyle.Singleton);

            AddMassTransitComponents(Container);

            Container.RegisterSingleton(() => new BusHealth(nameof(IBus)));

            Container.RegisterSingleton<IBusHealth>(() => Container.GetInstance<BusHealth>());

            Container.RegisterSingleton<IBusInstance, DefaultBusInstance>();

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

        public void AddBus(Func<IRegistrationContext, IBusControl> busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(AddBus));

            IBusControl BusFactory()
            {
                var provider = Container.GetInstance<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                var context = GetRegistrationContext();

                return busFactory(context);
            }

            Container.RegisterSingleton(BusFactory);
            Container.RegisterSingleton<IBus>(() => Container.GetInstance<IBusControl>());
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

        IRegistrationContext GetRegistrationContext()
        {
            return new RegistrationContext(
                CreateRegistration(Container.GetInstance<IConfigurationServiceProvider>()),
                Container.GetInstance<BusHealth>()
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
