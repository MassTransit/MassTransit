namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using MassTransit.Registration;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using Transports;


    public class ContainerBuilderRegistrationConfigurator :
        RegistrationConfigurator,
        IContainerBuilderConfigurator
    {
        readonly ContainerBuilder _builder;
        readonly AutofacContainerRegistrar _registrar;

        public ContainerBuilderRegistrationConfigurator(ContainerBuilder builder)
            : this(builder, new AutofacContainerRegistrar(builder))
        {
        }

        ContainerBuilderRegistrationConfigurator(ContainerBuilder builder, AutofacContainerRegistrar registrar)
            : base(registrar)
        {
            _builder = builder;
            _registrar = registrar;

            ScopeName = "message";

            builder.RegisterType<BusDepot>()
                .As<IBusDepot>()
                .SingleInstance();

            _builder.Register(context => new BusHealth(nameof(IBus)))
                .As<BusHealth>()
                .As<IBusHealth>()
                .SingleInstance();

            _builder.RegisterType<DefaultBusInstance>()
                .As<IBusInstance>()
                .SingleInstance();

            _builder.Register(GetCurrentSendEndpointProvider)
                .As<ISendEndpointProvider>()
                .InstancePerLifetimeScope();

            _builder.Register(GetCurrentPublishEndpoint)
                .As<IPublishEndpoint>()
                .InstancePerLifetimeScope();

            _builder.Register(context => ClientFactoryProvider(context.Resolve<IConfigurationServiceProvider>(), context.Resolve<IBus>()))
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

            builder.Register(provider => CreateRegistration(provider.Resolve<IConfigurationServiceProvider>()))
                .As<IRegistration>()
                .SingleInstance();
        }

        public string ScopeName
        {
            private get => _registrar.ScopeName;
            set => _registrar.ScopeName = value;
        }

        ContainerBuilder IContainerBuilderConfigurator.Builder => _builder;

        public Action<ContainerBuilder, ConsumeContext> ConfigureScope
        {
            get => _registrar.ConfigureScope;
            set => _registrar.ConfigureScope = value;
        }

        public void AddBus(Func<IRegistrationContext<IComponentContext>, IBusControl> busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured();

            _builder.Register(context => BusFactory(context, busFactory))
                .As<IBusControl>()
                .As<IBus>()
                .SingleInstance();
        }

        public void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory<IComponentContext>
        {
            throw new NotImplementedException();
        }

        IBusControl BusFactory(IComponentContext componentContext, Func<IRegistrationContext<IComponentContext>, IBusControl> busFactory)
        {
            var provider = componentContext.Resolve<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            IRegistrationContext<IComponentContext> context = GetRegistrationContext(componentContext);

            return busFactory(context);
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

            return new ScopedSendEndpointProvider<ILifetimeScope>(context.Resolve<IBus>(), context.Resolve<ILifetimeScope>());
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IComponentContext context)
        {
            if (context.TryResolve(out ConsumeContext consumeContext))
                return consumeContext;

            return new PublishEndpoint(new ScopedPublishEndpointProvider<ILifetimeScope>(context.Resolve<IBus>(), context.Resolve<ILifetimeScope>()));
        }

        IRegistrationContext<IComponentContext> GetRegistrationContext(IComponentContext context)
        {
            return new RegistrationContext<IComponentContext>(
                CreateRegistration(context.Resolve<IConfigurationServiceProvider>()),
                context.Resolve<BusHealth>(),
                context
            );
        }
    }
}
