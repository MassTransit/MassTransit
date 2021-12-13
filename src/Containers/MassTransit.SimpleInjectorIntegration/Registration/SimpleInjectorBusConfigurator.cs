namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using System.Collections.Generic;
    using AutofacIntegration;
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
        protected readonly HashSet<Type> RiderTypes;

        static bool _masstransit_components_added = false;

        public SimpleInjectorBusConfigurator(Container container)
            : this(container, new SimpleInjectorContainerRegistrar(container))
        {
            IBusRegistrationContext CreateRegistrationContext()
            {
                var provider = Container.GetInstance<IConfigurationServiceProvider>();
                return new BusRegistrationContext(provider, Endpoints, Consumers, Sagas, ExecuteActivities, Activities, Futures);
            }

            Container.RegisterSingleton(() => Bind<IBus>.Create(CreateRegistrationContext()));
            Container.RegisterSingleton(() => Container.GetInstance<Bind<IBus, IBusRegistrationContext>>().Value);

            Container.RegisterSingleton(() => ClientFactoryProvider(Container.GetInstance<IConfigurationServiceProvider>(), Container.GetInstance<IBus>()));
        }

        protected SimpleInjectorBusConfigurator(Container container, SimpleInjectorContainerRegistrar registrar)
            : base(registrar)
        {
            Container = container;
            RiderTypes = new HashSet<Type>();

            _hybridLifestyle = Lifestyle.CreateHybrid(container.Options.DefaultScopedLifestyle, Lifestyle.Singleton);
            AddMassTransitComponents(Container);
        }

        public Container Container { get; }

        public void AddBus(Func<IBusControl> busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            AddBus(_ => busFactory());
        }

        public virtual void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            SetBusFactory(new RegistrationBusFactory(busFactory));
        }

        public virtual void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(SetBusFactory));

            Container.RegisterSingleton(() => busFactory.CreateBus(Container.GetInstance<IBusRegistrationContext>()));
            Container.RegisterSingleton<IReceiveEndpointConnector>(() => Container.GetInstance<IBusInstance>());
            Container.RegisterSingleton(() => Container.GetInstance<IBusInstance>().BusControl);
            Container.RegisterSingleton(() => Container.GetInstance<IBusInstance>().Bus);

            Registrar.RegisterScopedClientFactory();

        #pragma warning disable 618
            Container.RegisterSingleton<IBusHealth>(() => new BusHealth(Container.GetInstance<IBusInstance>()));
        }

        public virtual void AddRider(Action<IRiderRegistrationConfigurator> configure)
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
            if (_masstransit_components_added)
                return;

            container.RegisterSingleton<IBusDepot, BusDepot>();

            container.Register<ScopedConsumeContextProvider>(Lifestyle.Scoped);
            container.Register(() => container.GetInstance<ScopedConsumeContextProvider>().GetContext() ?? new MissingConsumeContext(),
                Lifestyle.Scoped);

            container.Register(GetSendEndpointProvider, _hybridLifestyle);
            container.Register(GetPublishEndpoint, _hybridLifestyle);

            container.RegisterSingleton<IConsumerScopeProvider>(() => new SimpleInjectorConsumerScopeProvider(container));
            container.RegisterSingleton<IConfigurationServiceProvider>(() => new SimpleInjectorConfigurationServiceProvider(container));

            _masstransit_components_added = true;
        }
    }
}
