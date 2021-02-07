namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using MassTransit.Registration;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using Transactions;
    using Transports;


    public class ContainerBuilderBusRegistrationConfigurator :
        RegistrationConfigurator,
        IContainerBuilderBusConfigurator
    {
        readonly AutofacContainerRegistrar _registrar;
        readonly HashSet<Type> _riderTypes;

        public ContainerBuilderBusRegistrationConfigurator(ContainerBuilder builder)
            : this(builder, new AutofacContainerRegistrar(builder))
        {
        }

        ContainerBuilderBusRegistrationConfigurator(ContainerBuilder builder, AutofacContainerRegistrar registrar)
            : base(registrar)
        {
            IBusRegistrationContext CreateRegistrationContext(IComponentContext context)
            {
                var provider = context.Resolve<IConfigurationServiceProvider>();
                var busHealth = context.Resolve<BusHealth>();
                return new BusRegistrationContext(provider, busHealth, Endpoints, Consumers, Sagas, ExecuteActivities, Activities, Futures);
            }

            Builder = builder;
            _registrar = registrar;
            _riderTypes = new HashSet<Type>();

            ScopeName = "message";

            builder.RegisterType<BusDepot>()
                .As<IBusDepot>()
                .SingleInstance();

            Builder.Register(context => new BusHealth())
                .As<BusHealth>()
                .As<IBusHealth>()
                .SingleInstance();

            Builder.Register(GetCurrentSendEndpointProvider)
                .As<ISendEndpointProvider>()
                .InstancePerLifetimeScope();

            Builder.Register(GetCurrentPublishEndpoint)
                .As<IPublishEndpoint>()
                .InstancePerLifetimeScope();

            Builder.Register(context => ClientFactoryProvider(context.Resolve<IConfigurationServiceProvider>(), context.Resolve<IBus>()))
                .As<IClientFactory>()
                .SingleInstance();

            builder.Register(CreateConsumerScopeProvider)
                .As<IConsumerScopeProvider>()
                .SingleInstance()
                .IfNotRegistered(typeof(IConsumerScopeProvider));

            builder.Register(context => new AutofacConfigurationServiceProvider(context.Resolve<ILifetimeScope>()))
                .As<IConfigurationServiceProvider>()
                .SingleInstance()
                .IfNotRegistered(typeof(IConfigurationServiceProvider));

            builder.Register(CreateRegistrationContext)
                .As<IBusRegistrationContext>()
                .SingleInstance();
        }

        public string ScopeName
        {
            private get => _registrar.ScopeName;
            set => _registrar.ScopeName = value;
        }

        public ContainerBuilder Builder { get; }

        public Action<ContainerBuilder, ConsumeContext> ConfigureScope
        {
            get => _registrar.ConfigureScope;
            set => _registrar.ConfigureScope = value;
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

            Builder.Register(context => CreateBus(busFactory, context))
                .As<IBusInstance>()
                .As<IReceiveEndpointConnector>()
                .SingleInstance();

            Builder.Register(context => context.Resolve<IBusInstance>().BusControl)
                .As<IBusControl>()
                .SingleInstance();

            Builder.Register(context => context.Resolve<IBusInstance>().Bus)
                .As<IBus>()
                .SingleInstance();
        }

        public void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            var configurator = new ContainerBuilderRiderConfigurator(Builder, _registrar, _riderTypes);
            configure?.Invoke(configurator);
        }

        static IBusInstance CreateBus<T>(T busFactory, IComponentContext context)
            where T : IRegistrationBusFactory
        {
            var specifications = context.Resolve<IEnumerable<IBusInstanceSpecification>>();

            var busInstance = busFactory.CreateBus(context.Resolve<IBusRegistrationContext>(), specifications);

            return busInstance;
        }

        IConsumerScopeProvider CreateConsumerScopeProvider(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacConsumerScopeProvider(lifetimeScopeProvider, ScopeName, ConfigureScope);
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IComponentContext context)
        {
            if (context.TryResolve(out ConsumeContext consumeContext))
                return consumeContext;

            var bus = context.ResolveOptional<ITransactionalBus>() ?? context.Resolve<IBus>();
            return new ScopedSendEndpointProvider<ILifetimeScope>(bus, context.Resolve<ILifetimeScope>());
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IComponentContext context)
        {
            if (context.TryResolve(out ConsumeContext consumeContext))
                return consumeContext;

            var bus = context.ResolveOptional<ITransactionalBus>() ?? context.Resolve<IBus>();
            return new PublishEndpoint(new ScopedPublishEndpointProvider<ILifetimeScope>(bus, context.Resolve<ILifetimeScope>()));
        }
    }
}
