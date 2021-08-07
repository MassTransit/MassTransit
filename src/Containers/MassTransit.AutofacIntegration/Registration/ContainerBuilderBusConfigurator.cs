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
        protected readonly HashSet<Type> RiderTypes;

        public ContainerBuilderBusRegistrationConfigurator(ContainerBuilder builder)
            : this(builder, new AutofacContainerRegistrar(builder))
        {
            IBusRegistrationContext CreateRegistrationContext(IComponentContext context)
            {
                var provider = context.Resolve<IConfigurationServiceProvider>();
                return new BusRegistrationContext(provider, Endpoints, Consumers, Sagas, ExecuteActivities, Activities, Futures);
            }

            builder.Register(context => ClientFactoryProvider(context.Resolve<IConfigurationServiceProvider>(), context.Resolve<IBus>()))
                .As<IClientFactory>()
                .SingleInstance();

            builder.Register(context => Bind<IBus>.Create(CreateRegistrationContext(context)))
                .As<Bind<IBus, IBusRegistrationContext>>()
                .SingleInstance();
            builder.Register(context => context.Resolve<Bind<IBus, IBusRegistrationContext>>().Value)
                .As<IBusRegistrationContext>()
                .SingleInstance();
        }

        protected ContainerBuilderBusRegistrationConfigurator(ContainerBuilder builder, AutofacContainerRegistrar registrar)
            : base(registrar)
        {
            Builder = builder;
            _registrar = registrar;
            RiderTypes = new HashSet<Type>();

            ScopeName = "message";

            builder.RegisterType<BusDepot>()
                .As<IBusDepot>()
                .SingleInstance();

            builder.Register(GetCurrentSendEndpointProvider)
                .As<ISendEndpointProvider>()
                .InstancePerLifetimeScope();

            builder.Register(GetCurrentPublishEndpoint)
                .As<IPublishEndpoint>()
                .InstancePerLifetimeScope();


            builder.Register(CreateConsumerScopeProvider)
                .As<IConsumerScopeProvider>()
                .SingleInstance()
                .IfNotRegistered(typeof(IConsumerScopeProvider));

            builder.Register(context => new AutofacConfigurationServiceProvider(context.Resolve<ILifetimeScope>()))
                .As<IConfigurationServiceProvider>()
                .SingleInstance()
                .IfNotRegistered(typeof(IConfigurationServiceProvider));
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

            Builder.Register(context => Bind<IBus>.Create(CreateBus(busFactory, context)))
                .As<Bind<IBus, IBusInstance>>()
                .SingleInstance();

            Builder.Register(context => context.Resolve<Bind<IBus, IBusInstance>>().Value)
                .As<IBusInstance>()
                .As<IReceiveEndpointConnector>()
                .SingleInstance();

            Builder.Register(context => context.Resolve<Bind<IBus, IBusInstance>>().Value.BusControl)
                .As<IBusControl>()
                .SingleInstance();

            Builder.Register(context => context.Resolve<Bind<IBus, IBusInstance>>().Value.Bus)
                .As<IBus>()
                .SingleInstance();

        #pragma warning disable 618
            Builder.Register(context => new BusHealth(context.Resolve<Bind<IBus, IBusInstance>>().Value))
                .As<IBusHealth>()
                .SingleInstance();
        }

        public virtual void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            var configurator = new ContainerBuilderRiderConfigurator(Builder, _registrar, RiderTypes);
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
