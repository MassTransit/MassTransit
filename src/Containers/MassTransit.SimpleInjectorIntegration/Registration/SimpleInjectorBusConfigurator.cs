namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using Context;
    using MassTransit.Registration;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using SimpleInjector;
    using Transactions;
    using Transports;


    public class SimpleInjectorBusConfigurator :
        RegistrationConfigurator,
        ISimpleInjectorBusConfigurator
    {
        readonly Lifestyle _hybridLifestyle;

        public SimpleInjectorBusConfigurator(Container container)
            : base(new SimpleInjectorContainerRegistrar(container))
        {
            IBusRegistrationContext CreateRegistrationContext()
            {
                var provider = Container.GetInstance<IConfigurationServiceProvider>();
                var busHealth = Container.GetInstance<BusHealth>();
                return new BusRegistrationContext(provider, busHealth, Endpoints, Consumers, Sagas, ExecuteActivities, Activities, Futures);
            }

            Container = container;

            _hybridLifestyle = Lifestyle.CreateHybrid(container.Options.DefaultScopedLifestyle, Lifestyle.Singleton);

            AddMassTransitComponents(Container);

            Container.RegisterSingleton(() => new BusHealth());

            Container.RegisterSingleton<IBusHealth>(() => Container.GetInstance<BusHealth>());

            Container.RegisterSingleton(() => CreateRegistrationContext());

            Container.RegisterSingleton(() => ClientFactoryProvider(Container.GetInstance<IConfigurationServiceProvider>(), Container.GetInstance<IBus>()));
        }

        public Container Container { get; }

        public void AddBus(Func<IBusControl> busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            AddBus(_ => busFactory());
        }

        public void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            SetBusFactory(new RegistrationBusFactory(busFactory));
        }

        public void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(SetBusFactory));

            Container.RegisterSingleton(() => busFactory.CreateBus(Container.GetInstance<IBusRegistrationContext>()));
            Container.RegisterSingleton<IReceiveEndpointConnector>(() => Container.GetInstance<IBusInstance>());
            Container.RegisterSingleton(() => Container.GetInstance<IBusInstance>().BusControl);
            Container.RegisterSingleton(() => Container.GetInstance<IBusInstance>().Bus);
        }

        public void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            throw new NotSupportedException("Riders are only supported with Microsoft DI and Autofac");
        }

        ISendEndpointProvider GetSendEndpointProvider()
        {
            return (ISendEndpointProvider)Container.GetConsumeContext()
                ?? new ScopedSendEndpointProvider<Container>(Container.TryGetInstance<ITransactionalBus>() ?? Container.GetInstance<IBus>(), Container);
        }

        IPublishEndpoint GetPublishEndpoint()
        {
            return (IPublishEndpoint)Container.GetConsumeContext()
                ?? new PublishEndpoint(new ScopedPublishEndpointProvider<Container>(
                    Container.TryGetInstance<ITransactionalBus>() ?? Container.GetInstance<IBus>(),
                    Container));
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
